using UnityEngine;

public class PreviewSpriteAlphaProvider
{
    public float GetPreviewAlpha(EditMode editMode, bool forceShowPreviewSprite = false)
    {
        float deleteAlpha = PreviewSpriteData.Delete.Color.a;
        GameObject currentPrefab = editMode.Prefab;
        
        if (editMode == EditModeManager.Delete || currentPrefab == null)
        {
            return deleteAlpha;
        }

        if (currentPrefab.TryGetComponent(out PreviewSpriteConfigurator previewSprite))
        {
            return previewSprite.Color.a;
        }

        SpriteRenderer spriteRenderer = FindSpriteRenderer(currentPrefab);
        
        if (spriteRenderer == null)
        {
            return deleteAlpha;
        }
        
        return spriteRenderer.color.a;
    }

    private static SpriteRenderer FindSpriteRenderer(GameObject prefab)
    {
        if (prefab.TryGetComponent(out SpriteRenderer renderer)) return renderer;

        foreach (Transform child in prefab.transform)
        {
            if (child.TryGetComponent(out SpriteRenderer childRenderer))
                return childRenderer;
        }

        // not found
        return null;
    }
}