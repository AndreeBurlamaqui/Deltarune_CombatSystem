using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class OpenTabTween : MonoBehaviour
{

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


    [ContextMenu("Toggle Tab")]
    public void ToggleTab()
    {

        if (rt == null)
        {
            rt = GetComponent<RectTransform>();

            startHeight = rt.rect.height;
            startWidth = rt.rect.width;
        }

        if (!gameObject.activeSelf) //OPEN
        {
            OpenTab();

        }
        else //CLOSE
        {
            if (rt == null)
                return;

            SetChildrenActivate(false);

            if (tweenHeight)
            {
                DOTween.To(ApplyTween, startHeight, 0, tweenDuration).SetEase(easeType).OnComplete(() => gameObject.SetActive(false));
            }
            if (tweenWidth)
            {
                DOTween.To(ApplyTween, startWidth, 0, tweenDuration).SetEase(easeType).OnComplete(() => gameObject.SetActive(false));
            }

        }

    }

    public void OpenTab()
    {
        if (rt == null)
        {
            rt = GetComponent<RectTransform>();

            startHeight = rt.rect.height;
            startWidth = rt.rect.width;
        }

        if (rt == null)
            return;

        //Disable every child and tween to the starting width or height 

        SetChildrenActivate(false);
        gameObject.SetActive(true);

        if (tweenHeight)
        {
            DOTween.To(ApplyTween, 0, startHeight, tweenDuration).SetEase(easeType).OnComplete(() => SetChildrenActivate(true));
        }
        if (tweenWidth)
        {
            DOTween.To(ApplyTween, 0, startWidth, tweenDuration).SetEase(easeType).OnComplete(() => SetChildrenActivate(true));
        }
    }

    private void SetChildrenActivate(bool state)
    {
        if (hideChildrensWhileTween)
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
        if (rt == null)
            return;

        if (tweenHeight)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
        if (tweenWidth)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        }

        rt.ForceUpdateRectTransforms();
    }
}
