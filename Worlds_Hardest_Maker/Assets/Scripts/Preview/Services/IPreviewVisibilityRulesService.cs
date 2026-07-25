/// <summary>
///     Service responsible for determining if a preview should be visible
///     based on current game state and user input.
/// </summary>
public interface IPreviewVisibilityRulesService
{
    /// <summary>
    ///     Checks if preview should currently be visible.
    /// </summary>
    bool IsPreviewVisible(EditMode editMode);
}
