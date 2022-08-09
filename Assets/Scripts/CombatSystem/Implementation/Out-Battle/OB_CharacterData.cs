using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OB_CharacterData : MonoBehaviour
{

    [SerializeField] SpriteRenderer visual;
    public SpriteRenderer VisualRenderer => visual;

    internal abstract void Awake();

    internal void Initiate(CharacterData charData, Transform _runtimeTransform = null)
    {
        charData.ClearRuntimeData();

        GetComponent<Animator>().runtimeAnimatorController = charData.Animator;

        if (_runtimeTransform == null)
            return;

        charData.InitiateRuntimeData((_runtimeTransform, visual));
    }
}
