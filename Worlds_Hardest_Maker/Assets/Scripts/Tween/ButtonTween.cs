using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform backgroundPanel;
    [SerializeField] private float clickDuration;
    [SerializeField] private float highlightElevation;
    [SerializeField] private float highlightFloating;
    [SerializeField] private float highlightElevateDuration;
    [SerializeField] private float highlightFloatingDuration;

    private bool hovered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;

        // elevate
        content.DOLocalMove(new(-highlightElevation, highlightElevation + highlightFloating), highlightElevateDuration);

        // loop floating
        content.DOLocalMove(new(-highlightElevation, highlightElevation), highlightFloatingDuration * 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(highlightElevateDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;

        // idle anim move to original position
        content.DOKill();
        content.DOLocalMove(Vector2.zero, highlightElevateDuration);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // click (& hold) animation
        content.DOKill();
        content.DOLocalMove(backgroundPanel.localPosition, clickDuration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // release -> hover state if hovered, original state if not
        content.DOKill();

        if (hovered) OnPointerEnter(null);
        else OnPointerExit(null);
    }
}
