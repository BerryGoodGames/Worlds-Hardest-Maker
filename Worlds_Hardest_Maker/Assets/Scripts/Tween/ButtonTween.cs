using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform backgroundPanel;

    [SerializeField] private float clickDuration;
    [SerializeField] private float highlightElevation;
    [SerializeField] private float highlightFloating;
    [SerializeField] private float highlightElevateDuration;
    [SerializeField] private float highlightFloatingDuration;

    [Space] public bool IsWarningButton;

    [ConditionalField(nameof(IsWarningButton))] [SerializeField] private float singleShakeDuration;

    [ConditionalField(nameof(IsWarningButton))] [SerializeField] private float shake1;

    [ConditionalField(nameof(IsWarningButton))] [SerializeField] private float shake2;

    private RectTransform contentRT;

    private bool hovered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;

        // elevate
        contentRT.DOAnchorPos(
                new(-highlightElevation, highlightElevation + highlightFloating),
                highlightElevateDuration
            )
            .SetId(gameObject);

        if (IsWarningButton)
        {
            // add shake
            Sequence shakeSeq = DOTween.Sequence();
            shakeSeq.Append(content.DORotate(new(0, 0, shake1), singleShakeDuration))
                .Append(content.DORotate(new(0, 0, -shake2), singleShakeDuration))
                .Append(content.DORotate(new(0, 0, 0), singleShakeDuration))
                .SetId(gameObject);
        }

        // loop floating
        contentRT.DOAnchorPos(new(-highlightElevation, highlightElevation), highlightFloatingDuration * 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(highlightElevateDuration)
            .SetId(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;

        // idle anim move to original position
        contentRT.DOKill();
        contentRT.DOAnchorPos(Vector2.zero, highlightElevateDuration)
            .SetId(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // click (& hold) animation
        contentRT.DOKill();
        contentRT.DOAnchorPos(((RectTransform)backgroundPanel).anchoredPosition, clickDuration)
            .SetId(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // release -> hover state if hovered, original state if not
        contentRT.DOKill();

        if (hovered) OnPointerEnter(null);
        else OnPointerExit(null);
    }

    private void Start() => contentRT = (RectTransform)content;

    private void OnDestroy() => DOTween.Kill(gameObject);
}