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

    /// <summary>
    /// Setup the enemy status
    /// </summary>
    /// <param name="_enemyData">Data of the enemy that represents this status.</param>
    /// <param name="id">Runtime battle ID</param>
    public void SetupEnemyStatus(EnemyData _enemyData, int id)
    {
        gameObject.SetActive(true);

        currentEnemy_Data = _enemyData;
        currentEnemy_ID = id;

        nameLabel.text = _enemyData.CharacterName;
        HPSlider.fillAmount = _enemyData.FillHPRange(id);
    }

    /// <summary>
    /// By clicking the enemy name
    /// </summary>
    public void SelectEnemy()
    {
        if (currentEnemy_Data.Lost(currentEnemy_ID))
            return; // Already dead or spared


        EnemySelectEvent?.Invoke(currentEnemy_Data, currentEnemy_ID);

        UnHighlightEnemy();
    }

    /// <summary>
    /// When the cursor is entered above the enemy name.
    /// </summary>
    public void HighlightEnemy()
    {
        if (currentEnemy_Data.Lost(currentEnemy_ID))
            return; // Already dead or spared

        // Doing a simple fade to exemplify how the highlight would work
        // Can change to any visual representation.
        // Like spawn something on enemy position
        // Use custom shader and/or material to change additive color
        // etc

        currentEnemy_Data.RuntimeVisual[currentEnemy_ID].DOFade(0.35f, 0.3f).From(1);
    }

    /// <summary>
    /// When the cursor exited from the enemy name.
    /// </summary>
    public void UnHighlightEnemy()
    {
        if (currentEnemy_Data.Lost(currentEnemy_ID))
            return; // Already dead or spared

        currentEnemy_Data.RuntimeVisual[currentEnemy_ID].DOFade(1, 0.3f).From(0.75f);
    }
}
