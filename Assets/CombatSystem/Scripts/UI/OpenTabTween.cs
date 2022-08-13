using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class OpenTabTween : MonoBehaviour
{
    [SerializeField] RectTransform rectTransformToUpdate;
    [SerializeField] bool tweenWidth;
    [SerializeField] bool tweenHeight;
    [SerializeField] float tweenDuration = 0.35f;
    public float animationDuration { get => tweenDuration; }
    [SerializeField] Ease easeType = Ease.Linear;

    [SerializeField] bool hideChildrensWhileTween;
    [Header("Childrens that shouldn't open with this")]
    [SerializeField] Transform[] openException;

    private float startWidth, startHeight;
    private RectTransform rt;
    private RectTransform TabRect
    {
        get
        {
            if (rt == null)
            {
                rt = GetComponent<RectTransform>();

                startHeight = rt.rect.height;
                startWidth = rt.rect.width;
            }

            return rt;
        }
    }


    [ContextMenu("Toggle Tab")]
    public void ToggleTab()
    {

        if (!gameObject.activeSelf) //OPEN
        {
            OpenTab();

        }
        else //CLOSE
        {
            CloseTab();
        }

    }

    public void OpenTab()
    {

        if (TabRect == null)
            return;

        //Disable every child and tween to the starting width or height 

        if (tweenHeight)
        {
            DOTween.To(ApplyTween, 0, startHeight, tweenDuration).SetEase(easeType).OnComplete(() => SetChildrenActivate(true));
        }
        if (tweenWidth)
        {
            DOTween.To(ApplyTween, 0, startWidth, tweenDuration).SetEase(easeType).OnComplete(() => SetChildrenActivate(true));
        }

        SetChildrenActivate(false);
        gameObject.SetActive(true);
    }

    public void CloseTab()
    {

        if (TabRect == null)
            return;


        if (tweenHeight)
        {
            DOTween.To(ApplyTween, startHeight, 0, tweenDuration).SetEase(easeType).OnComplete(() => gameObject.SetActive(false));
        }
        if (tweenWidth)
        {
            DOTween.To(ApplyTween, startWidth, 0, tweenDuration).SetEase(easeType).OnComplete(() => gameObject.SetActive(false));
        }

        SetChildrenActivate(false);
    }

    private void SetChildrenActivate(bool state)
    {
        if (!hideChildrensWhileTween)
            return;

        bool catchInactives = state; // It'll only catch inactives if the state is set to True. I.e. we're opening it.
                                     // If not, then we don't need to disable whats already disabled

        if (gameObject.TryGetComponentInChildrens(out Transform[] childs, catchInactives))
        {

            foreach (Transform t in childs)
            {

                if (gameObject.transform == t)
                    continue;

                bool canOpen = true;

                foreach (Transform te in openException)
                {

                    if (te == t)
                    {

                        canOpen = false;
                        Debug.Log($"Will not {(state ? "open" : "close")}: {te.name}");
                        break;
                    }
                }


                if (canOpen)
                    t.gameObject.SetActive(state);
            }
        }
    }

    private void ApplyTween(float size)
    {
        if (TabRect == null)
            return;

        if (tweenHeight)
        {
            TabRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
        if (tweenWidth)
        {
            TabRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        }

        TabRect.ForceUpdateRectTransforms();


        if (rectTransformToUpdate != null)
            rectTransformToUpdate.ForceUpdateRectTransforms();
    }
}
