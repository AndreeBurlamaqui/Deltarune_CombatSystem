using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleCaller : MonoBehaviour
{
    [SerializeField] OB_Enemy[] enemies; 
    [SerializeField] BattleData battleData;

    public UnityEvent<BattleData> BattleCallerEvent;

    [Header("TESTING")]
    public bool InitiateBattleOnStart = false;

    private void Start()
    {
        if (!InitiateBattleOnStart)
            return;

        StartBattle();
    }

#if UNITY_EDITOR
    [ContextMenu("Start Battle")]
#endif
    public void StartBattle()
    {
        for (int obe = 0; obe < enemies.Length; obe++)
        {
            OB_Enemy obEnemy = enemies[obe];

            if (obEnemy == null)
                continue; // Avoid errors in case we forgot to assign something, at least it'll work.

            obEnemy.Character.InitiateRuntimeData((obEnemy.transform, obEnemy.VisualRenderer, obEnemy.Animator));
            obEnemy.Character._runtimeBattleID.Add(obe);

            if (battleData.InBattleCharacters.Contains(obEnemy.Character))
                continue;

            obEnemy.Character._runtimeBattleID.Clear();

            battleData.InBattleCharacters.Add(obEnemy.Character);
            battleData.InBattleEnemies.Add(obe, obEnemy.Character);
            obEnemy.Character._runtimeBattleID.Add(obe);
        }

        BattleCallerEvent?.Invoke(battleData);
    }

}
