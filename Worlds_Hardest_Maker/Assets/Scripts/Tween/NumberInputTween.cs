using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NumberInputTween : MonoBehaviour
{
    [SerializeField] private RectTransform leftArrow;
    [SerializeField] private RectTransform rightArrow;
    private float leftArrowX;
    private float rightArrowX;
    [Space]
    [SerializeField] private float duration;
    [SerializeField] private float wiggle;
    private readonly Ease wiggleStartEase = Ease.OutCubic;
    private readonly Ease wiggleReturnEase = Ease.InOutSine;


    public void IncreaseTween()
    {
        rightArrow.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(rightArrow.DOMoveX(rightArrowX + wiggle, duration * 0.5f).SetEase(wiggleStartEase))
            .Append(rightArrow.DOMoveX(rightArrowX, duration * 0.5f).SetEase(wiggleReturnEase));
    }

    public void DecreaseTween()
    {
        leftArrow.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(leftArrow.DOMoveX(leftArrowX - wiggle, duration * 0.5f).SetEase(wiggleStartEase))
            .Append(leftArrow.DOMoveX(leftArrowX, duration * 0.5f).SetEase(wiggleReturnEase));
    }

    private void Start()
    {
        leftArrowX = leftArrow.position.x;
        rightArrowX = rightArrow.position.x;
    }
}
