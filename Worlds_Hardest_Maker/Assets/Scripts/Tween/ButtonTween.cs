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

    public bool isWarningButton;
    [SerializeField] private float singleShakeDuration;
    [SerializeField] private float shake1;
    [SerializeField] private float shake2;

    private RectTransform contentRT;

    private bool hovered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;

        // elevate
        contentRT.DOAnchorPos(new(-highlightElevation, highlightElevation + highlightFloating), highlightElevateDuration);

        if (isWarningButton)
        {
            // add shake
            Sequence shakeSeq = DOTween.Sequence();
            shakeSeq.Append(content.DORotate(new(0, 0, shake1), singleShakeDuration))
                .Append(content.DORotate(new(0, 0, -shake2), singleShakeDuration))
                .Append(content.DORotate(new(0, 0, 0), singleShakeDuration));
        }

        // loop floating
        contentRT.DOAnchorPos(new(-highlightElevation, highlightElevation), highlightFloatingDuration * 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(highlightElevateDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;

        // idle anim move to original position
        contentRT.DOKill();
        contentRT.DOAnchorPos(Vector2.zero, highlightElevateDuration);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // click (& hold) animation
        contentRT.DOKill();
        contentRT.DOAnchorPos(((RectTransform)backgroundPanel).anchoredPosition, clickDuration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // release -> hover state if hovered, original state if not
        contentRT.DOKill();

        if (hovered) OnPointerEnter(null);
        else OnPointerExit(null);
    }

    private void Start()
    {
        contentRT = (RectTransform)content;
    }
}
