using System;
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
    [SerializeField] [DefinedValues(-1, 1)] private int highlightXDirection = -1;
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
        
        int floatXDirection = Math.Sign(highlightXDirection);
        
        // elevate
        contentRT.DOAnchorPos(
                new(floatXDirection * highlightElevation, highlightElevation + highlightFloating),
                highlightElevateDuration
            )
            .SetId(gameObject);
        
        if (IsWarningButton)
        {
            // add shake
            Sequence shakeSeq = DOTween.Sequence();
            shakeSeq.Append(content.DORotate(Vector3.forward * shake1, singleShakeDuration))
                .Append(content.DORotate(Vector3.back * shake2, singleShakeDuration))
                .Append(content.DORotate(Vector3.zero, singleShakeDuration))
                .SetId(gameObject);
        }
        
        // loop floating
        contentRT.DOAnchorPos(new(floatXDirection * highlightElevation, highlightElevation), highlightFloatingDuration * 0.5f)
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