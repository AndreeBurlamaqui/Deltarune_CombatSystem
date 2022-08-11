using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TinyCacto.Utils;
using UnityEngine.Events;
using DG.Tweening;

public class EnemyStatus : MonoBehaviour
{
    public EnemyData currentEnemy_Data;
    int currentEnemy_ID;

    [Header("UI")]
    [SerializeField] Image HPSlider;
    [SerializeField] TMP_Text nameLabel;

    public UnityEvent<EnemyData, int> EnemySelectEvent;

    public void SetupEnemyStatus(EnemyData _enemyData, int id)
    {
        gameObject.SetActive(true);

        currentEnemy_Data = _enemyData;
        currentEnemy_ID = id;

        nameLabel.text = _enemyData.CharacterName;
        HPSlider.fillAmount = _enemyData.FillHPRange(id);
    }

    public void SelectEnemy()
    {
        EnemySelectEvent?.Invoke(currentEnemy_Data, currentEnemy_ID);
    }

    public void HighlightEnemy()
    {
        currentEnemy_Data.RuntimeVisual[currentEnemy_ID].DOFade(0.35f, 0.3f).From(1);
    }

    public void UnHighlightEnemy()
    {
        currentEnemy_Data.RuntimeVisual[currentEnemy_ID].DOFade(1, 0.3f).From(0.75f);
    }
}
