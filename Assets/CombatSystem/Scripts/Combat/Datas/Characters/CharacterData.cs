using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TinyCacto.Utils;
using Random = UnityEngine.Random;

[Serializable]
public class CharacterData : ScriptableObject
{
    [Header("MAIN STATUS")]
    [SerializeField] string _name;
    [SerializeField] internal float _maxHP;
    internal List<float> _runtimeCurrentHP = new List<float>();
    [SerializeField] internal int _maxDefense;
    internal List<int> _runtimeCurrentDefense = new List<int>();

    /// <summary>
    /// Unique ID value to add on InGame"TypeCharacter"
    /// </summary>
    [HideInInspector] public List<int> _runtimeBattleID = new List<int>();
    [HideInInspector] public List<bool> _runtimeSpared = new List<bool>();

    /// <summary>
    /// Brief description of the character.
    /// Used on Check ACT.
    /// </summary>
    [TextArea(5,100)] public string flavorDescription;

    [SerializeField] FloatRange _damageRange = new(10, 50);
    [SerializeField] float _criticalHitMultiplier = 1.5f;
    [SerializeField] bool _defending;


    [Header("VISUAL")]
    internal List<Transform> _runtimeTransform = new List<Transform>();
    List<SpriteRenderer> _runtimeTransformPool = new List<SpriteRenderer>();
    internal List<SpriteRenderer> _runtimeVisual = new List<SpriteRenderer>();
    List<SpriteRenderer> _runtimeVisualPool = new List<SpriteRenderer>();
    internal List<Animator> _runtimeAnimator = new List<Animator>();

#if UNITY_EDITOR
    [Help("Expected trigger values. Change them only if it's different on the animator.", UnityEditor.MessageType.None)]
#endif
    [SerializeField] string _idleTriggerParameter = "Idle";
    [SerializeField] string _attackTriggerParameter = "Attack";
    [SerializeField] string _hurtTriggerParameter = "Hurt";
    [SerializeField] string _defendTriggerParameter = "Defend";
    [SerializeField] Color _characterColor = Color.white;


    #region PROPERTIES


    #region MAIN STATUS

    public string CharacterName => _name;
    public float MaxHP => _maxHP;
    public FloatRange HPRange => new FloatRange(0, MaxHP);
    public int MaxDefense => _maxDefense;
    // Remove all ToInt() if you want to keep break values
    // There's also a ShortString() method to call when displaying the values on a user-friendly way
    public int MinDamage => _damageRange.Minimum.ToInt();
    public int MinCriticalDamage => (MinDamage * _criticalHitMultiplier).ToInt();
    public int MaxDamage => _damageRange.Maximum.ToInt();
    public int MaxCriticalDamage => (MaxDamage * _criticalHitMultiplier).ToInt();
    public int RandomDamage => _damageRange.RandomInRange.ToInt();
    public int RandomCriticalDamage => (RandomDamage * _criticalHitMultiplier).ToInt();
    public bool IsDefending { get => _defending; set => _defending = value; }

    #endregion


    public virtual bool IsProtagonist { get; }

    public IReadOnlyList<Transform> RuntimeTransform => _runtimeTransform;
    public IReadOnlyList<SpriteRenderer> RuntimeVisual => _runtimeVisual;
    public Color CharacterColor => _characterColor;


    #region ANIMATIONS PARAMETERS

    public string IdleAnimation =>_idleTriggerParameter;
    public string AttackAnimation => _attackTriggerParameter;
    public string HurtAnimation => _hurtTriggerParameter;
    public string DefendAnimation => _defendTriggerParameter;

    #endregion

    #endregion

    public float GetCurrentHP(int index) => index >= _runtimeCurrentHP.Count ? 0 : _runtimeCurrentHP[index];

