using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ActOptions", menuName = "Combat System/ACTion Options")]
public class ActOptions : ScriptableObject
{

    [Header("FIRST OPTION")]
    public string firstOptionName;
    public string firstOptionFlavor;
    public GameEvent firstOptionAction; 

    [Header("SECOND OPTION")]
    public string secondOptionName;
    public string secondOptionFlavor;
    public GameEvent secondOptionAction;

}
