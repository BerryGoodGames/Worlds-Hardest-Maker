using DG.Tweening;
using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
///     Handles animation state for preview visibility.
///     Uses animator to show/hide preview based on visibility rules.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PreviewSpriteComponent))]
public class PreviewAnimationComponent : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer spriteRenderer;
    [SerializeField] [InitializationField] [MustBeAssigned] private PreviewSpriteComponent spriteComponent;
    [Separator] [SerializeField] [PositiveValueOnly] private float fadeOutDuration;
    [SerializeField] [PositiveValueOnly] private float blinkingDuration;
    [Space] [SerializeField] [PositiveValueOnly] private float brightBlinkMultiplier = 1.1f;
    [SerializeField] [PositiveValueOnly] private float darkBlinkMultiplier = 0.9f;
    
    private PreviewVisibilityRulesService visibilityService;
    private PreviewSpriteAlphaProvider alphaProvider;
    
    private Tween fadeTween;
    
    private bool isCurrentlyVisible;

    [Inject]
    private void Construct(PreviewVisibilityRulesService visibilityService, PreviewSpriteAlphaProvider alphaProvider)
    {
        this.visibilityService = visibilityService;
        this.alphaProvider = alphaProvider;
    }

    private void Update()
    {
        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        bool isVisible = visibilityService.IsPreviewVisible(currentEditMode);
        
        if (isVisible != isCurrentlyVisible)
        {
            SetTween(isVisible, currentEditMode);
        }
        
        isCurrentlyVisible = isVisible;
    }
    
    private void SetTween(bool isVisible, EditMode editMode)
    {
        fadeTween?.Kill();
        if (!isVisible)
        {
            fadeTween = spriteRenderer.DOFade(0, fadeOutDuration)
                .SetId(gameObject)
                .SetUpdate(true);
        }
        else
        {
            float alpha = alphaProvider.GetPreviewAlpha(editMode) * spriteComponent.Alpha / 255f;
            float brightAlpha = brightBlinkMultiplier * alpha;
            float darkAlpha = darkBlinkMultiplier * alpha;
            
            Color blinkBeginColor = spriteRenderer.color;
            blinkBeginColor.a = brightAlpha;
            spriteRenderer.color = blinkBeginColor;
            
            fadeTween = spriteRenderer.DOFade(darkAlpha, blinkingDuration / 2)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId(gameObject)
                .SetUpdate(true);
        }
    }
}