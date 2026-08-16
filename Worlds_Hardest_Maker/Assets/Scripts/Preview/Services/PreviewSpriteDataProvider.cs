using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

/// <summary>
///     Implementation of edit mode preview appearance provider.
///     Determines sprite, color, and scale for preview display based on edit mode.
/// </summary>
public class PreviewSpriteDataProvider
{
    [Inject] private ISelectionStateService selectionStateService;

    public PreviewSpriteData GetPreviewSpriteData(EditMode editMode, bool forceShowPreviewSprite = false)
    {
        // Delete mode uses default sprite
        if (editMode == EditModeManager.Delete)
        {
            return PreviewSpriteData.Delete;
        }

        GameObject currentPrefab = editMode.Prefab;
        if (currentPrefab == null)
        {
            return PreviewSpriteData.Delete;
        }

        // Check for PreviewSprite component on prefab
        if (currentPrefab.TryGetComponent(out PreviewSpriteConfigurator previewSprite))
        {
            bool shouldShowPreviewSprite = forceShowPreviewSprite || 
                (!selectionStateService.IsSelecting && !CopyManager.Instance.Pasting);
            
            if (shouldShowPreviewSprite)
            {
                return new PreviewSpriteData
                {
                    Sprite = previewSprite.Sprite,
                    Color = previewSprite.Color,
                    Scale = previewSprite.Scale,
                };
            }
        }

        // Fallback: Extract sprite from prefab or its children
        (SpriteRenderer spriteRenderer, Vector2 scale) = FindSpriteRendererAndScale(currentPrefab);
        
        if (spriteRenderer == null)
        {
            return PreviewSpriteData.Delete;
        }

        Color prefabColor = spriteRenderer.color;
        Color finalColor = new Color(
            prefabColor.r,
            prefabColor.g,
            prefabColor.b,
            prefabColor.a
        );
        
        return new PreviewSpriteData
        {
            Sprite = spriteRenderer.sprite,
            Color = finalColor,
            Scale = scale,
        };
    }

    private static (SpriteRenderer renderer, Vector2 scale) FindSpriteRendererAndScale(GameObject prefab)
    {
        // Try to get sprite renderer directly on prefab
        if (prefab.TryGetComponent(out SpriteRenderer renderer))
            return (renderer, prefab.transform.localScale);

        // Search children
        foreach (Transform child in prefab.transform)
        {
            if (child.TryGetComponent(out SpriteRenderer childRenderer))
                return (childRenderer, child.localScale);
        }

        // Not found
        return (null, Vector2.one);
    }
}
