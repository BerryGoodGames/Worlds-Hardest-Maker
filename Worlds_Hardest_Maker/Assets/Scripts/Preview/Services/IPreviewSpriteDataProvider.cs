/// <summary>
///     Service responsible for providing preview sprite, color, and scale
///     based on the current edit mode.
/// </summary>
public interface IPreviewSpriteDataProvider
{
    /// <summary>
    ///     Gets the preview appearance for a given edit mode.
    /// </summary>
    /// <param name="editMode">The edit mode to get preview for</param>
    /// <param name="alpha">Alpha value (0-1 range) for color transparency</param>
    /// <param name="forceShowPreviewSprite">If true, always show custom preview sprite if available (ignores pasting state)</param>
    /// <returns>PreviewData containing the calculated data for the preview</returns>
    PreviewSpriteData GetPreviewSpriteData(EditMode editMode, float alpha = 1f, bool forceShowPreviewSprite = false);
}
