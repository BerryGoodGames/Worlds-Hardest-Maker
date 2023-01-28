using System.Collections;
using Photon.Pun;
using UnityEngine;

/// <summary>
///     controls mouse events: placing, filling, deleting
///     attach to game manager
/// </summary>
public class MouseEvents : MonoBehaviour
{
    private const float selectionCancelMaxTime = 0.15f;

    private void Update()
    {
        PhotonView photonView = GameManager.Instance.photonView;
        bool multiplayer = MultiplayerManager.Instance.Multiplayer;

        // get mouse position and scale it to units
        Vector2 mousePos = MouseManager.Instance.MouseWorldPos;

        int matrixX = (int)Mathf.Round(mousePos.x);
        int matrixY = (int)Mathf.Round(mousePos.y);

        float gridX = Mathf.Round(mousePos.x * 2) * 0.5f;
        float gridY = Mathf.Round(mousePos.y * 2) * 0.5f;

        EditMode editMode = EditModeManager.Instance.CurrentEditMode;

        // selection
        if (Input.GetMouseButtonDown(KeybindManager.Instance.selectionMouseButton))
            StartCoroutine(StartCancelSelection());

        // select Anchor
        if (Input.GetKey(KeybindManager.Instance.editSpeedKey) && Input.GetMouseButtonDown(0))
        {
            AnchorManager.Instance.SelectAnchor(MouseManager.Instance.MouseWorldPosGrid);
        }

        // place / delete stuff
        if (!MouseManager.Instance.IsUIHovered && !EditModeManager.Instance.Playing &&
            !SelectionManager.Instance.Selecting &&
            !CopyManager.pasting)
        {
            // if none of the relevant keys is held, check field placement + entity placement
            if (!Input.GetKey(KeybindManager.Instance.entityMoveKey) &&
                !Input.GetKey(KeybindManager.Instance.editSpeedKey) &&
                !Input.GetKey(KeybindManager.Instance.entityDeleteKey) &&
                !SelectionManager.Instance.Selecting)
            {
                CheckFieldPlacement(editMode, matrixX, matrixY, mousePos);
                CheckEntityPlacement(editMode, gridX, gridY);
            }

            CheckEntityDelete(gridX, gridY, photonView, multiplayer);
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
        while (Input.GetMouseButton(KeybindManager.Instance.selectionMouseButton))
        {
            if (passedTime > selectionCancelMaxTime || MouseManager.Instance.MousePosDelta.magnitude > 10) yield break;
            passedTime += Time.deltaTime;
            yield return null;
        }

        SelectionManager.Instance.CancelSelection();
    }

    private static void CheckFieldPlacement(EditMode editMode, int matrixX, int matrixY, Vector2 mousePos)
    {
        // on drag: place fields
        if (!Input.GetMouseButton(0)) return;

        if (!editMode.IsFieldType())
        {
            GameManager.PlaceEditModeAtPosition(editMode, mousePos);
            return;
        }

        // place field
        int rotation = FieldManager.IsRotatable(editMode) ? EditModeManager.Instance.EditRotation : 0;

        FieldType type = EnumUtils.ConvertEnum<EditMode, FieldType>(editMode);

        // if user didn't drag to fast, just place field normally
        if (Vector2.Distance(MouseManager.Instance.MouseWorldPos, MouseManager.Instance.PrevMouseWorldPos) < 1.414f)
        {
            FieldManager.Instance.SetField(matrixX, matrixY, type, rotation);
            return;
        }

        // if user did drag to fast, fill path between two mouse pos for smoother placing on low framerate
        FieldManager.FillPathWithFields(type, rotation);
    }

    private static void CheckEntityPlacement(EditMode editMode, float gridX, float gridY)
    {
        // onclick: place entities
        if (!Input.GetMouseButtonDown(0)) return;

        switch (editMode)
        {
            case EditMode.ANCHOR:
                // place new anchor
                AnchorManager.Instance.SetAnchor(gridX, gridY);
                break;
            case EditMode.BALL:
                AnchorBallManagerOld.SetAnchorBall(gridX, gridY);
                break;
            case EditMode.BALL_DEFAULT:
                // place new ball
                BallManager.Instance.SetBall(gridX, gridY);
                break;
            case EditMode.BALL_CIRCLE:
                // place new ball circle
                BallCircleManager.Instance.SetBallCircle(gridX, gridY);
                break;
        }
    }

    private static void CheckEntityDelete(float gridX, float gridY, PhotonView photonView, bool multiplayer)
    {
        if (!Input.GetKey(KeybindManager.Instance.entityDeleteKey)) return;

        //if (!Input.GetMouseButton(0) || (!Input.GetMouseButtonDown(0) && Input.mousePosition.Equals(MouseManager.Instance.PrevMousePos)))
        if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) return;

        // delete entities
        if (multiplayer)
        {
            // remove player (only own client)
            PlayerManager.Instance.RemovePlayerAtPosIgnoreOtherClients(gridX, gridY);

            // remove coins
            photonView.RPC("RemoveCoin", RpcTarget.All, gridX, gridY);

            // remove balls
            photonView.RPC("RemoveBall", RpcTarget.All, gridX, gridY);
            photonView.RPC("RemoveBallCircle", RpcTarget.All, gridX, gridY);
            photonView.RPC("RemoveAnchorBall", RpcTarget.All, gridX, gridY);

            // remove anchors
            photonView.RPC("RemoveAnchor", RpcTarget.All, gridX, gridY);

            // remove keys
            photonView.RPC("RemoveKey", RpcTarget.All, gridX, gridY);
        }
        else
        {
            // remove player
            PlayerManager.Instance.RemovePlayerAtPos(gridX, gridY);

            // remove coins
            CoinManager.Instance.RemoveCoin(gridX, gridY);

            // remove balls
            BallManager.Instance.RemoveBall(gridX, gridY);
            BallCircleManager.Instance.RemoveBallCircle(gridX, gridY);
            AnchorBallManagerOld.Instance.RemoveAnchorBall(gridX, gridY);
            // AnchorBallManager.Instance.RemoveBall(new(matrixX, matrixY));

            // remove anchors
            AnchorManager.Instance.RemoveAnchor(gridX, gridY);

            // remove keys
            KeyManager.Instance.RemoveKey(gridX, gridY);
        }
    }
}