using System.Collections;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.CopyPaste;
using WorldsHardestMaker.Selection;

/// <summary>
///     Controls mouse events: placing, filling, deleting
///     <para>Attach to game manager</para>
/// </summary>
public class MouseEventManager : MonoBehaviour
{
    private const float SELECTION_CANCEL_MAX_TIME = 0.15f;
    
    private bool isFullyFocused = true;
    
    [Inject] private EventBus eventBus;
    [Inject] private IMouseService mouseService;
    [Inject] private ISelectionStateService selectionStateService;
    [Inject] private ICopyPasteService copyPasteService;
    
    private void Update()
    {
        // selection
        if (KeyBinds.GetKeyBindDown("Editor_Select")) StartCoroutine(StartCancelSelection());
        
        CheckPlaceAndDelete();
        
        // track drag positions
        if (!Input.GetMouseButtonUp(0)) return;
        
        mouseService.DragStart = null;
        mouseService.DragCurrent = null;
        mouseService.MouseDragEnd = null;
        
        eventBus.Fire(new EditActionEvent());
    }
    
    
    private void CheckPlaceAndDelete()
    {
        EditMode editMode = LevelSessionEditManager.Instance.CurrentEditMode;
        
        // place / delete stuff
        if (mouseService.IsUIHovered
            || LevelSessionEditManager.Instance.Playing
            || selectionStateService.IsSelecting
            || copyPasteService.IsPasting
            || AnchorPositionInputEditManager.Instance.IsEditing) return;
        
        // if none of the relevant keys is held, check field placement + entity placement
        if (!KeyBinds.GetKeyBind("Editor_MoveEntity")
            && !KeyBinds.GetKeyBind("Editor_Modify")
            && !KeyBinds.GetKeyBind("Editor_DeleteEntity")
            && !selectionStateService.IsSelecting)
        {
            if (Input.GetMouseButton(0)) CheckDragPlacement(editMode);
            if (Input.GetMouseButtonDown(0)) CheckClickPlacement(editMode);
        }
        
        CheckEntityDelete();
    }
    
    private IEnumerator StartCancelSelection()
    {
        float passedTime = 0;
        while (KeyBinds.GetKeyBind("Editor_Select"))
        {
            if (passedTime > SELECTION_CANCEL_MAX_TIME || mouseService.MousePosDelta.magnitude > 10) yield break;
            passedTime += Time.deltaTime;
            yield return null;
        }
        
        selectionStateService.ClearSelection();
    }
    
    private void CheckClickPlacement(EditMode editMode)
    {
        if (editMode.IsDraggable) return;
        
        PlaceManager.Instance.Place(editMode, mouseService.MouseWorldPos, LevelSessionEditManager.Instance.EditRotation, true);
    }
    
    private void CheckDragPlacement(EditMode editMode)
    {
        // check placement
        if (!editMode.IsDraggable) return;
        
        if (!isFullyFocused) return;
        
        if (Vector2.SqrMagnitude(mouseService.MouseWorldPos - mouseService.PrevMouseWorldPos) > 2)
        {
            PlaceManager.Instance.PlacePath(
                editMode,
                mouseService.PrevMouseWorldPos, mouseService.MouseWorldPos,
                LevelSessionEditManager.Instance.EditRotation, true
            );
        }
        else
        {
            PlaceManager.Instance.Place(
                editMode, mouseService.MouseWorldPos,
                LevelSessionEditManager.Instance.EditRotation, true
            );
        }
    }
    
    private void CheckEntityDelete()
    {
        if (!KeyBinds.GetKeyBind("Editor_DeleteEntity")) return;
        
        if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) return;
        
        // delete entities
        PlaceManager.RemoveEntitiesAt(
            mouseService.MouseWorldPosGrid,
            LayerManager.Instance.Layers.Entity
        );
    }
    
    
    private void OnApplicationFocus(bool hasFocus)
    {
        StartCoroutine(Assign());
        
        return;
        
        IEnumerator Assign()
        {
            yield return new WaitForEndOfFrame();
            isFullyFocused = hasFocus;
        }
    }
}