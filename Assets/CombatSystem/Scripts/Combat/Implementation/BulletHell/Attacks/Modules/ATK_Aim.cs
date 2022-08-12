using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[CreateAssetMenu(fileName = "NewAimAttack", menuName = "Combat System/Enemy Attack/New Aim Attack Module")]
public class ATK_Aim : AttackTypeModule
{

    [Help("[AIM MODULE]: \n\n" +
        "HOW IT WORKS: \n" +
        "> CONSTAINT AIM ENABLED => Keep getting the heart's position and aimValue will be the duration of the aiming.\n" +
        "> CONSTAINT AIM DISABLED => Gets the heart's posiiton once and aimValue will be how longer it'll take to aim at it." +
        "\n \n" +
        "> Aim Value will also be the Attack Duration.")]
    [SerializeField] bool constantAim = false;
    [SerializeField] float aimValue = 1;
    [SerializeField] bool overrideAttackDuration = false;

    public override void Attack(BulletHellHandler bulletHell, GameObject currentVisual)
    {
        base.Attack(bulletHell, currentVisual);

        if (overrideAttackDuration)
            AttackDuration = aimValue;


        Vector3 finalRot = bulletHell.HeartPosition - AttackTransform.position;
        DOVirtual.Float(0, 1, aimValue, ApplyAim).SetEase(Ease.Linear);


        void ApplyAim(float lerpValue)
        {
            if (constantAim)
                finalRot = bulletHell.HeartPosition - AttackTransform.position;

             AttackForward = Vector3.Lerp(AttackForward, finalRot.normalized, lerpValue);
        }
    }

}

