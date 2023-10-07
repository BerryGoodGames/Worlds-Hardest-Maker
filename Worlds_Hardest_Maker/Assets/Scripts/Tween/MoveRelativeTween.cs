using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveRelativeTween : ChainableTween
{
    [Separator] [SerializeField] private bool isRectTransform;

    [ConditionalField(nameof(isRectTransform))]
    [SerializeField]
    private bool animateAnchor = false;

    [ConditionalField(nameof(animateAnchor), true)]
    [SerializeField] private Vector2 movement;
    
    [ConditionalField(nameof(animateAnchor))]
    [SerializeField] private Vector2 anchorMin;

    [ConditionalField(nameof(animateAnchor))]
    [SerializeField] private Vector2 anchorMax;

    public Tween MoveTween { get; private set; }

    public void Move()
    {
        if (isRectTransform)
        {
            if (animateAnchor)
            {
                ((RectTransform)transform).DOAnchorMin(anchorMin, Duration).SetRelative().SetEase(Ease.InOutSine).SetDelay(Delay);
                ((RectTransform)transform).DOAnchorMax(anchorMax, Duration).SetRelative().SetEase(Ease.InOutSine).SetDelay(Delay);
            }
            else
            {
                ((RectTransform)transform).DOAnchorPos(movement, Duration)
                                .SetRelative()
                                .SetEase(Ease.InOutSine)
                                .SetDelay(Delay);
            }

        }
        else
        {
            transform.DOMove(movement, Duration)
                .SetRelative()
                .SetEase(Ease.InOutSine)
                .SetDelay(Delay);
        }

        StartCoroutine(StartDelay());
    }

    public override void StartChain() => Move();
}
