using DG.Tweening;
using MyBox;
using UnityEngine;

public class LockHighlightTween : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float scale = 3;
    [SerializeField] private int shakes = 5;
    [SerializeField] private int shakeRotation = 45;

    private Sequence scaleSequence;

    [ButtonMethod]
    public void Highlight()
    {
        // only play animation when not currently playing
        if (scaleSequence != null) return;

        // scale
        scaleSequence = DOTween.Sequence();

        scaleSequence
            // scale up
            .Append(transform.DOScale(Vector3.one * scale, duration / 2).SetEase(Ease.OutCubic))
            // scale back down
            .Append(transform.DOScale(Vector3.one, duration / 2).SetEase(Ease.InOutSine));

        // shake
        Sequence shakeSequence = DOTween.Sequence();
        float singleShakeDuration = duration / shakes;
        float shakeRotationCopy = shakeRotation;

        shakeSequence
            // go into shake position
            .Append(transform.DORotate(new(0, 0, shakeRotationCopy), singleShakeDuration).SetEase(Ease.InOutSine));

        shakeRotationCopy *= -1;

        for (int i = 0; i < shakes - 1; i++)
        {
            shakeSequence
                // return to normal position
                .Append(transform.DORotate(Vector3.zero, singleShakeDuration / 2).SetEase(Ease.InSine))
                // go into shake position
                .Append(transform.DORotate(new(0, 0, shakeRotationCopy), singleShakeDuration / 2)
                    .SetEase(Ease.OutSine));

            // shake into other direction next time
            shakeRotationCopy *= -1;
        }

        shakeSequence
            // return to normal position
            .Append(transform.DORotate(Vector3.zero, singleShakeDuration).SetEase(Ease.InOutSine))
            .OnComplete(() => scaleSequence = null);
    }
}