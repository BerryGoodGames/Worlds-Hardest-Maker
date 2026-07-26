using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PreviewSpriteComponent : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer spriteRenderer;
    [Space] [SerializeField] [MustBeAssigned] [InitializationField] private Sprite deleteSprite;
    [field: Space] [field: SerializeField] [field: Range(0, 255)] public float Alpha { get; private set; } = 115;
    
    public bool ShowSpriteWhenPasting { get; set; }
    
    private PreviewSpriteDataProvider previewSpriteDataProvider;
    
    [Inject]
    private void Construct(PreviewSpriteDataProvider previewSpriteDataProvider)
    {
        this.previewSpriteDataProvider = previewSpriteDataProvider;
    }
    
    /// <summary>
    ///     Updates sprite of preview to the current edit mode
    /// </summary>
    public void UpdateSprite()
    {
        SetSprite(LevelSessionEditManager.Instance.CurrentEditMode);
    }
    
    public void SetSprite(EditMode editMode)
    {
        PreviewSpriteData previewSpriteData = previewSpriteDataProvider.GetPreviewSpriteData(editMode, forceShowPreviewSprite: ShowSpriteWhenPasting);
        
        ApplyPreviewData(previewSpriteData);
    }
    
    public void ApplyPreviewData(PreviewSpriteData previewSpriteData)
    {
        Sprite appliedSprite = previewSpriteData.IsDelete ? deleteSprite : previewSpriteData.Sprite;
        
        Color appliedColor = previewSpriteData.Color;
        appliedColor.a *= Alpha / 255f;
        
        spriteRenderer.sprite = appliedSprite;
        spriteRenderer.color = appliedColor;
        transform.localScale = previewSpriteData.Scale;
    }
}