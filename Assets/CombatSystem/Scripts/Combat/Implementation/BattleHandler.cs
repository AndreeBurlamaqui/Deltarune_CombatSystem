using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TinyCacto.Effects;
using TinyCacto.Utils;
using TMPro;
using System;
using UnityEngine.UI;

public class BattleHandler : MonoBehaviour
{
    [Header("EVENTS")]
    [SerializeField] GameEvent EndQTEEvent;
    [SerializeField] GameEvent BattleStartEvent;
    [SerializeField] GameEvent BattleEndEvent;

    [Header("BATTLE FEEDBACKS")]
    [SerializeField] float moveSpawnSpeed = 0.75f;
    [SerializeField] float ghostSpawnSpeed = 0.75f;
    [SerializeField] float ghostFadeSpeed = 0.5f;
    [SerializeField] Ease moveSpawnEase;
    [SerializeField] int waitAnimationDelay = 1;
    [SerializeField] DamageText[] damageTexts = new DamageText[6]; // We can only have 6 enemies, so no need to do more than that 
    public TinyText flavorText;

    [Header("PROTAGONISTS DATA")]
    public GroupData CurrentGroupData;

    [Header("SPAWNS POSITIONS")]
    [SerializeField] RectTransform[] _spawnsProtagonist = new RectTransform[3];
    [SerializeField] RectTransform[] _spawnsEnemy = new RectTransform[6];

    [Header("PROTAGONISTS")]
    [SerializeField] ProtagonistStatus[] _protagonistsStatus = new ProtagonistStatus[3];
    [SerializeField] QTEHandler[] _protagonistsQTE = new QTEHandler[3];

    [Header("OPTIONS")]
    [SerializeField] EnemyStatus[] _enemiesStatus = new EnemyStatus[6];
    [SerializeField] ACTStatus[] _actStatus = new ACTStatus[4];
    [SerializeField] ItemStatus[] _itemStatus = new ItemStatus[6];
    [SerializeField] TMP_Text itemDescriptionLabel;
    [SerializeField] Image tpBarPercentageSlider;
    [SerializeField] TMP_Text tpBarPercentageLabel;

    [Header("BULLET HELL")]
    [SerializeField] BulletHellHandler bulletHell;

    private BattleData currentBattleData;

    /// <summary>
    /// Battle ID of the selected protagonist
    /// </summary>
    private int lastProtagonistAttacked_ID;

    /// <summary>
    /// Battle ID of the selected enemy
    /// </summary>
    private int lastEnemyAttacked_ID;

    /// <summary>
    /// Runtime ID of enemies
    /// </summary>
    public int _runtimeEnemySelect_ID;

    /// <summary>
    /// How many protagonists are attacking now
    /// </summary>
    private int currentlyAttackingCount;
    
    /// <summary>
    /// Current runtime ID of the character in turn, can be either protagonist or enemy. 
    /// </summary>
    private int currentCharacterTurn_ID;
    private OptionChoice currentOptionChoice;
    int currentSelectedOption = -1;
    

    /// <summary>
    /// Key is battle id, value is if it's critical or not
    /// </summary>
    private Dictionary<int, bool> attackingCharacters = new();

    public BattleState currentBattleState;


    public ProtagonistData CurrentPlayerTurn_DATA => currentBattleData.InBattleProtagonists[currentCharacterTurn_ID];
    public EnemyData CurrentEnemyTurn_DATA => currentBattleData.InBattleEnemies[currentCharacterTurn_ID];
    public EnemyData LastEnemyAttacked_DATA => currentBattleData.InBattleEnemies[lastEnemyAttacked_ID];
    public ProtagonistData LastProtagonistAttacked_DATA => currentBattleData.InBattleProtagonists[lastProtagonistAttacked_ID];
    public CharacterData CurrentCharacterTurn_DATA => currentBattleData.InBattleCharacters[currentCharacterTurn_ID];

    #region UNITY METHODS

    public void OnDisable()
    {
        // End Battle forcefully

        EndLastBattle();
    }

    #endregion


