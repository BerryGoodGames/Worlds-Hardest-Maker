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
    private const float SelectionCancelMaxTime = 0.15f;

    private void Update()
    {
        PhotonView photonView = GameManager.Instance.photonView;
        bool multiplayer = MultiplayerManager.Instance.Multiplayer;

        EditMode editMode = EditModeManager.Instance.CurrentEditMode;

        // selection
        if (Input.GetMouseButtonDown(KeybindManager.Instance.SelectionMouseButton)) StartCoroutine(StartCancelSelection());

        // select anchor
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeybindManager.Instance.EditSpeedKey))
            AnchorManager.Instance.SelectAnchor(MouseManager.Instance.MouseWorldPosGrid);

        // select anchor through anchor ball
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeybindManager.Instance.EditSpeedKey))
            AnchorBallManager.SelectAnchorBall(MouseManager.Instance.MouseWorldPosGrid);

        // place / delete stuff
        if (!MouseManager.Instance.IsUIHovered && !EditModeManager.Instance.Playing &&
            !SelectionManager.Instance.Selecting &&
            !CopyManager.Instance.Pasting &&
            !AnchorPositionInputEditManager.Instance.IsEditing)
        {
            // if none of the relevant keys is held, check field placement + entity placement
            if (!Input.GetKey(KeybindManager.Instance.EntityMoveKey) &&
                !Input.GetKey(KeybindManager.Instance.EditSpeedKey) &&
                !Input.GetKey(KeybindManager.Instance.EntityDeleteKey) &&
                !SelectionManager.Instance.Selecting)
            {
                if (Input.GetMouseButton(0)) CheckDragPlacement(editMode);
                if (Input.GetMouseButtonDown(0)) CheckClickPlacement(editMode);
            }

            CheckEntityDelete(photonView, multiplayer);
        }

        // track drag positions
        if (!Input.GetMouseButtonUp(0)) return;

        MouseManager.Instance.MouseDragStart = null;
        MouseManager.Instance.MouseDragCurrent = null;
        MouseManager.Instance.MouseDragEnd = null;
    }

    private static IEnumerator StartCancelSelection()
    {
        float passedTime = 0;
        while (Input.GetMouseButton(KeybindManager.Instance.SelectionMouseButton))
        {
            if (passedTime > SelectionCancelMaxTime || MouseManager.Instance.MousePosDelta.magnitude > 10) yield break;
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
        if (!Input.GetKey(KeybindManager.Instance.EntityDeleteKey)) return;

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