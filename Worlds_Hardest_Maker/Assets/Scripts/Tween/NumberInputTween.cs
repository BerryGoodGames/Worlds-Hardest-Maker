using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class NumberInputTween : MonoBehaviour
{
    [SerializeField] private RectTransform leftArrow;
    [SerializeField] private RectTransform rightArrow;
    private float leftArrowX;
    private float rightArrowX;
    [Space]
    [SerializeField] private float wiggleDuration;
    [SerializeField] private float wiggle;
    [SerializeField] private float hoverDuration;
    [SerializeField] private float hover;
    private Vector2 hoveredScl;
    private Vector2 unhoveredScl;

    private readonly Ease wiggleStartEase = Ease.OutCubic;
    private readonly Ease wiggleReturnEase = Ease.InOutSine;


    public void IncreaseTween()
    {
        rightArrow.DOKill();
        
        Sequence seq = DOTween.Sequence();
        seq.Append(rightArrow.DOMoveX(rightArrowX + wiggle, wiggleDuration * 0.5f).SetEase(wiggleStartEase))
            .Append(rightArrow.DOMoveX(rightArrowX, wiggleDuration * 0.5f).SetEase(wiggleReturnEase));
    }

    public void DecreaseTween()
    {
        leftArrow.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(leftArrow.DOMoveX(leftArrowX - wiggle, wiggleDuration * 0.5f).SetEase(wiggleStartEase))
            .Append(leftArrow.DOMoveX(leftArrowX, wiggleDuration * 0.5f).SetEase(wiggleReturnEase));
    }

    public void HoverEventArrow(RectTransform arrow, bool enter)
    {
        // boolean enter: did the mouse enter or leave the arrow
        arrow.DOKill();
        arrow.DOScale(enter ? hoveredScl : unhoveredScl, hoverDuration);
    }

    private void Start()
    {
        unhoveredScl = leftArrow.transform.localScale;
        hoveredScl = new(unhoveredScl.x + hover, unhoveredScl.y + hover);
        
        leftArrowX = leftArrow.position.x;
        rightArrowX = rightArrow.position.x;
    }
}
