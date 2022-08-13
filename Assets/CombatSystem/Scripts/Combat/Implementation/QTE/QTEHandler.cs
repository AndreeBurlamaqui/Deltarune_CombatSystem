using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TinyCacto.Utils;
using TinyCacto.Effects;

public class QTEHandler : MonoBehaviour
{
    public ProtagonistData runtimeCharacterData;

    [Header("VISUAL")]
    [SerializeField] float cursorGhostSpawnSpeed = 0.01f;
    [SerializeField] float cursorGhostFadeSpeed = 0.3f;
    [SerializeField] float cursorTapPunchStrength = 0.3f;
    [SerializeField] float cursorTapPunchSpeed = 0.3f;
    [SerializeField] Color cursorTapCriticalColor = Color.yellow;
    Color initialCursorColor;
    Coroutine ghostTrailRoutine;

    [Header("UI")]
    [SerializeField] Image characterIcon; 


    [Header("METER SYSTEM")]
    [SerializeField] RectTransform cursorSlider;
    Image _cursorImage;
    [SerializeField] RectTransform boxMeter;
    [SerializeField] float boxRange = 750f;
    [SerializeField] RectTransform criticalMeter;
    bool qteActivated = false;
    public bool IsQTEActive => qteActivated;

    [Header("EVENTS")]
    [SerializeField] GameEvent ShowScreenButtonEvent;
    [SerializeField] GameEvent HideScreenButtonEvent;
    [SerializeField] IntGameEvent SucessfulQTEEvent;
    [SerializeField] IntGameEvent CriticalQTEEvent;
    [SerializeField] IntGameEvent FailedQTEEvent;


    [Header("TESTING PURPOSE")]
    [SerializeField] bool testInPlay;
    [Tooltip("How fast in SECONDS the cursor will reach the end of the meter.")]
    [SerializeField] float cursorTime = 5;
    [Tooltip("Based on the start position, how much closer (less than 1) or further (more than 1)")]
    [SerializeField] float cursorStartPosOffset = 1;
    [SerializeField] FloatRange criticalRange = new(0, 50);

    #region PROPERTIES

    public float DamageReducer => Mathf.InverseLerp(boxRange, 0, cursorSlider.anchoredPosition.x);
    public bool IsCriticalHit => cursorSlider.anchoredPosition.x.IsWithin(criticalRange);

    public Image CursorImage
    {
        get
        {
            if (_cursorImage == null)
                _cursorImage = cursorSlider.GetComponent<Image>();

            return _cursorImage;
        }
    }

    #endregion

    private void Awake()
    {
        initialCursorColor = CursorImage.color;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        // Testing purposes
        if (criticalMeter == null || boxMeter == null)
            return;

        SetupCriticalMeter(criticalRange);
        SetupBoxMeter(boxRange);
    }

#endif

    public void SetupQTE(ProtagonistData _charData)
    {
        runtimeCharacterData = _charData;
        characterIcon.sprite = _charData.CharacterIcon;
        characterIcon.color = _charData.CharacterColor;
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
    private void SetupCriticalMeter(FloatRange range)
    {
        float currentEndRange = criticalMeter.anchoredPosition.y;
        float currentStartRange = criticalMeter.sizeDelta.x;

        if(currentEndRange == range.Minimum && currentStartRange == range.Maximum)
        {
            //Debug.Log("Critical Meter already setup");
            return;
        }

        criticalMeter.anchoredPosition = new Vector2(range.Minimum, currentEndRange);
        criticalMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, range.Maximum);
    }

    [ContextMenu("Test QTE")]
    private void TestQTE()
    {
        StartQTE(cursorTime, cursorStartPosOffset, new Vector2(0,50));
    }

    /// <summary>
    /// Start scrolling the cursor through the meter
    /// </summary>
    /// <param name="speed">Speed of the cursor</param>
    /// <param name="startPosOffset">How far from the start of the box meter the cursor should start. Less than 0 will count as already inside of the box meter.</param>
    public void StartQTE(float speed, float startPosOffset, Vector2 criticalMeterPosition)
    {
        qteActivated = true;

        gameObject.SetActive(true);
        
        CursorImage.SetAlpha(1);
        CursorImage.color = initialCursorColor;

        ghostTrailRoutine = CursorImage.SpawnGhostTrail(this, cursorGhostSpawnSpeed * speed, speed, cursorGhostFadeSpeed);
        Vector2 startPos = new Vector2(boxRange + startPosOffset, 0);

        // GO to 0 because, by the organization of the UI, it's where the cursor is outside of the meter (at the end)
        cursorSlider.DOAnchorPosX(0, speed).From(startPos).SetEase(Ease.Linear).OnComplete(FailQTE);
        ShowScreenButtonEvent?.Raise();
    }

    public void TryCompleteQTE()
    {
        if (!qteActivated)
            return;

        qteActivated = false;


        this.StopGhostTrail(ghostTrailRoutine);
        cursorSlider.DOKill(false);

        cursorSlider.DOPunchScale(Vector2.one * cursorTapPunchStrength, cursorTapPunchSpeed, 0,0);
        CursorImage.DOFade(0, cursorTapPunchSpeed/2).From(1);

        // If player couldn't do damage, count as if they failed
        if (DamageReducer <= 0)
        {
            // Quick explanation:
            // If the DamageReducer is below 0, it means that the player didn't wait for the cursor to be above the meter
            // And so it count as a failure

            FailQTE();
            return;
        }

        // If the player could do damage
        
        if (IsCriticalHit)
        {
            // If it's critical hit, we'll ignore the damage reducer
            CursorImage.color = cursorTapCriticalColor;

            // Raise the critical event
            CriticalQTEEvent.Raise(runtimeCharacterData._runtimeBattleID[0]);
        }
        else
        {
            // Otherwise, raise the normal QTE event
            SucessfulQTEEvent.Raise(runtimeCharacterData._runtimeBattleID[0]);
        }
    }

    public void FailQTE()
    {

        qteActivated = false;

        // Tell the game that we lost
        FailedQTEEvent.Raise(runtimeCharacterData._runtimeBattleID[0]);

        EndQTE();
    }

    public void EndQTE()
    {
        // Hide the UI that receives the tap
        HideScreenButtonEvent?.Raise();

        // Hide the bar
        gameObject.SetActive(false);
    }

}
