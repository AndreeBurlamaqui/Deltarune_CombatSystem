using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QTEHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image characterIcon; 

    [Header("METER SYSTEM")]
    [SerializeField] RectTransform cursorSlider;
    [SerializeField] RectTransform boxMeter;
    [SerializeField] float boxRange = 750f;
    [SerializeField] RectTransform criticalMeter;
    [SerializeField] Vector2 criticalRange = new Vector2(0,50);
    [SerializeField] GameEvent ShowScreenButtonEvent;
    [SerializeField] GameEvent HideScreenButtonEvent;


    [Header("TESTING PURPOSE")]
    [SerializeField] bool testInPlay;
    [Tooltip("How fast in SECONDS the cursor will reach the end of the meter.")]
    [SerializeField] float cursorTime = 5;
    [Tooltip("Based on the start position, how much closer (less than 1) or further (more than 1)")]
    [SerializeField] float cursorStartPosOffset = 1;



    private void OnValidate()
    {
        // Testing purposes
        if (criticalMeter == null || boxMeter == null)
            return;

        SetupCriticalMeter(criticalRange.x, criticalRange.y);
        SetupBoxMeter(boxRange);
    }

    public void SetupQTE(Sprite _charIcon)
    {
        characterIcon.sprite = _charIcon;
    }

    /// <summary>
    /// Setup the end and initial range of the critical meter.
    /// <para>I.e. where the cursor needs to be to apply the critical.</para>
    /// </summary>
    private void SetupBoxMeter(float startRange)
    {
        float currentStartRange = boxMeter.sizeDelta.x;

        if (currentStartRange == startRange)
        {
            //Debug.Log("Box Meter already setup");
            return;
        }

        boxMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startRange);
    }

    /// <summary>
    /// Setup the end and initial range of the critical meter.
    /// <para>I.e. where the cursor needs to be to apply the critical.</para>
    /// </summary>
    private void SetupCriticalMeter(float endRange, float startRange)
    {
        float currentEndRange = criticalMeter.anchoredPosition.y;
        float currentStartRange = criticalMeter.sizeDelta.x;

        if(currentEndRange == endRange && currentStartRange == startRange)
        {
            //Debug.Log("Critical Meter already setup");
            return;
        }

        criticalMeter.anchoredPosition = new Vector2(endRange, currentEndRange);
        criticalMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startRange);
    }

    [ContextMenu("Test QTE")]
    private void TestQTE()
    {
        StartQTE(cursorTime, cursorStartPosOffset);
    }

    /// <summary>
    /// Start scrolling the cursor through the meter
    /// </summary>
    /// <param name="speed">Speed of the cursor</param>
    /// <param name="startPosOffset">How far from the start of the box meter the cursor should start. Less than 0 will count as already inside of the box meter.</param>
    public void StartQTE(float speed, float startPosOffset = 0)
    {
        gameObject.SetActive(true);

        Vector2 startPos = new Vector2(boxRange + startPosOffset, 0);

        cursorSlider.DOAnchorPosX(0, speed).From(startPos).SetEase(Ease.Linear).OnComplete(FailQTE);
        ShowScreenButtonEvent?.Raise();
    }

    public void TryCompleteQTE()
    {
        cursorSlider.DOKill(false);

        float currentSliderPosition = cursorSlider.anchoredPosition.x;
        Debug.Log($"Trying to apply damage based on boxRange {boxRange} vs currentSliderPosition {currentSliderPosition}");
        float damageReducer = Mathf.InverseLerp(boxRange, 0, currentSliderPosition);
        Debug.Log("Applied attack when cursor is on position " + damageReducer);


        // If we couldn't do damage, count as if we failed
        if (damageReducer <= 0)
        {
            FailQTE();
            return;
        }

        // If we could do damage

        HideScreenButtonEvent?.Raise();
        EndQTE();
    }

    public void FailQTE()
    {
        Debug.Log("QTE Failed, no damage applied");
        HideScreenButtonEvent?.Raise();
        EndQTE();
    }

    public void EndQTE()
    {
        gameObject.SetActive(false);
    }

}
