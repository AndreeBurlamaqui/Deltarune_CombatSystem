using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] TMP_Text damageLabel;
    [SerializeField] float radiusRange = 3;
    [SerializeField] float showSpeed = 1;
    [SerializeField] float curvePower = 1;

    bool _isActive = false;
    CanvasGroup _canvaGroup;
    Camera _cam;

    public bool CurrentlyInUse
    {
        get => _isActive;
        set
        {
            _isActive = value;
            gameObject.SetActive(value);
        }

    }

    CanvasGroup CanvaGroup
    {
        get
        {
            if (_canvaGroup == null)
                _canvaGroup = GetComponent<CanvasGroup>();

            return _canvaGroup;
        }
    }

    Camera MainCamera
    {
        get
        {
            if (_cam == null)
                _cam = Camera.main;

            return _cam;
        }
    }


    public void ShowDamage(float damage, Vector3 position)
    {
        CurrentlyInUse = true;

        damageLabel.text = damage.ToString();

        Vector2 screenPos = MainCamera.WorldToScreenPoint(position);

        // Go to the center
        //Rect.position = screenPos;
        transform.position = screenPos + (5 * radiusRange * Time.fixedDeltaTime * Random.insideUnitCircle);

        // Get an offset around the center
        Vector3 offsetTween = screenPos + (10 * radiusRange * Time.fixedDeltaTime * Random.insideUnitCircle);

        // Tween
        transform.DOJump(offsetTween, Random.Range(-curvePower, curvePower), 1, showSpeed)
            .SetDelay(0.3f).OnComplete(() => CurrentlyInUse = false);

        CanvaGroup.DOFade(1, showSpeed / 2).From(0).SetLoops(1);
    }
}
