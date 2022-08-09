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


        foreach (OB_Enemy obe in enemies)
        {
            obe.Character.InitiateRuntimeData((obe.transform, obe.VisualRenderer));

            if (battleData.InBattleCharacters.Contains(obe.Character))
                continue;

            battleData.InBattleCharacters.Add(obe.Character);
        }

        BattleCallerEvent?.Invoke(battleData);
    }

}
