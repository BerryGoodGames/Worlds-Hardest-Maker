using UnityEngine;
using VContainer;
using WorldsHardestMaker.CopyPaste;
using WorldsHardestMaker.Selection;

/// <summary>
///     Implementation of preview visibility rules.
///     Determines when previews should be shown based on game state, input, and managers.
/// </summary>
public class PreviewVisibilityRulesService
{
    [Inject] private IMouseService mouseService;
    [Inject] private ISelectionStateService selectionStateService;
    [Inject] private ICopyPasteService copyPasteService;
    
    public bool IsPreviewVisible(EditMode editMode)
    {
        // hidden when UI is hovered
        if (mouseService.IsUIHovered) return false;

        // hidden when using hotkeys for moving/modifying/deleting
        if (KeyBinds.GetKeyBind("Editor_MoveEntity")) return false;
        if (KeyBinds.GetKeyBind("Editor_Modify")) return false;
        if (KeyBinds.GetKeyBind("Editor_DeleteEntity")) return false;

        // hidden during anchor block operations
        if (AnchorBlockManager.Instance.DraggingBlock) return false;

        // hidden during copy/paste operations
        if (copyPasteService.IsPasting) return false;

        // hidden during anchor position editing
        if (AnchorPositionInputEditManager.Instance.IsEditing) return false;

        // during fill selection, check if mode supports fill preview
        if (selectionStateService.IsSelecting)
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
            WorldPositionType.Any => mouseService.MouseWorldPos,
            WorldPositionType.Grid => mouseService.MouseWorldPosGrid,
            _ => mouseService.MouseWorldPosMatrix,
        };
    }
}
