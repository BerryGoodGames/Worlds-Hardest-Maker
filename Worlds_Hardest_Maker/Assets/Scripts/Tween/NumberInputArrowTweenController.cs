using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumberInputArrowTweenController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private NumberInputTween tween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tween.HoverEventArrow(rt, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tween.HoverEventArrow(rt, false);
    }
}
