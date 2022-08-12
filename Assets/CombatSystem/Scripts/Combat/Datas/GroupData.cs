using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroupData", menuName = "Combat System/Group Data")]
public class GroupData : ScriptableObject
{
    /// <summary>
    /// Current protagonsists in the group.
    /// </summary>
    public List<ProtagonistData> Protagonists = new List<ProtagonistData>();


    public int GetIndexOf(ProtagonistData protag) => Protagonists.FindIndex(p => p == protag);
    
}
