using UnityEngine;
using UnityEngine.EventSystems;

public class NumberInputArrowTweenController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool left;
    [SerializeField] private NumberInputTween tween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (left) tween.HoverEventArrowLeft(true);
        else tween.HoverEventArrowRight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (left) tween.HoverEventArrowLeft(false);
        else tween.HoverEventArrowRight(false);
    }
}