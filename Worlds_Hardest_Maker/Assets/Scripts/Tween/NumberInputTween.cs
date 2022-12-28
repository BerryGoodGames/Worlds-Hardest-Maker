using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NumberInputTween : MonoBehaviour
{
    [SerializeField] private RectTransform leftArrow;
    [SerializeField] private RectTransform rightArrow;
    [Space]
    [SerializeField] private float wiggleDuration;
    [SerializeField] private float wiggle;
    [SerializeField] private float hoverDuration;
    [SerializeField] private float hover;
    private float leftArrowX;
    private float rightArrowX;
    private Vector2 unhoveredScl = Vector2.one;
    private Vector2 hoveredScl;

    private const Ease wiggleStartEase = Ease.OutCubic;
    private const Ease wiggleReturnEase = Ease.InOutSine;

    private Sequence wiggleSeqLeft;
    private Sequence wiggleSeqRight;

    public void IncreaseTween()
    {
        rightArrow.DOKill();

        wiggleSeqRight = DOTween.Sequence();
        wiggleSeqRight.Append(rightArrow.DOMoveX(rightArrowX + wiggle, wiggleDuration * 0.5f).SetEase(wiggleStartEase))
            .Append(rightArrow.DOMoveX(rightArrowX, wiggleDuration * 0.5f).SetEase(wiggleReturnEase));
    }

    public void DecreaseTween()
    {
        leftArrow.DOKill();

        wiggleSeqLeft = DOTween.Sequence();
        wiggleSeqLeft.Append(leftArrow.DOMoveX(leftArrowX - wiggle, wiggleDuration * 0.5f).SetEase(wiggleStartEase))
            .Append(leftArrow.DOMoveX(leftArrowX, wiggleDuration * 0.5f).SetEase(wiggleReturnEase));
    }

    public void HoverEventArrowLeft(bool enter)
    {
        // boolean enter: did the mouse enter or leave the arrow
        leftArrow.DOScale(enter ? hoveredScl : unhoveredScl, hoverDuration);
    }

    public void HoverEventArrowRight(bool enter)
    {
        // boolean enter: did the mouse enter or leave the arrow
        rightArrow.DOScale(enter ? hoveredScl : unhoveredScl, hoverDuration);
    }



    private void Start()
    {
        hoveredScl = Vector2.one * hover + unhoveredScl;
        
        leftArrowX = leftArrow.position.x;
        rightArrowX = rightArrow.position.x;

        wiggleSeqRight = DOTween.Sequence();
        wiggleSeqLeft = DOTween.Sequence();
    }
}
