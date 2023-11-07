using DG.Tweening;
using UnityEngine;

public class NumberInputTween : MonoBehaviour
{
    [SerializeField] private RectTransform leftArrow;
    [SerializeField] private RectTransform rightArrow;
    [Space] [SerializeField] private float wiggleDuration;
    [SerializeField] private float wiggle;
    [SerializeField] private float hoverDuration;
    [SerializeField] private float hover;
    private float leftArrowX;
    private float rightArrowX;
    private readonly Vector2 unhoveredScl = Vector2.one;
    private Vector2 hoveredScl;

    private const Ease WiggleStartEase = Ease.OutCubic;
    private const Ease WiggleReturnEase = Ease.InOutSine;

    private Sequence wiggleSeqLeft;
    private Sequence wiggleSeqRight;

    public void IncreaseTween()
    {
        rightArrow.DOKill();

        wiggleSeqRight = DOTween.Sequence();
        wiggleSeqRight.Append(
                rightArrow.DOLocalMoveX(rightArrowX + wiggle, wiggleDuration * 0.5f)
                    .SetEase(WiggleStartEase)
            )
            .Append(rightArrow.DOLocalMoveX(rightArrowX, wiggleDuration * 0.5f).SetEase(WiggleReturnEase));
    }

    public void DecreaseTween()
    {
        leftArrow.DOKill();

        wiggleSeqLeft = DOTween.Sequence();
        wiggleSeqLeft
            .Append(leftArrow.DOLocalMoveX(leftArrowX - wiggle, wiggleDuration * 0.5f).SetEase(WiggleStartEase))
            .Append(leftArrow.DOLocalMoveX(leftArrowX, wiggleDuration * 0.5f).SetEase(WiggleReturnEase));
    }

    public void HoverEventArrowLeft(bool enter) =>
        // boolean enter: did the mouse enter or leave the arrow
        leftArrow.DOScale(enter ? hoveredScl : unhoveredScl, hoverDuration);

    public void HoverEventArrowRight(bool enter) =>
        // boolean enter: did the mouse enter or leave the arrow
        rightArrow.DOScale(enter ? hoveredScl : unhoveredScl, hoverDuration);


    private void Start()
    {
        hoveredScl = Vector2.one * hover + unhoveredScl;

        leftArrowX = leftArrow.localPosition.x;
        rightArrowX = rightArrow.localPosition.x;

        wiggleSeqRight = DOTween.Sequence();
        wiggleSeqLeft = DOTween.Sequence();
    }
}