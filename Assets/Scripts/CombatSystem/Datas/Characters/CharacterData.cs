using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class CharacterData : ScriptableObject
{
    [Header("MAIN STATUS")]
    [SerializeField] string _name;
    [SerializeField] float _maxHP;
    float _curHP;

    [Header("VISUAL")]
    [SerializeField] List<Transform> _runtimeTransform = new List<Transform>();
    [SerializeField] List<SpriteRenderer> _runtimeVisual = new List<SpriteRenderer>();
    [SerializeField] AnimatorController _animator;

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
    public float CurrentHP => _curHP;
    public virtual bool IsProtagonist { get; }
    public IReadOnlyList<Transform> RuntimeTransform => _runtimeTransform;
    public IReadOnlyList<SpriteRenderer> RuntimeVisual => _runtimeVisual;
    public AnimatorController Animator => _animator;
    public Color CharacterColor => _characterColor;

    #endregion

    public void PlayAnimation()
    {
        //_characterAnimator.Play()
    }

    public void InitiateRuntimeData(params (Transform rTransform, SpriteRenderer rVisual)[] runtimes)
    {
        // Do a foreach loop with a tuple params so we can assure that each index will represent the same character
        foreach(var r in runtimes)
        {
            _runtimeTransform.Add(r.rTransform);
            _runtimeVisual.Add(r.rVisual);
        }

        _curHP = _maxHP;
    }

    public void ClearRuntimeData()
    {
        _runtimeTransform.Clear();
        _runtimeVisual.Clear();
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
