using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform content;
    [SerializeField] private float highlightElevation;
    [SerializeField] private float highlightFloating;
    [SerializeField] private float highlightElevateDuration;
    [SerializeField] private float highlightFloatingDuration;


    public void OnPointerEnter(PointerEventData eventData)
    {
        // elevate
        content.DOLocalMove(new(-highlightElevation, highlightElevation + highlightFloating), highlightElevateDuration);

        // loop floating
        content.DOLocalMove(new(-highlightElevation, highlightElevation), highlightFloatingDuration / 2)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(highlightElevateDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        content.DOKill();
        content.DOLocalMove(Vector2.zero, highlightElevateDuration);
    }
}
