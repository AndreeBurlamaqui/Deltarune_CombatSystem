using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ActOptions", menuName = "Combat System/ACTion Options")]
public class ActOptions : ScriptableObject
{

    public ACT[] options = new ACT[4]; // Max of four, one of them can be overlapped if check is enabled

}

[System.Serializable]
public struct ACT
{
    public string actName;
    [Multiline] public string actFlavor;
    public GameEvent actEvent;
}
