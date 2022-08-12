using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OB_Enemy : OB_CharacterData
{
    public EnemyData Character;

    internal override void Awake()
    {
        Initiate(Character);
    }
}
