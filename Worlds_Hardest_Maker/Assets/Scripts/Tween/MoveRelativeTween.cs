using DG.Tweening;
using MyBox;
using UnityEngine;

public class MoveRelativeTween : ChainableTween
{
    [Separator] [SerializeField] private bool isRectTransform;
    [SerializeField] private Vector2 movement;

    public Tween MoveTween { get; private set; }

    public void Move()
    {
        if (isRectTransform)
        {
            MoveTween = ((RectTransform)transform).DOAnchorPos(movement, Duration)
                .SetRelative()
                .SetEase(Ease.InOutSine)
                .SetDelay(Delay);
        }
        else
        {
            MoveTween = transform.DOMove(movement, Duration)
                .SetRelative()
                .SetEase(Ease.InOutSine)
                .SetDelay(Delay);
        }

        StartCoroutine(StartDelay());
    }

    public override void StartChain() => Move();
}
