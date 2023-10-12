using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class LevelCardTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] [InitializationField] [MustBeAssigned]
    private RectTransform card;

    [Separator] [SerializeField] [PositiveValueOnly]
    private float hoverScale;

    [SerializeField] [PositiveValueOnly] private float startEditScale;

    [SerializeField] [PositiveValueOnly] private float hoverDuration;
    [SerializeField] [PositiveValueOnly] private float startEditingDuration;

    private bool checkHoverDetection = true;

    public void OnHover() => card.DOScale(hoverScale, hoverDuration).SetEase(Ease.InOutSine).SetId(gameObject);

    public void OnUnhover()
    {
        if (checkHoverDetection)
        {
            card.DOScale(1, hoverDuration).SetEase(Ease.InOutSine).SetId(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (checkHoverDetection) OnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (checkHoverDetection) OnUnhover();
    }

    public void OnStartEditing()
    {
        card.DOScale(startEditScale, startEditingDuration).SetEase(Ease.OutCubic).SetId(gameObject);

        // disable checking for hovering completely
        checkHoverDetection = false;
    }

    private void OnDestroy() => DOTween.Kill(gameObject);
}