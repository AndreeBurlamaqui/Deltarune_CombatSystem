using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// We can also use ACTModule to apply item stuff
/// </summary>
public class ACTHeal : ACTModule
{
    [SerializeField] BattleHandler battleH;
    [SerializeField] [Range(0,100)] int healPoint;
    public override void ApplyACT()
    {
        battleH.CurrentPlayerTurn_DATA.AddCurrentHP(0, healPoint);
        battleH.UpdateProtagonistStatus();
        battleH.flavorText.Type($"* You got healed by {healPoint} points!", () => battleH.NextPlayerTurn());
    }

}
