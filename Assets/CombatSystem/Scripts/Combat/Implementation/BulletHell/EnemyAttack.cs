using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewAttackSequence", menuName = "Combat System/Enemy Attack/Attack Sequence")]
public class EnemyAttack : ScriptableObject
{
    [SerializeField] [TextArea(5, 20)] string _textBeforeAttack;
    [SerializeField] [TextArea(5, 20)] string _textAfterAttack;

    [SerializeField] List<AttackSequence> _attacks = new List<AttackSequence>();

    Action currentEndSequenceEvent;
    BulletHellHandler _runtimeRoutineHolder;
    List<GameObject> _runtimeSpawnedAttacks = new List<GameObject>();
    public string flavorBeforeAttack => _textBeforeAttack;
    public string flavorAfterAttack => _textAfterAttack;

    public void StartAttackSequence(BulletHellHandler routineHolder, Action EndSequenceEvent)
    {
        currentEndSequenceEvent = EndSequenceEvent;
        _runtimeRoutineHolder = routineHolder;
        _runtimeRoutineHolder.StartCoroutine(SpawnSequence());
    }

    IEnumerator SpawnSequence()
    {
        Debug.Log("Initiating type squence");
        yield return new WaitForSeconds(_runtimeRoutineHolder.flavorLabel.Type(_textBeforeAttack) * 2);

        Debug.Log("Attacking sequence!");

        foreach (AttackSequence atks in _attacks)
        {
            if (atks.actions == null)
                continue;

            if (atks.actions.Count <= 0)
                continue;


            atks.spawnModule.SpawnSpawner(_runtimeRoutineHolder);

            yield return new WaitForSeconds(atks.sequenceWait);

            for (int r = 0; r < atks.Repetitions; r++)
            {

                GameObject currentAttack = atks.spawnModule.SpawnAttack(_runtimeRoutineHolder);
                _runtimeSpawnedAttacks.Add(currentAttack);

                foreach (AttackTypeModule modules in atks.actions)
                {
                    if (modules == null)
                        continue;


                    modules.Attack(_runtimeRoutineHolder, currentAttack);

                    yield return new WaitForSeconds(modules.AttackDuration);
                }

                yield return new WaitForSeconds(atks.repeatDelay);
                atks.spawnModule.DespawnAttack(_runtimeRoutineHolder, currentAttack, atks.destroyDelay);
            }

            atks.spawnModule.DespawnSpawner();


            yield return new WaitForSeconds(atks.sequenceDelay);


            if (string.IsNullOrEmpty(atks.textDuringAttack))
                continue;

            // Type before continuing
            yield return new WaitForSeconds(_runtimeRoutineHolder.flavorLabel.Type(atks.textDuringAttack) * 2);

        }

        yield return new WaitForSeconds(_runtimeRoutineHolder.flavorLabel.Type(_textAfterAttack) * 2);

        //routineHolder.flavorLabel.gameObject.SetActive(false);

        currentEndSequenceEvent?.Invoke();
        DespawnSequence();
    }

    public void StopEnemyAttack()
    {
        _runtimeRoutineHolder.StopCoroutine(SpawnSequence());
        DespawnSequence();
        currentEndSequenceEvent?.Invoke();
    }

    void DespawnSequence()
    {
        foreach (AttackSequence atks in _attacks)
        {
            if (atks.actions == null)
                continue;

            if (atks.actions.Count <= 0)
                continue;


            atks.spawnModule.DespawnSpawner();

            foreach (GameObject spwn in _runtimeSpawnedAttacks)
            {
                if (spwn == null)
                    continue;

                atks.spawnModule.DespawnAttack(_runtimeRoutineHolder, spwn, atks.destroyDelay);

            }

            _runtimeSpawnedAttacks.Clear();
        }


    }

    private void OnValidate()
    {
        foreach (AttackSequence debugAtks in _attacks)
        {
            float duration = 0;

            foreach (AttackTypeModule modules in debugAtks.actions)
                duration += modules.AttackDuration;

            debugAtks.debugName = $"Attack duration: { debugAtks.sequenceWait + (debugAtks.repeatDelay * debugAtks.Repetitions) + debugAtks.sequenceDelay + duration}";
        }
    }

}

[System.Serializable]
public class AttackSequence
{
    [HideInInspector] public string debugName;
    public ATK_Spawn spawnModule;
    public List<AttackTypeModule> actions = new List<AttackTypeModule>();
    public float sequenceWait = 0;
    public float sequenceDelay = 1;
    public float repeatDelay = 1;
    [SerializeField] int repeatTimes = 1;
    public float destroyDelay = 0;
    [TextArea(5, 20)] public string textDuringAttack;

    public int Repetitions => Mathf.Clamp(repeatTimes, 1, 100);
}
