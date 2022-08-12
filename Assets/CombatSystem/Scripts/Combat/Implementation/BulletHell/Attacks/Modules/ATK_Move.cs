using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "NewMoveAttack", menuName = "Combat System/Enemy Attack/New Move Attack Module")]
public class ATK_Move : AttackTypeModule
{
    [Help("[MOVE MODULE]: \n\n" +
       "HOW IT WORKS: \n" +
       "> Move distance is how much it'll move. \n" +
        "> Move speed is how fast it'll move" )]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;
    public override void Attack(BulletHellHandler bulletHell, GameObject currentVisual)
    {
        base.Attack(bulletHell, currentVisual);
        //float referenceResolutionWidth = 800;
        //float screenWidthsPerSecond = 2.5f;
        //Vector3 finalPos = AttackTransform.position + (moveDistance * (referenceResolutionWidth / screenWidthsPerSecond) * Time.deltaTime * AttackForward.normalized);
        //AttackTransform.DOMove(finalPos, moveSpeed).SetEase(Ease.Linear);

        bulletHell.StartCoroutine(ApplyMove(AttackTransform, AttackForward));

        IEnumerator ApplyMove(Transform toMove, Vector3 forward)
        {
            float startTime = Time.time;
            while (Time.time - startTime < moveDistance)
            {
                if (toMove == null)
                    break;
                // magic number 2 to make move speed value smaller, so we don't need to assign 4 decimals values, haha
                toMove.localPosition += ((2 *moveSpeed * forward) * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
        }
    }


}