    public void SetupBattle(BattleData battleInfo)
    {
        if (currentBattleData != null)
            EndLastBattle();


        // Disable every bottom UI so we can enable as we go and need
        DisableBottomUI();

        // Reset TPBar
        DecreaseTP(100);
        
        // Search for every Protagonists.cs
        // Because enemy will be the caller
        OB_Protagonist[] characters = FindObjectsOfType<OB_Protagonist>();

        foreach (OB_Protagonist pc in characters)
        {
            if (pc.Character == null)
                continue;

            pc.Character._runtimeBattleID.Clear();

            for(int pgd = 0; pgd < CurrentGroupData.Protagonists.Count; pgd++)
            {
                if (pc.Character == CurrentGroupData.Protagonists[pgd])
                {
                    // Add the character to the in battle list
                    // So that we know who is battling who
                    battleInfo.InBattleCharacters.Add(pc.Character);
                    battleInfo.InBattleProtagonists.Add(pgd, pc.Character);
                    pc.Character._runtimeBattleID.Add(pgd);

                    // Setup protagonists status
                    _protagonistsStatus[pgd].SetupStatus(pc.Character);
                    _protagonistsStatus[pgd].gameObject.SetActive(true);

                    // Setup QTE ui
                    _protagonistsQTE[pgd].SetupQTE(pc.Character);
                    break;
                }
            }
        }

        int enemySpawn = 0;
        // Move every protagonist and enemy into place
        for(int cd = 0; cd < battleInfo.InBattleCharacters.Count; cd++)
        {
            CharacterData character = battleInfo.InBattleCharacters[cd];

            if (battleInfo.InBattleCharacters == null)
            {
                // Remove and continue to the next
                battleInfo.InBattleCharacters.RemoveAt(cd);
                continue;
            }

            if (character.IsProtagonist)
            {
                // Move to protagonists' place
                // Order is defined Group Data list position
                int spawnPos = CurrentGroupData.GetIndexOf(battleInfo.InBattleProtagonists[character._runtimeBattleID[0]]);
                Vector3 fakeScreenPos = Camera.main.ScreenToWorldPoint(_spawnsProtagonist[spawnPos].position);
                character.MoveAllCharactersAt(fakeScreenPos, moveSpawnSpeed, moveSpawnEase, GhostTrailEffect);
            }
            else
            {
                // Move to enemies' place

                for(int e = 0; e < character.RuntimeTransform.Count; e++)
                {
                    Vector3 fakeScreenPos = Camera.main.ScreenToWorldPoint(_spawnsEnemy[enemySpawn].position);
                    character.MoveSingleCharacterAt(fakeScreenPos, moveSpawnSpeed, moveSpawnEase, e, GhostTrailEffect);
                    enemySpawn++;
                }
            }
        }

        HideEnemyList();

        currentBattleData = battleInfo;
        currentCharacterTurn_ID = -1; // Start as -1 because we'll trigger the next player turn and add 1 on that


        currentBattleState = BattleState.START;
        
        TinyTasks.WaitThenCall(this, 2, NextPlayerTurn);

        BattleStartEvent.Raise();
    }


    public void GhostTrailEffect(SpriteRenderer sprite)
    {
        sprite.SpawnGhostTrail(this, ghostSpawnSpeed, moveSpawnSpeed, ghostFadeSpeed);
    }

    public void EndLastBattle()
    {

        if (currentBattleData == null)
            return;

        currentBattleData.ClearBattleRuntimeInformations();

        currentBattleData = null;
    }

