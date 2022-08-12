using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OB_CharacterData : MonoBehaviour
{

    [SerializeField] SpriteRenderer _visual;
    [SerializeField] Animator _animator;
    public SpriteRenderer VisualRenderer => _visual;
    public Animator Animator => _animator;

    internal abstract void Awake();

    internal void Initiate(CharacterData charData, Transform _runtimeTransform = null)
    {
        charData.ClearRuntimeData();

        if (_runtimeTransform == null)
            return;

        charData.InitiateRuntimeData((_runtimeTransform, _visual, _animator));
    }
}
