using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// General tweening script for UI at the top or bottom of the screen
/// Tweens UI Element offscreen when playing (playingY) and onscreen when editing (editingY) with SetPlay
/// </summary>
public class BarTween : MonoBehaviour
{
    [SerializeField] private float visibleY;
    [SerializeField] private float invisibleY;
    [SerializeField] private bool isVisibleOnlyOnEdit = true;
    [Space]
    [SerializeField] private float appearDuration;
    [SerializeField] private float disappearDuration;
    [Space]
    [SerializeField] private Ease easeAppear;
    [SerializeField] private Ease easeDisappear;
    [SerializeField] private AnimationCurve easeAppearCurve;
    [SerializeField] private AnimationCurve easeDisappearCurve;

    private bool playing = false;

    private RectTransform rt;

    public void SetPlay(bool play)
    {
        if (playing && !play)
        {
            // the frame unplayed -> editmode
            if (isVisibleOnlyOnEdit) TweenVis();
            else TweenInvis();
        }

        if (!playing && play)
        {
            // the frame played -> playmode
            if (!isVisibleOnlyOnEdit) TweenVis();
            else TweenInvis();
        }

        playing = play;
    }

    private void TweenInvis()
    {
        Tween t = rt.DOAnchorPosY(invisibleY, disappearDuration);
        if (easeDisappearCurve.length > 1) t.SetEase(easeDisappearCurve);
        else t.SetEase(easeDisappear);
    }

    private void TweenVis()
    {
        Tween t = rt.DOAnchorPosY(visibleY, appearDuration);
        if (easeAppearCurve.length > 1) t.SetEase(easeAppearCurve);
        else t.SetEase(easeAppear);
    }

    private void Start()
    {
        rt = (RectTransform)transform;

        if (GameManager.Instance.Playing) rt.anchoredPosition = new(rt.anchoredPosition.x, isVisibleOnlyOnEdit ? invisibleY : visibleY);
        else rt.anchoredPosition = new(rt.anchoredPosition.x, !isVisibleOnlyOnEdit ? invisibleY : visibleY);
    }
}