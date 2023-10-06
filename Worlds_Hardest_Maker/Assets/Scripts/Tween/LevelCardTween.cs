using System;
using System.Collections;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelCardTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] [InitializationField] [MustBeAssigned]
    private RectTransform card;

    [Separator]
    [SerializeField][PositiveValueOnly] private float hoverScale; 
    [SerializeField] private float duration;

    public void OnHover()
    {
        card.DOScale(hoverScale, duration).SetEase(Ease.InOutSine).SetId(gameObject);
    }

    public void OnUnhover()
    {
        card.DOScale(1, duration).SetEase(Ease.InOutSine).SetId(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData) => OnHover();

    public void OnPointerExit(PointerEventData eventData) => OnUnhover();

    private void OnDestroy() => DOTween.Kill(gameObject);
}
