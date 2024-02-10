using System;
using DG.Tweening;
using MyBox;
using UnityEngine;

public class MoveRelativeTween : ChainableTween
{
    [Separator] [SerializeField] private bool isRectTransform;

    [ConditionalField(nameof(isRectTransform))] [SerializeField] private bool animateAnchor;

    [ConditionalField(nameof(animateAnchor), true)] [SerializeField] private Vector2 movement;

    [ConditionalField(nameof(animateAnchor))] [SerializeField] private Vector2 anchorMin;

    [ConditionalField(nameof(animateAnchor))] [SerializeField] private Vector2 anchorMax;

    private Tween tween;

    public void Move()
    {
        if (tween != null && tween.IsPlaying()) return;

        if (isRectTransform)
        {
            if (animateAnchor)
            {
                ((RectTransform)transform).DOAnchorMin(anchorMin, Duration)
                    .SetRelative()
                    .SetEase(Ease.InOutSine)
                    .SetDelay(Delay)
                    .SetId(gameObject);

                tween = ((RectTransform)transform).DOAnchorMax(anchorMax, Duration)
                    .SetRelative()
                    .SetEase(Ease.InOutSine)
                    .SetDelay(Delay)
                    .SetId(gameObject);
            }
            else
            {
                tween = ((RectTransform)transform).DOAnchorPos(movement, Duration)
                    .SetRelative()
                    .SetEase(Ease.InOutSine)
                    .SetDelay(Delay)
                    .SetId(gameObject);
            }
        }
        else
        {
            tween = transform.DOMove(movement, Duration)
                .SetRelative()
                .SetEase(Ease.InOutSine)
                .SetDelay(Delay)
                .SetId(gameObject);
        }

        StartCoroutine(StartDelay());
    }

    public override void StartChain() => Move();

    private void OnDestroy() => DOTween.Kill(gameObject);
}