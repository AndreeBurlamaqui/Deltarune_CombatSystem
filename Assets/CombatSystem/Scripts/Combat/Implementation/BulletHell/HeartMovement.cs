using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class HeartMovement : MonoBehaviour, IDragHandler,IEndDragHandler
{
    [Header("RING")]
    [SerializeField] RectTransform ringRect;

    [Header("HEART")]
    [SerializeField] RectTransform heart;
    [SerializeField] float speedSensitivity = 1;

    public void OnDrag(PointerEventData eventData)
    {
        if (DOTween.IsTweening(heart.transform))
            return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        heart.transform.localPosition += 50 * speedSensitivity * Time.deltaTime * (Vector3)eventData.delta.normalized;

        float fixOffsetMinX = Mathf.Clamp(heart.offsetMin.x, 0, 5000);
        float fixOffsetMinY = Mathf.Clamp(heart.offsetMin.y, 0, 5000);
        float fixOffsetMaxX = Mathf.Clamp(heart.offsetMax.x, -5000, 0);
        float fixOffsetMaxY =Mathf.Clamp(heart.offsetMax.y, -5000, 0);

        float fixedX = (fixOffsetMinX - heart.offsetMin.x) + (fixOffsetMaxX - heart.offsetMax.x);
        float fixedY = (fixOffsetMinY - heart.offsetMin.y) + (fixOffsetMaxY - heart.offsetMax.y);

        // Local position because we used offsetMax/Min
        heart.transform.localPosition += new Vector3(fixedX, fixedY);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


}
