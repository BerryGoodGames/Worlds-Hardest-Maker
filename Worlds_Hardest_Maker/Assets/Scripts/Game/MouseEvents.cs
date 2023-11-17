using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
        
        if (KeyBinds.GetKeyBindDown("Editor_Pick")) 

        CheckPlaceAndDelete();

        // track drag positions
        if (!Input.GetMouseButtonUp(0)) return;

        MouseManager.Instance.MouseDragStart = null;
        MouseManager.Instance.MouseDragCurrent = null;
        MouseManager.Instance.MouseDragEnd = null;
    }


    private static void CheckPlaceAndDelete()
    {
        PhotonView photonView = GameManager.Instance.photonView;
        bool multiplayer = MultiplayerManager.Instance.Multiplayer;
        EditMode editMode = EditModeManager.Instance.CurrentEditMode;
        
        // place / delete stuff
        if (MouseManager.Instance.IsUIHovered
            || EditModeManager.Instance.Playing
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

        CheckEntityDelete(photonView, multiplayer);
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
        // place anchor
        if (editMode is EditMode.Anchor)
        {
            // place new anchor + select
            AnchorController anchor = AnchorManager.Instance.SetAnchor(MouseManager.Instance.MouseWorldPosGrid);
            if (anchor != null) AnchorManager.Instance.SelectAnchor(anchor);
        }
    }

    private static void CheckDragPlacement(EditMode editMode)
    {
        List<EditMode> dragPlaceEditModes = new()
        {
            EditMode.DeleteField, EditMode.Player, EditMode.Coin, EditMode.AnchorBall,
        };

        // check placement
        if (dragPlaceEditModes.Contains(editMode) || editMode.IsKey() || editMode.IsFieldType())
            GameManager.PlaceEditModeAtPosition(editMode, MouseManager.Instance.MouseWorldPos);

        // if user dragged to fast, fill path between two mouse pos for smoother placing on low framerate
        if (Vector2.Distance(MouseManager.Instance.MouseWorldPos, MouseManager.Instance.PrevMouseWorldPos) > 1.414f &&
            editMode.IsFieldType())
        {
            FieldType type = EnumUtils.ConvertEnum<EditMode, FieldType>(editMode);
            int rotation = type.IsRotatable() ? EditModeManager.Instance.EditRotation : 0;
            FieldManager.FillPathWithFields(type, rotation);
        }
    }

    private static void CheckEntityDelete(PhotonView photonView, bool multiplayer)
    {
        if (!KeyBinds.GetKeyBind("Editor_DeleteEntity")) return;

        if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) return;

        // delete entities
        if (multiplayer)
        {
            // remove player (only own client)
            PlayerManager.Instance.RemovePlayerAtPosIgnoreOtherClients(MouseManager.Instance.MouseWorldPosGrid);

            // remove coins
            photonView.RPC("RemoveCoin", RpcTarget.All, MouseManager.Instance.MouseWorldPosGrid);

            // remove balls
            photonView.RPC("RemoveAnchorBall", RpcTarget.All, MouseManager.Instance.MouseWorldPosGrid);

            // remove anchors
            photonView.RPC("RemoveAnchor", RpcTarget.All, MouseManager.Instance.MouseWorldPosGrid);

            // remove keys
            photonView.RPC("RemoveKey", RpcTarget.All, MouseManager.Instance.MouseWorldPosGrid);
        }
        else
        {
            // remove entity
            GameEntityManager.RemoveEntitiesAt(
                MouseManager.Instance.MouseWorldPosGrid,
                LayerManager.Instance.Layers.Entity
            );
        }
    }
}