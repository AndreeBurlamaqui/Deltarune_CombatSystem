using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCacto.Utils;
using System;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Combat System/Enemy Data")]
public class EnemyData : CharacterData
{
    [Header("SPARE")]
    [Tooltip("An enemy will be spareable if below 0. And will start the battle with this value")]
    [SerializeField] int _startSpareFactor = 10;
    [SerializeField]List<float> _runtimeCurrentSpareFactor = new List<float>();
    [Tooltip("Percentage threshold that, when HP is below, this enemy will be spareable")]
    [SerializeField] int _healthPercentageSpareable;

    [Header("ATTACKS")]
    [SerializeField] EnemyAttack[] _attackActions = new EnemyAttack[1]; // Minimum of 1 attack

    [Header("HP BASED ATTACK PATTERN")]
    [Tooltip("Every time the enemy loses the given threshold, the next attack will be selected")]
    [SerializeField] float _hpThreshold = 15;


    public override bool IsProtagonist { get => false; }

    // SPARE FACTOR REDUCERS

    public float SubtractSpareFactor(int index, int value = 1) =>_runtimeCurrentSpareFactor[index] -= value;

    public bool CanSpare(int index) => _runtimeCurrentSpareFactor[index] <= 0;

    public override int SubtractCurrentDefense(int index, int value)
    {
        SubtractSpareFactor(index);

        return base.SubtractCurrentDefense(index, value);
    }
    public override float SubtractCurrentHP(int index, float value)
    {
        if (index >= _runtimeCurrentHP.Count)
            return 0;

        float newHP = base.SubtractCurrentHP(index, value);

        Debug.Log($"HP percentage of enemy {CharacterName} is {FillHPRange(index).FromPercentage()} against {_healthPercentageSpareable} spare hp factor");
        if (FillHPRange(index).FromPercentage() <= _healthPercentageSpareable) // If the HP is below our threshold %
            SubtractSpareFactor(index, _startSpareFactor); // Then make the enemy spareable by reducing the factor by it's max

        return newHP;
    }

    public override void InitiateRuntimeData(params (Transform rTransform, SpriteRenderer rVisual, Animator rAnimator)[] runtimes)
    {
        base.InitiateRuntimeData(runtimes);

        foreach (var r in runtimes)
        {
            _runtimeCurrentSpareFactor.Add(_startSpareFactor);
        }
    }

    public override void ClearRuntimeData()
    {
        base.ClearRuntimeData();

        _runtimeCurrentSpareFactor.Clear();
    }

    public EnemyAttack GetRandomAttackSequence()
    {
        return _attackActions.RandomContent();
    }
}
