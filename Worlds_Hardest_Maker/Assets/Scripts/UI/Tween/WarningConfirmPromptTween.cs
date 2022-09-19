using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WarningConfirmPromptTween : MonoBehaviour
{
    [SerializeField] private float activateDuration;
    [SerializeField] private float deactivateDuration;
    [Space]
    [SerializeField] private AnimationCurve easeScaleXActivate;
    [SerializeField] private AnimationCurve easeScaleYActivate;
    [SerializeField] private Ease easeDeactivate;

    private bool isVisible;

    public void SetVisible(bool vis)
    {
        if (isVisible && !vis)
        {
            // the frame not visible
            transform.DOScaleX(0, deactivateDuration).SetEase(easeDeactivate);
            transform.DOScaleY(0, deactivateDuration).SetEase(easeDeactivate);
        }

        if (!isVisible && vis)
        {
            // the frame visible
            transform.DOScaleX(1, activateDuration).SetEase(easeScaleXActivate);
            transform.DOScaleY(1, activateDuration).SetEase(easeScaleYActivate);
        }

        isVisible = vis;
    }

    private void Start()
    {
        transform.localScale = new(0, 0);
    }
}