    public void FinishBattle(BattleState endResult)
    {
        EndLastBattle(); // Reset scriptable

        // In our case, reload scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    #region PLAYER TURN

    public void NextPlayerTurn()
    {
        Cursor.visible = true;

        if (currentBattleData == null)
            return;

        // Check if both died
        if (CheckPlayerDeath())
            return;

        if (flavorText.gameObject.activeSelf)
            flavorText.gameObject.SetActive(false);

        currentCharacterTurn_ID++;


        Debug.Log($"Character {currentCharacterTurn_ID} turn");

        if (currentCharacterTurn_ID >= currentBattleData.InBattleProtagonists.Count)
        {
            // It's now enemy turn
            currentCharacterTurn_ID = -1;
            NextEnemyTurn();
            return;
        }


        if (_protagonistsStatus[currentCharacterTurn_ID] == null)
        {
            // Something went wrong. Go to the enemy turn so we don't get stuck on the game
            // At least, the player will lose but will not crash or get stuck.
            currentCharacterTurn_ID = -1;
            NextEnemyTurn();
            return;
        }

        if (CurrentPlayerTurn_DATA.Lost(0))
        {
            Debug.Log($"{CurrentPlayerTurn_DATA.CharacterName} already lost, go to next available player");
            NextPlayerTurn();
            return;
        }

        // Hide QTEs
        HideAllQTE();

        currentBattleState = BattleState.PLAYERTURN;

        CurrentPlayerTurn_DATA.IsDefending = false;

        CurrentPlayerTurn_DATA.PlayAnimationOnCharacter(this, 0, CurrentCharacterTurn_DATA.IdleAnimation);

        if (CurrentPlayerTurn_DATA.DoInstaAttack)
        {
            // Insta attack the first enemy and go to the next turn
            currentOptionChoice = OptionChoice.CUSTOM;

            // Select the enemy
            // Due to the option choice to be custom, it'll not trigger QTE event
            SelectEnemy(currentBattleData.GetAnyAliveEnemy(out int eID), eID);

            // Call the normal damage as in a insta attack no critical damage will be dealt
            TinyTasks.WaitThenCall(this, 1, () => SucessfulAttack(currentCharacterTurn_ID));
        }
        else
        {
            _protagonistsStatus[currentCharacterTurn_ID].OpenActionbar(); // Force open it in case something went wrong
        }
    }

    private void HideAllQTE()
    {
        for (int qte = 0; qte < _protagonistsQTE.Length; qte++)
        {
            if (!_protagonistsQTE[qte].gameObject.activeSelf)
                continue; // Already hidden

            _protagonistsQTE[qte].EndQTE();
        }
    }

    private bool CheckPlayerDeath()
    {
        // Check if both died
        int howManyDied = 0;
        foreach (ProtagonistData charData in currentBattleData.InBattleProtagonists.Values)
        {
            if (charData.Lost(0))
                howManyDied++;
        }

        if (howManyDied == currentBattleData.InBattleProtagonists.Count)
        {
            FinishBattle(BattleState.LOST);
            return true;
        }


        return false;


    }

    #region FIGHT

    public void FightOption()
    {
        currentOptionChoice = OptionChoice.FIGHT;

        // Show the enemies
        ShowEnemyList();

    }
    private void TryAttack()
    {
        currentlyAttackingCount = currentBattleData.RandomFightersCount;


        _protagonistsStatus[currentCharacterTurn_ID].CloseActionbar();

        // Get the current player QTE bar
        _protagonistsQTE[currentCharacterTurn_ID].StartQTE(currentBattleData.RandomCursorTime, currentBattleData.RandomOffsetRange, new Vector2(0, 5));

        if (currentlyAttackingCount > 1)
        {
            // How many QTE evets we'll fire
            for (int qte = 0; qte < currentlyAttackingCount - 1; qte++)
            {
                if (_protagonistsQTE[qte].runtimeCharacterData.Lost(0))
                    continue; // Dead player, don't start it

                if (_protagonistsQTE[qte].IsQTEActive)
                    continue; // Already active, so probably it's from the main player

                float cursorSpeed = currentBattleData.RandomCursorTime;
                float randomOffset = currentBattleData.RandomOffsetRange;

                _protagonistsQTE[qte].StartQTE(cursorSpeed, randomOffset, new Vector2(0, 5));
            }
        }
    }

    public void SucessfulAttack(int charID)
    {
        // Get who's attacking
        ProtagonistData attackingChar = currentBattleData.InBattleProtagonists[charID];

        // Play the attack animation
        // Which should only call the event if it's the main character attacking (the one who selected the option)
        // Otherwise, the turn will end twice
        Action attackEvent = attackingChar._runtimeBattleID[0] == currentCharacterTurn_ID ? OnAttackAnimationEnd : null;

        attackingChar.PlayAnimationOnCharacter(this, 0, attackingChar.AttackAnimation, attackEvent, waitAnimationDelay);

        // Hurt enemy
        // Sum the last damage in case we had two cursors attack
        if(!attackingCharacters.ContainsKey(charID))
            attackingCharacters.Add(charID, false);

        TinyTasks.WaitThenCall(this, waitAnimationDelay, ApplyDamageOnEnemy);
    }

    public void CriticalAttack(int charID)
    {
        // Get who's attacking
        ProtagonistData attackingChar = currentBattleData.InBattleProtagonists[charID];

        // Play the attack animation
        // Which should only call the event if it's the main character attacking (the one who selected the option)
        // Otherwise, the turn will end twice
        Action attackEvent = attackingChar._runtimeBattleID[0] == currentCharacterTurn_ID ? OnAttackAnimationEnd : null;

        attackingChar.PlayAnimationOnCharacter(this, 0, attackingChar.AttackAnimation, attackEvent, waitAnimationDelay);

        // Hurt enemy        
        // Sum the last damage in case we had two cursors attack
        if (!attackingCharacters.ContainsKey(charID))
            attackingCharacters.Add(charID, true);

        TinyTasks.WaitThenCall(this, waitAnimationDelay, ApplyDamageOnEnemy);
    }

    public void FailedAttack(int charID)
    {
        // Check if the enemy's defense is 0 or below
        if (LastEnemyAttacked_DATA.GetCurrentDefense(lastEnemyAttacked_ID) <= 0)
        {
            // If so, we'll count it as a sucessful attack

            SucessfulAttack(charID);
            return;
        }

        // Avoid call it twice
        if (currentBattleData.InBattleProtagonists[charID]._runtimeBattleID[0] == currentCharacterTurn_ID)
            OnAttackAnimationEnd();

    }

    private void ApplyDamageOnEnemy()
    {
        Debug.Log($"Applying damage with {attackingCharacters.Count} attacking characters");

        foreach (int id in attackingCharacters.Keys)
        {
            CharacterData attacker = currentBattleData.InBattleProtagonists[id];
            CharacterData victim = LastEnemyAttacked_DATA;

            // TIP: You can create a second-hpbar boss by checkin IsDefending on ScriptableObject
            // So, it'll reduce every damage and the player will need to reduce it's DF status
            int damageReducer = victim.IsDefending ? victim.GetCurrentDefenseClamped(0) : 1;
            int damage = (attackingCharacters[id] ? attacker.RandomCriticalDamage : attacker.RandomDamage) / damageReducer;

            victim.SubtractCurrentHP(_runtimeEnemySelect_ID, damage);

            // Play hurt animation
            victim.PlayAnimationOnCharacter(this, _runtimeEnemySelect_ID, victim.HurtAnimation);

            foreach (DamageText dt in damageTexts)
            {
                if (dt.CurrentlyInUse)
                    continue;

                dt.ShowDamage(damage, victim.RuntimeVisual[_runtimeEnemySelect_ID].transform.position);
                break;
            }
        }

        attackingCharacters.Clear();
    }

    public void OnAttackAnimationEnd()
    {
        if (currentBattleState != BattleState.WAITING)
            return;

        Debug.Log("Animation ended");

        if (!CurrentPlayerTurn_DATA.DoInstaAttack)
            EndQTEEvent?.Raise(); // No need to raise this event if we insta attack, since we don't open the QTE

        NextPlayerTurn();
    }

    #endregion


    #region ACTion

    public void ActOption()
    {
        currentOptionChoice = OptionChoice.ACT;

        // Show enemies
        ShowEnemyList();
    }
    private void ShowActOptions()
    {
        HideAllOptions();

        // Get ACTion options
        int actAdded = 0;
        if (CurrentPlayerTurn_DATA.hasCheckACT)
        {
            // setup the first one as a check
            _actStatus[actAdded].SetupActOption(actAdded, "Check");
            actAdded++;
        }

        ACT[] opt = currentBattleData.actOptions.options;

        for (int a = 0; a < opt.Length; a++)
        {
            if (actAdded >= _actStatus.Length)
            {
                Debug.LogWarning($"More ACTions than max capacity (4) were tried to be added in battle. \n " +
                    $"{opt.Length - a} option(s) is/are going to be ignored.");
                break;
            }

            _actStatus[actAdded].SetupActOption(actAdded, opt[a].actName);
            actAdded++;
        }
    }
    private void HideActOptions()
    {
        foreach (ACTStatus acts in _actStatus)
            acts.gameObject.SetActive(false);
    }

    public void TryAct(int id)
    {
        _protagonistsStatus[currentCharacterTurn_ID].CloseActionbar();
        HideAllOptions();
        string actResultText = "...";

        int finalID = CurrentPlayerTurn_DATA.hasCheckACT ? id - 1 : 1;

        if (CurrentPlayerTurn_DATA.hasCheckACT && finalID < 0)
        {
            // Show stats of target enemy
            CharacterData enemyData = LastEnemyAttacked_DATA;
            actResultText =
                $"{enemyData.CharacterName} - AT {enemyData.MaxDamage}   DF {enemyData.GetCurrentDefense(_runtimeEnemySelect_ID)}" +
                $"\n\n{enemyData.flavorDescription}";

        }
        else
        {
            ACT selectedOption = currentBattleData.actOptions.options[finalID];

            selectedOption.actEvent.Raise();
            actResultText = selectedOption.actFlavor;
        }

        flavorText.Type(actResultText, () => TinyTasks.WaitThenCall(this, 3, OnEndAct));
    }

    private void OnEndAct()
    {
        flavorText.gameObject.SetActive(false);
        NextPlayerTurn();
    }


    #endregion


    #region ITEMS

    public void ItemOption()
    {
        currentOptionChoice = OptionChoice.ITEMS;

        // Show item options
        ShowItems();

    }

    public void ShowItems()
    {
        // First hide
        HideAllOptions();

        // And what's available based on current character's inventory
        List<ItemData> itm = CurrentPlayerTurn_DATA.Inventory.GetInventory();

        for (int i = 0; i < itm.Count; i++)
        {
            if (i >= _itemStatus.Length)
            {
                Debug.LogWarning($"More Items than max capacity (6) were tried to be added in battle. \n " +
                    $"{itm.Count - i} option(s) is/are going to be ignored.");
                break;
            }

            _itemStatus[i].SetupItemOption(i, itm[i].ItemName, itm[i].ItemDescription);
        }
    }

    public void TryItem(int id)
    {
        // Check if we're already selecting it
        if (currentSelectedOption == id)
        {
            // If so, use it and close it
            _itemStatus[id].UnhighlightItem();
            CurrentPlayerTurn_DATA.Inventory.UseItem(id);
            _protagonistsStatus[currentCharacterTurn_ID].CloseActionbar();
            HideAllOptions();
        }
        else
        {
            // Otherwise,
            // Select to show the description

            if (currentSelectedOption >= 0)
            {
                // Something was already selected, so unhighlight it
                _itemStatus[currentSelectedOption].UnhighlightItem();
            }

            currentSelectedOption = id;
            string desc = _itemStatus[id].ItemDescription;
            itemDescriptionLabel.text = desc;
            itemDescriptionLabel.gameObject.SetActive(true);
            _itemStatus[id].HighlightItem();
        }
    }

    private void HideItemsOptions()
    {
        itemDescriptionLabel.gameObject.SetActive(false);

        foreach (ItemStatus items in _itemStatus)
            items.gameObject.SetActive(false);
    }

    #endregion


    #region SPARE

    public void SpareOption()
    {
        currentOptionChoice = OptionChoice.SPARE;

        // Show the enemies
        ShowEnemyList();
    }

    public void TrySpare()
    {
        EnemyData curEnemy = LastEnemyAttacked_DATA;
        _protagonistsStatus[currentCharacterTurn_ID].CloseActionbar();

        if (!curEnemy.CanSpare(_runtimeEnemySelect_ID))
        {
            flavorText.Type("* You can't spare yet...", () => NextPlayerTurn());
            return;
        }

        flavorText.Type($"* You spared {curEnemy.CharacterName}", () => NextPlayerTurn());
        curEnemy._runtimeSpared[lastEnemyAttacked_ID] = true;
        //NextPlayerTurn();
    }

    #endregion


    #region DEFEND

    public void DefendOption()
    {
        currentOptionChoice = OptionChoice.DEFEND;
        _protagonistsStatus[currentCharacterTurn_ID].CloseActionbar();

        CharacterData curChar = CurrentPlayerTurn_DATA;
        curChar.IsDefending = true;
        curChar.PlayAnimationOnCharacter(this, 0, curChar.DefendAnimation);

        IncreaseTP(16);

        // Go to next turn
        NextPlayerTurn();
    }

    #endregion


    private void ShowEnemyList()
    {
        // First hide all options
        HideAllOptions();

        // So we can show the ones we want
        int enemyAdded = 0;

        foreach (EnemyData ed in currentBattleData.InBattleEnemies.Values)
        {
            if (enemyAdded >= _enemiesStatus.Length)
            {
                Debug.LogError("More enemies than max capacity (6) were added in battle. \n Please review the enemy count");
                return;
            }

            for (int i = 0; i < ed._runtimeBattleID.Count; i++)
            {
                _enemiesStatus[enemyAdded].SetupEnemyStatus(ed, ed._runtimeBattleID[i]);
                enemyAdded++;
            }
        }
    }

    public void SelectEnemy(EnemyData targetEnemy, int targetID)
    {
        currentBattleState = BattleState.WAITING;
        HideAllOptions();

        _runtimeEnemySelect_ID = targetID;

        switch (currentOptionChoice)
        {
            case OptionChoice.FIGHT:
                TryAttack();
                break;

            case OptionChoice.ACT:
                ShowActOptions();
                break;

            case OptionChoice.SPARE:
                TrySpare();
                break;

        }
    }

    public void HideAllOptions()
    {
        HideActOptions();
        HideEnemyList();
        HideItemsOptions();
    }


    #region TP BAR RELATED

    /// <summary>
    /// Increase the TPBar by a set <paramref name="percentage"/> read in 0-100%
    /// </summary>
    /// <param name="percentage">Read in 0-100%</param>
    public void IncreaseTP(int percentage)
    {
        if (tpBarPercentageSlider.fillAmount >= 1)
            return; // Already full

        tpBarPercentageSlider.fillAmount += (float)percentage / 100;
        tpBarPercentageLabel.text = tpBarPercentageSlider.fillAmount.FromPercentage().ToInt().ToString();

    }

    /// <summary>
    /// Decrease the TPBar by a set <paramref name="percentage"/> read in 0-100%
    /// </summary>
    /// <param name="percentage">Read in 0-100%</param>
    public void DecreaseTP(int percentage)
    {
        if (tpBarPercentageSlider.fillAmount <= 0)
            return; // Already empty

        tpBarPercentageSlider.fillAmount -= (float)percentage / 100;
        tpBarPercentageLabel.text = tpBarPercentageSlider.fillAmount.FromPercentage().ToInt().ToString();
    }

    /// <summary>
    /// Try to use magic by checking if the player has enough TP
    /// </summary>
    /// <param name="tpCost">Read in 0-100%</param>
    public void TryUseMagic(int tpCost, Action successfullCallback, Action failureCallback)
    {
        if (tpCost / 100 > tpBarPercentageSlider.fillAmount)
        {
            // Not enough TP to cast it
            failureCallback?.Invoke();
            return;
        }

        successfullCallback?.Invoke();
        DecreaseTP(tpCost);
    }

    #endregion

    #region UI RELATED

    private void HideEnemyList()
    {
        foreach (EnemyStatus es in _enemiesStatus)
            es.gameObject.SetActive(false);
    }
    private void DisableBottomUI()
    {
        foreach (ProtagonistStatus ps in _protagonistsStatus)
            ps.gameObject.SetActive(false);

        foreach (QTEHandler qteh in _protagonistsQTE)
            qteh.gameObject.SetActive(false);
    }

    #endregion

    #endregion


    #region ENEMY TURN

    public void NextEnemyTurn()
    {

        if (currentBattleData == null)
            return;

        // Check if both died
        if (CheckEnemyDeath())
            return;

        if (flavorText.gameObject.activeSelf)
            flavorText.gameObject.SetActive(false);



        currentCharacterTurn_ID++;

        currentBattleState = BattleState.ENEMYTURN;
        Debug.Log($"Enemy {currentCharacterTurn_ID} turn");

        if (currentCharacterTurn_ID >= currentBattleData.InBattleEnemies.Count)
        {

            // It's now player turn
            currentCharacterTurn_ID = -1;
            NextPlayerTurn();
            return;
        }

        // Save enemy id
        currentBattleData.GetAnyAliveEnemy(out int _runtimeID);
        _runtimeEnemySelect_ID = _runtimeID;

        if (CurrentEnemyTurn_DATA.Lost(_runtimeEnemySelect_ID))
        {
            NextEnemyTurn();
            return;
        }

        // Hide QTEs
        HideAllQTE();

        CurrentCharacterTurn_DATA.PlayAnimationOnCharacter(this, _runtimeEnemySelect_ID, CurrentCharacterTurn_DATA.IdleAnimation);

        // Select target
        lastProtagonistAttacked_ID = currentBattleData.InBattleProtagonists.RandomKey();

        if (LastProtagonistAttacked_DATA.IsDead(0))
        {
            // If dead, check if all of them are dead
            if (CheckPlayerDeath())
                return;

            // If not, get an alive one

            lastProtagonistAttacked_ID = currentBattleData.GetAnyAliveProtagonist()._runtimeBattleID[0];
        }

        if (LastProtagonistAttacked_DATA.IsDead(0))
        {
            // If still dead, then game over
            FinishBattle(BattleState.LOST);
            return;
        }

        Debug.Log("Enemy will attack " + LastProtagonistAttacked_DATA.CharacterName);

        bulletHell.StartBulletHell(CurrentEnemyTurn_DATA.GetRandomAttackSequence(), FinishSequence);
    }

    /// <summary>
    /// When the sequence of attacks (bullet hell) is finished
    /// </summary>
    public void FinishSequence()
    {

        NextEnemyTurn();
    }

    public void HurtTargetProtagonist()
    {
        // Get who's receiving
        ProtagonistData attackedChar = LastProtagonistAttacked_DATA;

        int damageReducer = attackedChar.IsDefending ? attackedChar.GetCurrentDefenseClamped(0) : 1;
        int damage = CurrentEnemyTurn_DATA.RandomDamage / damageReducer;

        attackedChar.SubtractCurrentHP(0, damage);

        // Play hurt animation
        attackedChar.PlayAnimationOnCharacter(this, 0, attackedChar.HurtAnimation);

        foreach (DamageText dt in damageTexts)
        {
            if (dt.CurrentlyInUse)
                continue;

            dt.ShowDamage(damage, attackedChar.RuntimeVisual[0].transform.position);
            break;
        }

        // Update UI
        UpdateProtagonistStatus();

        // If dead, stop bullet hell
        if (attackedChar.GetCurrentHP(0) <= 0)
        {
            bulletHell.StopBulletHell();

            FinishSequence();
        }
    }

    public void UpdateProtagonistStatus()
    {
        // Update UI
        foreach (ProtagonistStatus ps in _protagonistsStatus)
            ps.UpdateStatus();
    }

    private bool CheckEnemyDeath()
    {
        // Check if all of them died or got spared
        int lostCount = 0;

        foreach (EnemyData charData in currentBattleData.InBattleEnemies.Values)
        {
            for (int e = 0; e < charData.RuntimeTransform.Count; e++)
            {
                if (charData.Lost(e))
                    lostCount++;

            }
        }

        if (lostCount == currentBattleData.TotalEnemies)
        {
            FinishBattle(BattleState.WIN);
            return true;
        }
        
        return false;
    }
    #endregion

  

}

public enum OptionChoice
{
    FIGHT,
    ACT,
    ITEMS,
    DEFEND,
    SPARE,
    CUSTOM
}