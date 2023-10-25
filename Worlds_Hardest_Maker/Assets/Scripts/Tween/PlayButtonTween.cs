using System;
using DG.Tweening;
using UnityEngine;

public class PlayButtonTween : MonoBehaviour
{
    [SerializeField] private float playingY;
    [SerializeField] private float editingY;
    [Space] [SerializeField] private float duration;
    [SerializeField] private Ease easeAppear;
    [SerializeField] private AnimationCurve easeDisappear;

    private bool playing;

    public void SetPlay(bool play)
    {
        RectTransform rt = (RectTransform)transform;

        if (playing && !play)
            // the frame unplayed
        {
            rt.DOAnchorPosY(editingY, duration)
                .SetEase(easeAppear)
                .SetId(gameObject);
        }

        if (!playing && play)
            // the frame played
        {
            rt.DOAnchorPosY(playingY, duration)
                .SetEase(easeDisappear)
                .SetId(gameObject);
        }

        playing = play;
    }

    private void OnDestroy() => DOTween.Kill(gameObject);
}