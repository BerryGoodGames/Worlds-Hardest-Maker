/// <summary>
///     Implementation of preview visibility rules.
///     Determines when previews should be shown based on game state, input, and managers.
/// </summary>
public class PreviewVisibilityRulesService : IPreviewVisibilityRulesService
{
    public bool IsPreviewVisible(EditMode editMode)
    {
        // hidden when UI is hovered
        if (MouseManager.Instance.IsUIHovered) return false;

        // hidden when using hotkeys for moving/modifying/deleting
        if (KeyBinds.GetKeyBind("Editor_MoveEntity")) return false;
        if (KeyBinds.GetKeyBind("Editor_Modify")) return false;
        if (KeyBinds.GetKeyBind("Editor_DeleteEntity")) return false;

        // hidden during anchor block operations
        if (AnchorBlockManager.Instance.DraggingBlock) return false;

        // hidden during copy/paste operations
        if (CopyManager.Instance.Pasting) return false;

        // hidden during anchor position editing
        if (AnchorPositionInputEditManager.Instance.IsEditing) return false;

        // during fill selection, check if mode supports fill preview
        if (SelectionManager.Instance.Selecting)
        {
            if (!editMode.ShowFillPreview) return false;
        }

        // mode-specific visibility checks
        if (editMode == EditModeManager.Coin)
        {
            return CoinManager.Instance.CanPlace(GetCurrentMousePosition(editMode));
        }
        
        if (editMode.Attributes.IsKey)
        {
            return KeyManager.Instance.CanPlace(GetCurrentMousePosition(editMode));
        }

        return true;
    }

    private UnityEngine.Vector2 GetCurrentMousePosition(EditMode editMode)
    {
        return editMode.WorldPositionType switch
        {
            WorldPositionType.Any => MouseManager.Instance.MouseWorldPos,
            WorldPositionType.Grid => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix,
        };
    }
}
