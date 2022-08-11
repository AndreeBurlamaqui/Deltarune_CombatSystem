using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TinyCacto.Effects;
using TinyCacto.Utils;
using TMPro;

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
    [SerializeField] SimpleTypeText flavorText;

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
    [SerializeField] ACTStatus[] _actStatus = new ACTStatus[6];

    private BattleData currentBattleData;

    private CharacterData characterBeingAttacked_Data;
    private int characterBeingAttacked_ID;
    private int currentlyAttackingCount;
    
    private ProtagonistData currentCharacterTurn_Data;
    private int currentCharacterTurn_ID;
    private OptionChoice currentOptionChoice;
    

    /// <summary>
    /// Key is battle id, value is if it's critical or not
    /// </summary>
    private Dictionary<int, bool> attackingCharacters = new();

    public BattleState currentBattleState;

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

        int protagonistSpawn = 0;
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
                Vector3 fakeScreenPos = Camera.main.ScreenToWorldPoint(_spawnsProtagonist[protagonistSpawn].position);
                character.MoveAllCharactersAt(fakeScreenPos, moveSpawnSpeed, moveSpawnEase, GhostTrailEffect);
                protagonistSpawn++;
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


        currentBattleState = BattleState.Start;
        
        TinyTasks.WaitDelayThenCall(2.SecondsToMilliseconds(), NextPlayerTurn);

        BattleStartEvent.Raise();
    }

    private void HideEnemyList()
    {
        foreach (EnemyStatus es in _enemiesStatus)
            es.gameObject.SetActive(false);
    }

    private void NextPlayerTurn()
    {
        if (currentBattleData == null)
            return;


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

        currentBattleState = BattleState.PlayerTurn;

        currentCharacterTurn_Data = _protagonistsStatus[currentCharacterTurn_ID].currentCharacter;

        if (currentCharacterTurn_Data.DoInstaAttack)
        {
            // Insta attack the first enemy and go to the next turn
            currentOptionChoice = OptionChoice.FIGHT;
            SelectEnemy(currentBattleData.InBattleEnemies[0], 0);
        }
        else
        {
            _protagonistsStatus[currentCharacterTurn_ID].ToggleActionBar();
        }
    }

    public void GhostTrailEffect(SpriteRenderer sprite)
    {
        sprite.SpawnGhostTrail(this, ghostSpawnSpeed, moveSpawnSpeed, ghostFadeSpeed);
    }

    private void DisableBottomUI()
    {
        foreach (ProtagonistStatus ps in _protagonistsStatus)
            ps.gameObject.SetActive(false);

        foreach (QTEHandler qteh in _protagonistsQTE)
            qteh.gameObject.SetActive(false);
    }
    public void EndLastBattle()
    {

        //foreach (CharacterData cd in lastBattle.InBattleCharacters)
        //{
        //    if (cd == null)
        //        continue;

        //    cd.ClearRuntimeData();
        //}

        if (currentBattleData == null)
            return;

        currentBattleData.ClearBattleRuntimeInformations();

        currentBattleData = null;
    }

    public void SucessfulAttack(int charID)
    {      
        // Get who's attacking
        ProtagonistData attackingChar = currentBattleData.InBattleProtagonists[charID];

        // Play the attack animation
        attackingChar.PlayAnimationOnCharacter(0, attackingChar.AttackAnimation, OnAnimationEnd, waitAnimationDelay);

        // Hurt enemy
        // Sum the last damage in case we had two cursors attack
        attackingCharacters.Add(charID, false);

        TinyTasks.WaitDelayThenCall(waitAnimationDelay.SecondsToMilliseconds(), ApplyDamageOnEnemy);
    }

    public void CriticalAttack(int charID)
    {
        // Get who's attacking
        ProtagonistData attackingChar = currentBattleData.InBattleProtagonists[charID];

        // Play the attack animation
        attackingChar.PlayAnimationOnCharacter(0, attackingChar.AttackAnimation, OnAnimationEnd, waitAnimationDelay);

        // Hurt enemy        
        // Sum the last damage in case we had two cursors attack
        attackingCharacters.Add(charID, true);

        TinyTasks.WaitDelayThenCall(waitAnimationDelay.SecondsToMilliseconds(), ApplyDamageOnEnemy);
    }

    public void FailedAttack(int charID)
    {
        OnAnimationEnd();

    }

    private void ApplyDamageOnEnemy()
    {
        foreach (int id in attackingCharacters.Keys)
        {
            CharacterData attacker = currentBattleData.InBattleProtagonists[id];
            int damage = attackingCharacters[id] ? attacker.RandomCriticalDamage : attacker.RandomDamage; 
            CharacterData victim = characterBeingAttacked_Data;

            victim.SubtractCurrentHP(characterBeingAttacked_ID, damage);

            // Play hurt animation
            victim.PlayAnimationOnCharacter(characterBeingAttacked_ID, victim.HurtAnimation);

            foreach (DamageText dt in damageTexts)
            {
                if (dt.CurrentlyInUse)
                    continue;

                dt.ShowDamage(damage, victim.RuntimeVisual[characterBeingAttacked_ID].transform.position);
                break;
            }
        }

        attackingCharacters.Clear();
    }
    public void OnAnimationEnd()
    {
        if (currentBattleState != BattleState.Waiting)
            return;

        Debug.Log("Animation ended");
        EndQTEEvent?.Raise();
        NextPlayerTurn();
    }

    public void NextEnemyTurn()
    {
        currentBattleState = BattleState.EnemyTurn;
        Debug.Log("New enemy turn");
        NextPlayerTurn();
    }


    #region PROTAGONIST STATUS OPTIONS

    public void FightOption()
    {
        currentOptionChoice = OptionChoice.FIGHT;

        // Show the enemies
        ShowEnemyList();

    }

    public void ActOption()
    {
        currentOptionChoice = OptionChoice.ACT;

        // Show enemies
        ShowEnemyList();
    }

    public void ItemOption()
    {
        currentOptionChoice = OptionChoice.ITEMS;

        // Get item options
    }

    public void SpareOption()
    {
        currentOptionChoice = OptionChoice.SPARE;

        // Show the enemies
    }

    public void DefendOption()
    {
        currentOptionChoice = OptionChoice.DEFEND;

        // Go to next turn
        NextPlayerTurn();
    }

    private void ShowEnemyList()
    {
        // First hide all options
        HideEnemyList();

        // So we can show the ones we want
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

    public void SelectEnemy(CharacterData targetEnemy, int targetID)
    {
        currentBattleState = BattleState.Waiting;
        HideEnemyList();

        characterBeingAttacked_Data = targetEnemy;
        characterBeingAttacked_ID = targetID;

        switch(currentOptionChoice)
        {
            case OptionChoice.FIGHT:
                TryAttack();
                break;

            case OptionChoice.ACT:
                ShowActOptions();
                break;

        }
    }

    private void TryAttack()
    {
        currentlyAttackingCount = currentBattleData.RandomFightersCount;


        _protagonistsStatus[currentCharacterTurn_ID].ToggleActionBar();

        if (currentlyAttackingCount > 1)
        {
            // How many QTE evets we'll fire
            for (int qte = 0; qte < currentlyAttackingCount; qte++)
            {
                float cursorSpeed = currentBattleData.RandomCursorTime;
                float randomOffset = currentBattleData.RandomOffsetRange;

                _protagonistsQTE[qte].StartQTE(cursorSpeed, randomOffset, new Vector2(0, 5));
            }
        }
        else
        {
            float cursorSpeed = currentBattleData.RandomCursorTime;
            float randomOffset = currentBattleData.RandomOffsetRange;

            _protagonistsQTE[currentCharacterTurn_ID].StartQTE(cursorSpeed, randomOffset, new Vector2(0, 5));

        }
    }

    private void ShowActOptions()
    {
        HideActOptions();

        // Get ACTion options
        int actAdded = 0;
        if (currentCharacterTurn_Data.hasCheckACT)
        {
            // setup the first one as a check
            _actStatus[actAdded].SetupActOption(actAdded, "Check");
            actAdded++;
        }

        foreach (ACT a in currentBattleData.actOptions.options) 
        {
            _actStatus[actAdded].SetupActOption(actAdded, a.actName);
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
        
        HideActOptions();

        if (currentCharacterTurn_Data.hasCheckACT && id > 0)
        {
            TryCheck();
        }
        else
        {
            ACT selectedOption = currentBattleData.actOptions.options[id];

            flavorText.Type(selectedOption.actFlavor, () => TinyTasks.WaitDelayThenCall(3.SecondsToMilliseconds(), OnEndAct));

            selectedOption.actEvent.Raise();
        }
    }


    private void TryCheck()
    {
        // Show stats of target enemy
    }
    private void OnEndAct()
    {
        flavorText.gameObject.SetActive(false);
        NextPlayerTurn();
    }

    #endregion

}

public enum OptionChoice
{
    FIGHT,
    ACT,
    ITEMS,
    DEFEND,
    SPARE
}