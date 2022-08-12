using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// We can also use ACTModule to apply item stuff
/// </summary>
public class ACTShiny : ACTModule
{
    [SerializeField] BattleHandler battleH;

    [Tooltip("From 0 <-> 100%")][Range(0,100)]
    [SerializeField] int magicCost;

    public override void ApplyACT()
    {
        battleH.TryUseMagic(magicCost, SuccessfullShine, FailureShine);

    }

    private void SuccessfullShine()
    {
        battleH.flavorText.Type("* They completely like it! As much as you do, maybe more! \n" +
    " * I think you'll have to stay wide eye open from now on, tho.", () => battleH.NextPlayerTurn());
    }
    private void FailureShine()
    {
        battleH.flavorText.Type("* They don't seem to quite like it as much as you do... \n" +
    " * It's okay, tho... It's them, not you...", () => battleH.NextPlayerTurn());
    }

}
