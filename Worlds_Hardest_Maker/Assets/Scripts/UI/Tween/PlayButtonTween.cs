using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayButtonTween : MonoBehaviour
{
    [SerializeField] private float playingY;
    [SerializeField] private float editingY;
    [Space]
    [SerializeField] private float duration;
    [SerializeField] private Ease easeAppear;
    [SerializeField] private AnimationCurve easeDisappear;

    private bool playing = false;

    public void SetPlay(bool play)
    {
        RectTransform rt = (RectTransform)transform;

        if (playing && !play)
        {
            // the frame unplayed
            rt.DOAnchorPosY(editingY, duration)
                .SetEase(easeAppear);
        }

        if (!playing && play)
        {
            // the frame played
            rt.DOAnchorPosY(playingY, duration)
                .SetEase(easeDisappear);
        }

        playing = play;
    }
}
