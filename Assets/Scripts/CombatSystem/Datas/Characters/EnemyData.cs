using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Combat System/Enemy Data")]
public class EnemyData : CharacterData
{
    [Header("ACT OPTIONS")]
    [SerializeField] GameEvent firstOptionEvent;
    [SerializeField] GameEvent secondOptionEvent;

    [Header("ATTACKS")]
    [SerializeField] AttackPattern _attackPattern;
    [SerializeField] AnimationClip[] _attackClips = new AnimationClip[1]; // Minimum of 1 attack

    [Header("HP BASED ATTACK PATTERN")]
    [Tooltip("Every time the enemy loses the given threshold, the next attack will be selected")]
    [SerializeField] float _hpThreshold = 15;


    public override bool IsProtagonist { get => false; }

}
