using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleHandler : MonoBehaviour
{
    [Header("BATTLE FEEDBACKS")]
    [SerializeField] float moveSpawnSpeed = 0.75f;
    [SerializeField] float ghostSpawnSpeed = 0.75f;
    [SerializeField] float ghostFadeSpeed = 0.5f;
    [SerializeField] Ease moveSpawnEase;

    [Header("PROTAGONISTS DATA")]
    public GroupData CurrentGroupData;

    [Header("SPAWNS POSITIONS")]
    [SerializeField] RectTransform[] _spawnsProtagonist = new RectTransform[3];
    [SerializeField] RectTransform[] _spawnsEnemy = new RectTransform[6];

    [Header("PROTAGONISTS")]
    [SerializeField] ProtagonistStatus[] _protagonistsStatus = new ProtagonistStatus[3];
    [SerializeField] QTEHandler[] _protagonistsQTE = new QTEHandler[3];

    private BattleData currentBattle;

    public void SetupBattle(BattleData battleInfo)
    {
        if (currentBattle != null)
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

            for(int pgd = 0; pgd < CurrentGroupData.Protagonists.Count; pgd++)
            {
                if (pc.Character == CurrentGroupData.Protagonists[pgd])
                {
                    // Add the character to the in battle list
                    // So that we know who is battling who
                    battleInfo.InBattleCharacters.Add(pc.Character);

                    // Setup protagonists status
                    _protagonistsStatus[pgd].SetupStatus(pc.Character);
                    _protagonistsStatus[pgd].gameObject.SetActive(true);

                    // Setup QTE ui
                    _protagonistsQTE[pgd].SetupQTE(pc.Character.CharacterIcon);
                    break;
                }
            }
        }

        int protagonistSpawn = 0;
        int enemySpawn = 0;
        // Move every protagonist and enemy into place
        foreach (CharacterData cd in battleInfo.InBattleCharacters)
        {
            if(battleInfo.InBattleCharacters == null)
            {
                // Remove and continue to the next
                battleInfo.InBattleCharacters.Remove(cd);
                continue;
            }

            if (cd.IsProtagonist)
            {
                // Move to protagonists' place
                Vector3 fakeScreenPos = Camera.main.ScreenToWorldPoint(_spawnsProtagonist[protagonistSpawn].position);
                Debug.Log($"Moving {cd.CharacterName} to position {protagonistSpawn} at {fakeScreenPos}");
                cd.MoveAllCharactersAt(fakeScreenPos, moveSpawnSpeed, moveSpawnEase, GhostTrailEffect);
                protagonistSpawn++;
            }
            else
            {
                // Move to enemies' place

                for(int e = 0; e < cd.RuntimeTransform.Count; e++)
                {
                    Debug.Log($"Moving {cd.CharacterName} to position {enemySpawn}");
                    Vector3 fakeScreenPos = Camera.main.ScreenToWorldPoint(_spawnsEnemy[enemySpawn].position);
                    cd.MoveSingleCharacterAt(fakeScreenPos, moveSpawnSpeed, moveSpawnEase, e, GhostTrailEffect);
                    enemySpawn++;
                }
            }
        }


        currentBattle = battleInfo;

        _protagonistsStatus[0].ToggleActionBar();
    }

    private void DisableBottomUI()
    {
        foreach (ProtagonistStatus ps in _protagonistsStatus)
            ps.gameObject.SetActive(false);

        foreach (QTEHandler qteh in _protagonistsQTE)
            qteh.gameObject.SetActive(false);
    }

    private void GhostTrailEffect(SpriteRenderer currentSprite)
    {
        //GameObject ghost = Instantiate(currentSprite.gameObject, currentSprite.transform.position, Quaternion.identity);
        //ghost.GetComponent<SpriteRenderer>().DOFade(0, ghostFadeSpeed).From(1).OnComplete(() => Destroy(ghost.gameObject));
        Debug.Log("starting ghost trail effect" + currentSprite, currentSprite.gameObject);
        StartCoroutine(SpawnGhostTrail(currentSprite.gameObject));
    }

    private IEnumerator SpawnGhostTrail(GameObject visualGO)
    {
        var interval = ghostSpawnSpeed;
        var time = 0f;
        var startTime = Time.time;
        while (Time.time - startTime < moveSpawnSpeed)
        {
            if (time + interval < Time.time - startTime)
            {
                if (visualGO == null)
                {
                    break;
                }

                GameObject go = Instantiate(visualGO);
                go.GetComponent<SpriteRenderer>().DOFade(0, ghostFadeSpeed).From(1).OnComplete(() => Destroy(go));

                Destroy(go.GetComponent<Animator>());
                go.transform.position = visualGO.transform.position;
                go.transform.rotation = visualGO.transform.rotation;
                go.transform.localScale = visualGO.transform.localScale;
                time = Time.time - startTime;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void EndLastBattle()
    {

        //foreach (CharacterData cd in lastBattle.InBattleCharacters)
        //{
        //    if (cd == null)
        //        continue;

        //    cd.ClearRuntimeData();
        //}

        if (currentBattle == null)
            return;

        currentBattle.InBattleCharacters.Clear();

        currentBattle = null;
    }

    public void OnDisable()
    {
        // End Battle forcefully

        EndLastBattle();
    }


    #region PROTAGONIST STATUS OPTIONS


    public void FightOption()
    {
        for(int qte = 0; qte < currentBattle.RandomFightersCount; qte++)
        {
            // How many QTE evets we'll fire
            _protagonistsQTE[qte].StartQTE(currentBattle.RandomCursorTime, currentBattle.RandomOffsetRange);
        }
    }

    #endregion

}
