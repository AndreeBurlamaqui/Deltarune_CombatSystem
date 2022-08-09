using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProtagonistData", menuName = "Combat System/Protagonist Data")]
public class ProtagonistData : CharacterData
{

    [Header("UI")]
    [Tooltip("Icon that'll be displayed on bottom UI")]
    [SerializeField] Sprite _icon;

    public Sprite CharacterIcon => _icon;
    public override bool IsProtagonist { get => true; }
}
