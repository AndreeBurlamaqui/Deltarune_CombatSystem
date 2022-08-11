using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using DG.Tweening;
using TinyCacto.Utils;
using Random = UnityEngine.Random;

[Serializable]
public class CharacterData : ScriptableObject
{
    [Header("MAIN STATUS")]
    [SerializeField] string _name;
    [SerializeField] float _maxHP;
    [SerializeField] List<float> _runtimeCurrentHP = new List<float>();
    public List<int> _runtimeBattleID = new List<int>();

    [SerializeField] FloatRange _damageRange = new(10,50);
    [SerializeField] float _criticalHitMultiplier = 1.5f;
    [SerializeField] bool _defending;
    [Tooltip("If this is enabled, the character will insta attack when reaching its turn. Affect only protagonsits.")]
    [SerializeField] bool _instaAttack;

    [Header("VISUAL")]
    [SerializeField] List<Transform> _runtimeTransform = new List<Transform>();
    List<SpriteRenderer> _runtimeTransformPool = new List<SpriteRenderer>();
    [SerializeField] List<SpriteRenderer> _runtimeVisual = new List<SpriteRenderer>();
    List<SpriteRenderer> _runtimeVisualPool = new List<SpriteRenderer>();
    [SerializeField] AnimatorController _animator;
    [SerializeField] List<Animator> _runtimeAnimator = new List<Animator>();

#if UNITY_EDITOR
    [Help("Expected trigger values. Change them only if it's different on the animator.", UnityEditor.MessageType.None)]
#endif
    [SerializeField] string _idleTriggerParameter = "Idle";
    [SerializeField] string _attackTriggerParameter = "Attack";
    [SerializeField] string _hurtTriggerParameter = "Hurt";
    [SerializeField] string _defendTriggerParameter = "Defend";
    [SerializeField] Color _characterColor = Color.white;


    #region PROPERTIES

    public string CharacterName => _name;
    public float MaxHP => _maxHP;
    public FloatRange HPRange => new FloatRange(0, MaxHP);
    public int MinDamage => _damageRange.Minimum.ToInt();
    public int MinCriticalDamage => (MinDamage * _criticalHitMultiplier).ToInt();
    public int MaxDamage => _damageRange.Maximum.ToInt();
    public int MaxCriticalDamage => (MaxDamage * _criticalHitMultiplier).ToInt();
    public int RandomDamage => _damageRange.RandomInRange.ToInt();
    public int RandomCriticalDamage => (RandomDamage * _criticalHitMultiplier).ToInt();
    public bool IsDefending => _defending;
    public bool DoInstaAttack => _instaAttack;


    public virtual bool IsProtagonist { get; }

    public IReadOnlyList<Transform> RuntimeTransform => _runtimeTransform;
    public IReadOnlyList<SpriteRenderer> RuntimeVisual => _runtimeVisual;
    public AnimatorController Animator => _animator;
    public Color CharacterColor => _characterColor;

    #region ANIMATIONS PARAMETERS

    public string IdleAnimation =>_idleTriggerParameter;
    public string AttackAnimation => _attackTriggerParameter;
    public string HurtAnimation => _hurtTriggerParameter;
    public string DefendAnimation => _defendTriggerParameter;

    #endregion

    #endregion

    public float GetCurrentHP(int index) => _runtimeCurrentHP[index];
    public float AddCurrentHP(int index, float value) => _runtimeCurrentHP[index] += value;
    public float SubtractCurrentHP(int index, float value) => _runtimeCurrentHP[index] -= value;
    public float FillHPRange(int index) => GetCurrentHP(index).InverseLerp(HPRange);

    public void PlayAnimationOnCharacter(int target, string triggerParameter, Action AnimationEndEvent = null, int extraWaitSeconds = 0)
    {
        Animator targetAnimator = _runtimeAnimator[target];
        targetAnimator.SetTrigger(triggerParameter);

        int animLength = targetAnimator.GetCurrentAnimatorClipInfo(0).Length;

        TinyTasks.WaitDelayThenCall(animLength.SecondsToMilliseconds() + extraWaitSeconds.SecondsToMilliseconds(), AnimationEndEvent);
    }


    public void InitiateRuntimeData(params (Transform rTransform, SpriteRenderer rVisual, Animator rAnimator)[] runtimes)
    {
        // Do a foreach loop with a tuple params so we can assure that each index will represent the same character
        foreach(var r in runtimes)
        {
            _runtimeTransform.Add(r.rTransform);
            _runtimeVisual.Add(r.rVisual);
            _runtimeAnimator.Add(r.rAnimator);

            // Fix HP
            // TIP: We can disable this part if we want for the character data to also work as a save file
            _runtimeCurrentHP.Add(_maxHP);
        }

    }

    public void ClearRuntimeData()
    {
        _runtimeTransform.Clear();
        _runtimeTransformPool.Clear();

        _runtimeVisual.Clear();
        _runtimeVisualPool.Clear();

        _runtimeAnimator.Clear();

        _runtimeCurrentHP.Clear();
        _runtimeBattleID.Clear();
    }

    public void MoveAllCharactersAt(Vector3 worldPos, float moveSpeed, Ease moveEase, Action<SpriteRenderer> movingEffect)
    {
        worldPos.z = 0;

        for(int t = 0; t < RuntimeTransform.Count; t++)
        {
            if (RuntimeTransform[t] == null)
            {
                // Remove it and continue to the next
                _runtimeTransform.RemoveAt(t);
                continue;
            }
            movingEffect?.Invoke(RuntimeVisual[t]);
            RuntimeTransform[t].DOMove(worldPos, moveSpeed).SetEase(moveEase);
        }
    }
    public void MoveSingleCharacterAt(Vector3 worldPos, float moveSpeed, Ease moveEase, int targetCharacter, Action<SpriteRenderer> movingEffect)
    {
        worldPos.z = 0;

        if (RuntimeTransform[targetCharacter] == null)
        {
            // Remove it and continue to the next
            _runtimeTransform.RemoveAt(targetCharacter);
            return;
        }

        movingEffect?.Invoke(RuntimeVisual[targetCharacter]);
        RuntimeTransform[targetCharacter].DOMove(worldPos, moveSpeed).SetEase(moveEase);

    }
}

/// <summary>
/// How will the attack be selected?
/// </summary>
public enum AttackPattern
{
    /// <summary>
    /// Attacks are choosen in order, when it reachs the end, it goes back to the beginning.
    /// </summary>
    Sequence,

    /// <summary>
    /// Totally random selection of attacks.
    /// </summary>
    Random,

    /// <summary>
    /// Based on a threshold, it'll select the next attack everytime it goes past that threshold.
    /// </summary>
    HPBased

}
