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
    [SerializeField] private float playingY;
    [SerializeField] private float editingY;
    [Space]
    [SerializeField] private float appearDuration;
    [SerializeField] private float disappearDuration;
    [Space]
    [SerializeField] private Ease easeAppear;
    [SerializeField] private Ease easeDisappear;
    [SerializeField] private AnimationCurve easeAppearCurve;
    [SerializeField] private AnimationCurve easeDisappearCurve;

    private bool playing = false;

    public void SetPlay(bool play)
    {
        RectTransform rt = (RectTransform)transform;
        
        if (playing && !play)
        {
            // the frame unplayed
            Tween t = rt.DOAnchorPosY(editingY, appearDuration);
            if (easeAppearCurve.length > 1) t.SetEase(easeAppearCurve);
            else t.SetEase(easeAppear);
        }

        if (!playing && play)
        {
            // the frame played
            Tween t = rt.DOAnchorPosY(playingY, disappearDuration);
            if (easeDisappearCurve.length > 1) t.SetEase(easeDisappearCurve);
            else t.SetEase(easeDisappear);
        }

        playing = play;
    }
}
