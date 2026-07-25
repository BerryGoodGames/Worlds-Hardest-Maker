using UnityEngine;

/// <summary>
///     Implementation of edit mode preview appearance provider.
///     Determines sprite, color, and scale for preview display based on edit mode.
/// </summary>
public class EditModePreviewProvider : IEditModePreviewProvider
{
    public PreviewData GetPreviewData(EditMode editMode, float alpha = 1f, bool forceShowPreviewSprite = false)
    {
        // Delete mode uses default sprite
        if (editMode == EditModeManager.Delete)
        {
            return PreviewData.Default;
        }

        GameObject currentPrefab = editMode.Prefab;
        if (currentPrefab == null)
        {
            return PreviewData.Default;
        }

        // Check for PreviewSprite component on prefab
        if (currentPrefab.TryGetComponent(out PreviewSprite previewSprite))
        {
            bool shouldShowPreviewSprite = forceShowPreviewSprite || 
                (!SelectionManager.Instance.Selecting && !CopyManager.Instance.Pasting);
            
            if (shouldShowPreviewSprite)
            {
                Color color = new Color(
                    previewSprite.Color.r,
                    previewSprite.Color.g,
                    previewSprite.Color.b,
                    alpha * previewSprite.Color.a
                );
                
                return new PreviewData
                {
                    Sprite = previewSprite.Sprite,
                    Color = color,
                    Scale = previewSprite.Scale,
                    ShouldRotate = editMode.IsRotatable,
                };
            }
        }

        // Fallback: Extract sprite from prefab or its children
        (SpriteRenderer spriteRenderer, Vector2 scale) = GetSpriteRendererAndScale(currentPrefab);
        
        if (spriteRenderer == null)
        {
            return PreviewData.Default;
        }

        Color prefabColor = spriteRenderer.color;
        Color finalColor = new Color(
            prefabColor.r,
            prefabColor.g,
            prefabColor.b,
            alpha * prefabColor.a
        );
        
        return new PreviewData
        {
            Sprite = spriteRenderer.sprite,
            Color = finalColor,
            Scale = scale,
            ShouldRotate = editMode.IsRotatable,
        };
    }

    private (SpriteRenderer renderer, Vector2 scale) GetSpriteRendererAndScale(GameObject prefab)
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
