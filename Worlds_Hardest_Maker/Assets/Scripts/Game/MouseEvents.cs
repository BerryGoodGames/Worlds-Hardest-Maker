using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
///     Controls mouse events: placing, filling, deleting
///     <para>Attach to game manager</para>
/// </summary>
public class MouseEvents : MonoBehaviour
{
    private const float selectionCancelMaxTime = 0.15f;

    private void Update()
    {
        // selection
        if (KeyBinds.GetKeyBindDown("Editor_Select")) StartCoroutine(StartCancelSelection());

        CheckPlaceAndDelete();

        // track drag positions
        if (!Input.GetMouseButtonUp(0)) return;

        MouseManager.Instance.MouseDragStart = null;
        MouseManager.Instance.MouseDragCurrent = null;
        MouseManager.Instance.MouseDragEnd = null;

        LevelSessionEditManager.Instance.OnEditAction.Invoke();
    }


    private static void CheckPlaceAndDelete()
    {
        EditMode editMode = LevelSessionEditManager.Instance.CurrentEditMode;

        // place / delete stuff
        if (MouseManager.Instance.IsUIHovered
            || LevelSessionEditManager.Instance.Playing
            || SelectionManager.Instance.Selecting
            || CopyManager.Instance.Pasting
            || AnchorPositionInputEditManager.Instance.IsEditing) return;

        // if none of the relevant keys is held, check field placement + entity placement
        if (!KeyBinds.GetKeyBind("Editor_MoveEntity")
            && !KeyBinds.GetKeyBind("Editor_Modify")
            && !KeyBinds.GetKeyBind("Editor_DeleteEntity")
            && !SelectionManager.Instance.Selecting)
        {
            if (Input.GetMouseButton(0)) CheckDragPlacement(editMode);
            if (Input.GetMouseButtonDown(0)) CheckClickPlacement(editMode);
        }

        CheckEntityDelete();
    }

    private static IEnumerator StartCancelSelection()
    {
        float passedTime = 0;
        while (KeyBinds.GetKeyBind("Editor_Select"))
        {
            if (passedTime > selectionCancelMaxTime || MouseManager.Instance.MousePosDelta.magnitude > 10) yield break;
            passedTime += Time.deltaTime;
            yield return null;
        }

        SelectionManager.Instance.CancelSelection();
    }

    private static void CheckClickPlacement(EditMode editMode)
    {
        if (editMode.IsDraggable) return;

        PlaceManager.Instance.Place(editMode, MouseManager.Instance.MouseWorldPos, LevelSessionEditManager.Instance.EditRotation, true);
    }

    private static void CheckDragPlacement(EditMode editMode)
    {
        // check placement
        if (!editMode.IsDraggable) return;

        if (Vector2.Distance(MouseManager.Instance.MouseWorldPos, MouseManager.Instance.PrevMouseWorldPos) > 1.414f)
        {
            PlaceManager.Instance.PlacePath(
                editMode,
                MouseManager.Instance.PrevMouseWorldPos, MouseManager.Instance.MouseWorldPos,
                LevelSessionEditManager.Instance.EditRotation, true
            );
        }
        else
        {
            PlaceManager.Instance.Place(
                editMode, MouseManager.Instance.MouseWorldPos,
                LevelSessionEditManager.Instance.EditRotation, true
            );
        }
    }

    private static void CheckEntityDelete()
    {
        if (!KeyBinds.GetKeyBind("Editor_DeleteEntity")) return;

        if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) return;

        // delete entities
        PlaceManager.RemoveEntitiesAt(
            MouseManager.Instance.MouseWorldPosGrid,
            LayerManager.Instance.Layers.Entity
        );
    }
}