using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelCardTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform levelCard;

    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform card;

    [SerializeField] [InitializationField] [MustBeAssigned] private ContentSizeFitter extendedArea;

    [Separator] [SerializeField] [PositiveValueOnly] private float hoverScale;

    [SerializeField] [PositiveValueOnly] private float startEditScale;

    [SerializeField] [PositiveValueOnly] private float hoverDuration;
    [SerializeField] [PositiveValueOnly] private float startEditingDuration;

    [Separator("Expand settings")] private float expandHeight;


    private float collapsedHeight;

    [SerializeField] [PositiveValueOnly] private float bottomPadding;
    [SerializeField] [PositiveValueOnly] private float expandDuration;

    public bool IsExpanded { get; private set; }

    private bool checkHoverDetection = true;

    public void OnHover() => card.DOScale(hoverScale, hoverDuration).SetEase(Ease.InOutSine).SetId(gameObject);

    public void OnUnhover() => card.DOScale(1, hoverDuration).SetEase(Ease.InOutSine).SetId(gameObject);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (checkHoverDetection) OnHover();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (checkHoverDetection) OnUnhover();
    }

    public void OnOpenLevel()
    {
        card.DOScale(startEditScale, startEditingDuration).SetEase(Ease.OutCubic).SetId(gameObject);

        // disable checking for hovering completely
        checkHoverDetection = false;
    }

    public void Expand()
    {
        levelCard.DOSizeDelta(new(levelCard.rect.width, expandHeight), expandDuration)
            .onUpdate += () => { LevelListLoader.Instance.LevelCardContentSizeFitter.Recalculate(); };

        float deltaHeight = expandHeight - collapsedHeight;
        float deltaY = deltaHeight * (hoverScale - 1) / 2;

        card.DOAnchorPosY(-deltaY, expandDuration).SetRelative(true);

        IsExpanded = true;
    }

    public void Collapse()
    {
        levelCard.DOSizeDelta(new(levelCard.rect.width, collapsedHeight), expandDuration)
            .onUpdate += () => { LevelListLoader.Instance.LevelCardContentSizeFitter.Recalculate(); };
        
        float deltaHeight = expandHeight - collapsedHeight;
        float deltaY = deltaHeight * (hoverScale - 1) / 2;

        card.DOAnchorPosY(deltaY, expandDuration).SetRelative(true);
        
        IsExpanded = false;
    }

    public void ToggleExpandCollapse()
    {
        if (!IsExpanded) Expand();
        else Collapse();
    }

    private void OnDestroy() => DOTween.Kill(gameObject);

    private void Start()
    {
        // remember initial values
        collapsedHeight = levelCard.rect.height;

        extendedArea.Recalculate();

        expandHeight = collapsedHeight + ((RectTransform)extendedArea.transform).sizeDelta.y + bottomPadding;
    }
}