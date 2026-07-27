using DG.Tweening;
using MyBox;
using UnityEngine;

public class ToastAnimation : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private CanvasGroup canvasGroup;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform container;
    
    [Separator] [SerializeField] [PositiveValueOnly] private float swipeDuration = 0.4f;
    [SerializeField] [PositiveValueOnly] private float fadeInDuration = 0.4f;
    [SerializeField] [PositiveValueOnly] private float fadeOutDuration = 0.4f;
    
    private void Start() { EnterAnimation(); }
    
    private void EnterAnimation()
    {
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, fadeInDuration)
            .SetId(gameObject)
            .SetUpdate(true);
        
        float width = (transform as RectTransform)!.sizeDelta.x;
        
        Vector2 startPosition = container.anchoredPosition;
        startPosition.x -= width / 2;
        Vector2 targetPosition = container.anchoredPosition;
        
        container.anchoredPosition = startPosition;
        container.DOAnchorPos(targetPosition, swipeDuration)
            .SetEase(Ease.OutQuart)
            .SetId(gameObject);
    }
    
    public Tween Death()
    {
        return canvasGroup.DOFade(0, fadeOutDuration)
            .SetId(gameObject);
    }
    
    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}