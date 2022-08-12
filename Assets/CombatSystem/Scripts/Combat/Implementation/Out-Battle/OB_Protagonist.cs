using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OB_Protagonist : OB_CharacterData
{
    [SerializeField] ProtagonistData _character;
    public ProtagonistData Character 
    { 
        get
        {
            return _character;
        } 
    }

    internal override void Awake()
    {
        Initiate(Character, transform);
    }
}
