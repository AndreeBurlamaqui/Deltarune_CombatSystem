using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCacto.Utils;
using DG.Tweening;
using System;

public class BulletHellHandler : MonoBehaviour
{
    [Header("UI")]
    public TinyText flavorLabel;

    [Header("GAMEPLAY")]
    [SerializeField]private BattleHandler _battleHander;
    [SerializeField] RectTransform _heart; // Heart needs to be anchor full stretch
    public bool isOnBattle = false;
    [SerializeField] float _hurtCooldownMax = 2;
    [SerializeField] float _dodgeCooldownMax = 3;
    float hurtCooldown;
    float dodgeCooldown;
    public Vector3 HeartPosition => _heart.position;

    [Header("VISUAL")]
    [SerializeField] float hurtPunchStrength = 0.5f;
    [SerializeField] float hurtPunchDuration = 0.35f;
    [SerializeField] float dodgeFadeMax = 0.25f;
    [SerializeField] float dodgeFadeDuration = 0.75f;
    [SerializeField] CanvasGroup dodgeEffect;

    [Header("TESTING")]
    public EnemyAttack attackToTest;

    EnemyAttack currentSequence;


    [ContextMenu("TEST PREFAB ATTACK")]
    public void TestPrefab()
    {
        if (attackToTest == null)
            return;

        attackToTest.StartAttackSequence(this, null);
        isOnBattle = true;

    }

    public void StartBulletHell(EnemyAttack sequence, Action EndSequenceEvent)
    {
        gameObject.SetActive(true);
        isOnBattle = true;
        currentSequence = sequence;
        currentSequence.StartAttackSequence(this, () => FinishBulletHell(EndSequenceEvent));
    }

    private void FinishBulletHell(Action callback = null)
    {
        isOnBattle = false;
        callback?.Invoke();
        gameObject.SetActive(false);
    }
    
    public void StopBulletHell()
    {
        if (!isOnBattle)
            return; // Already over

        currentSequence.StopEnemyAttack();
        FinishBulletHell();
    }

    private void Update()
    {
        if (!isOnBattle)
            return;

        if (hurtCooldown >= 0)
            hurtCooldown -= Time.deltaTime;

        if (dodgeCooldown >= 0)
            dodgeCooldown -= Time.deltaTime;
    }

    public void OnHeartTriggerEvent()
    {
        if(dodgeCooldown <= 0)
        {
            HeartDodge();
        }
    }

    public void HeartHurt()
    {
        if (hurtCooldown > 0)
            return; // Can't be hurt

        Debug.Log("Heart got hurt!");
        hurtCooldown = _hurtCooldownMax;

        _heart.DOPunchScale(Vector3.one * hurtPunchStrength, hurtPunchDuration, 5 , 0.5f);

        _battleHander.HurtTargetProtagonist();
    }

    public void HeartDodge()
    {
        if (dodgeCooldown > 0 || hurtCooldown >= 0)
            return; // Can't dodge

        Debug.Log("Heart dodged!");
        dodgeCooldown = _dodgeCooldownMax;

        dodgeEffect.DOFade(dodgeFadeMax, dodgeFadeDuration).From(0).SetLoops(2, LoopType.Yoyo);

        _battleHander.IncreaseTP(5);
    }

}
