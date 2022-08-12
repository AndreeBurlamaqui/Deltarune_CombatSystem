using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AttackTypeModule : ScriptableObject
{
    [Help("[ATTACK TYPE]: \n\n" +
    "HOW IT WORKS: \n" +
        "> Assign the Attack Visual Prefab. Remember that if a Position Module isn't assigned, the spawn point will be the prefab's origin.\n" +
        "> Set the Forward type. To know the right one, check the prefab's coloured arrows by selecting move tool (shorcut 'W'). " +
        "And then select the arrow the attack is pointing to. \n" +
        "> Set the Extra Attack Duration, which will be added to the Module's Attack Duration")]
    [SerializeField] ForwardSide _forward = ForwardSide.GreenArrow;
    [SerializeField] float _extraAttackDuration = 0;
    float attackDuration = 0;
    GameObject runtimeVisual;

    public Transform AttackTransform => runtimeVisual != null ? runtimeVisual.transform : null;
    public float AttackDuration { get => attackDuration + _extraAttackDuration; set => attackDuration = value; }
    public Vector3 AttackForward
    {
        get
        {
            switch (_forward)
            {
                case ForwardSide.GreenArrow:
                    return AttackTransform.up;

                case ForwardSide.InverseGreenArrow:
                    return -AttackTransform.up;

                case ForwardSide.RedArrow:
                    return AttackTransform.right;

                case ForwardSide.InverseRedArrow:
                    return -AttackTransform.right;

            }

            return AttackTransform.up;
        }
        set
        {

            switch(_forward)
            {
                case ForwardSide.GreenArrow:
                    AttackTransform.up = value;
                    break;
                case ForwardSide.InverseGreenArrow:
                    AttackTransform.up = -value;
                    break;
                case ForwardSide.RedArrow:
                    AttackTransform.right = value;
                    break;
                case ForwardSide.InverseRedArrow:
                    AttackTransform.right = -value;
                    break;
            }

        }
    }


    
    public virtual void Attack(BulletHellHandler bulletHell, GameObject visual)
    {
        runtimeVisual = visual;

        attackDuration = 0;
    }

    
}

/// <summary>
/// Which is the forward side of the prefab.
/// Check what you want to use as a "aim" point based on the colored arrows on editor.
/// </summary>
public enum ForwardSide
{
    /// <summary>
    /// UP
    /// </summary>
    GreenArrow,

    /// <summary>
    /// DOWN
    /// </summary>
    InverseGreenArrow,

    /// <summary>
    /// RIGHT
    /// </summary>
    RedArrow,

    /// <summary>
    /// LEFT
    /// </summary>
    InverseRedArrow
}
