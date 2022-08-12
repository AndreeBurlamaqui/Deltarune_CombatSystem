using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTBow : ACTModule
{
    [SerializeField] BattleHandler battleH;
    public override void ApplyACT()
    {
        Debug.Log("Applying ACTion BOW effect");
        battleH.LastEnemyAttacked_DATA.SubtractCurrentDefense(battleH._runtimeEnemySelect_ID, 1);

    }

}