    public float GetCurrentHPClamped(int index) => Mathf.Clamp(GetCurrentHP(index), 0, MaxHP);
    public virtual float AddCurrentHP(int index, float value) => _runtimeCurrentHP[index] += value;
    public virtual float SubtractCurrentHP(int index, float value) => _runtimeCurrentHP[index] -= value;
    public float FillHPRange(int index) => GetCurrentHPClamped(index).InverseLerp(HPRange);
    public bool IsDead(int index)
    {
        bool dead = GetCurrentHP(index) <= 0;

        // When dead, make character's visual fade out
        if (dead) 
        {
            Color newColor = RuntimeVisual[index].color;
            newColor.a = 0.35f;
            RuntimeVisual[index].color = newColor;
        }
        return dead;
    }
    public bool WasSpared(int index) 
    {
        bool spared = _runtimeSpared[index];

        // When spared, make character's visual green
        if (spared)
            _runtimeVisual[index].color = Color.green;

        return spared;
    }
    public bool Lost(int index) => WasSpared(index) || IsDead(index);

    public int GetCurrentDefense(int index) => _runtimeCurrentDefense[index];
    public int GetCurrentDefenseClamped(int index) => Mathf.Clamp(_runtimeCurrentDefense[index], 1, MaxDefense); // Clamp to 1 because we'll use it in divisions
    public virtual int AddCurrentDefense(int index, int value) => _runtimeCurrentDefense[index] += value;
    public virtual int SubtractCurrentDefense(int index, int value) => _runtimeCurrentDefense[index] -= value;

    /// <summary>
    /// Play an animation on the target character ID.
    /// </summary>
    /// <param name="target">Battle ID or index position of the animator (0) for protagonists</param>
    /// <param name="triggerParameter">Name of the *trigger* parameter. For example <see cref="AttackAnimation"/></param>
    /// <param name="AnimationEndEvent">Callback action that will be called after the animation ends</param>
    /// <param name="extraWaitSeconds">Extra seconds that can be added before calling the callback</param>
    public void PlayAnimationOnCharacter(MonoBehaviour routineHolder, int target, string triggerParameter, Action AnimationEndEvent = null, int extraWaitSeconds = 0)
    {
        Animator targetAnimator = _runtimeAnimator[target];
        targetAnimator.SetTrigger(triggerParameter);

        if (AnimationEndEvent == null)
            return; // No need to do the rest if we aren't requesting a callback

        int animLength = targetAnimator.GetCurrentAnimatorClipInfo(0).Length;

        // Wait the animation clip length + some set extra seconds before calling the event to noticed the caller that it ended
        // Useful to coordinate visual feedbacks
        // Like displaying damage texts at the final of the attack animation
        TinyTasks.WaitThenCall(routineHolder, animLength + extraWaitSeconds, AnimationEndEvent);
    }


    public virtual void InitiateRuntimeData(params (Transform rTransform, SpriteRenderer rVisual, Animator rAnimator)[] runtimes)
    {
        Debug.Log("Initiating runtime data of " + CharacterName);
        // Do a foreach loop with a tuple params so we can assure that each index will represent the same character
        foreach(var r in runtimes)
        {
            _runtimeTransform.Add(r.rTransform);
            _runtimeVisual.Add(r.rVisual);
            _runtimeAnimator.Add(r.rAnimator);

            // Fix Status
            // TIP: This part can be disabled to make the character data to also work as a save file
            _runtimeCurrentHP.Add(_maxHP);
            _runtimeCurrentDefense.Add(_maxDefense);
            _runtimeSpared.Add(false);
        }

    }

    public virtual void ClearRuntimeData()
    {
        _runtimeTransform.Clear();
        _runtimeTransformPool.Clear();

        _runtimeVisual.Clear();
        _runtimeVisualPool.Clear();

        _runtimeAnimator.Clear();

        _runtimeCurrentHP.Clear();
        _runtimeCurrentDefense.Clear();
        _runtimeBattleID.Clear();

        _runtimeSpared.Clear();
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

