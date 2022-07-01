using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// controls mouse events: placing, filling, deleting
/// attach to game manager
/// </summary>
public class MouseEvents : MonoBehaviour
{
    void Update()
    {
        // get mouse position and scale it to units
        Vector2 mousePos = GameManager.GetMouseWorldPos();
        int mx = (int)Mathf.Round(mousePos.x);
        int my = (int)Mathf.Round(mousePos.y);
        GameManager.EditMode editMode = GameManager.Instance.CurrentEditMode;

        // place / delete stuff when not hovering toolbar
        if (!GameManager.Instance.UIHovered && !GameManager.Instance.Playing && !GameManager.Instance.Filling)
        {
            if (!Input.GetKey(GameManager.Instance.BallDragKey) &&
                !Input.GetKey(GameManager.Instance.EditSpeedKey) &&
                !Input.GetKey(GameManager.Instance.EntityDeleteKey))
            {
                // ondrag
                if (Input.GetMouseButton(0))
                {
                    if (FieldManager.IsEditModeFieldType(editMode))
                    {
                        // place field
                        FieldManager.FieldType type = (FieldManager.FieldType)Enum.Parse(typeof(FieldManager.FieldType), GameManager.Instance.CurrentEditMode.ToString());
                        FieldManager.SetField(mx, my, type);
                    }
                    else if (editMode == GameManager.EditMode.DELETE_FIELD)
                    {
                        // delete
                        FieldManager.RemoveField(mx, my, updateOutlines: true);

                        // remove player if at deleted pos
                        PlayerManager.RemovePlayerAtPos(mx, my);
                    }
                    else if (editMode == GameManager.EditMode.PLAYER)
                    {
                        // place player
                        GameObject player = PlayerManager.GetCurrentPlayer();
                        if(player != null)
                        {
                            PlayerManager.SetPlayer(mx, my, player.GetComponent<PlayerController>().speed);
                        }
                        else
                        {
                            PlayerManager.SetPlayer(mx, my);
                        }
                    }
                    else if (editMode == GameManager.EditMode.COIN)
                    {
                        CoinManager.SetCoin(mx, my);
                    }
                    else if (editMode == GameManager.EditMode.GRAY_KEY)
                    {
                        KeyManager.SetKey(mx, my, FieldManager.KeyDoorColor.GRAY);
                    }
                    else if (editMode == GameManager.EditMode.RED_KEY)
                    {
                        KeyManager.SetKey(mx, my, FieldManager.KeyDoorColor.RED);
                    }
                    else if (editMode == GameManager.EditMode.GREEN_KEY)
                    {
                        KeyManager.SetKey(mx, my, FieldManager.KeyDoorColor.GREEN);
                    }
                    else if (editMode == GameManager.EditMode.BLUE_KEY)
                    {
                        KeyManager.SetKey(mx, my, FieldManager.KeyDoorColor.BLUE);
                    }
                    else if (editMode == GameManager.EditMode.YELLOW_KEY)
                    {
                        KeyManager.SetKey(mx, my, FieldManager.KeyDoorColor.YELLOW);
                    }
                }

                // onclick
                if (Input.GetMouseButtonDown(0))
                {
                    if (editMode == GameManager.EditMode.BALL_DEFAULT)
                    {
                        // place new ball
                        BallManager.SetBall(mx, my);
                    }
                    else if (editMode == GameManager.EditMode.BALL_CIRCLE)
                    {
                        // place new ball circle
                        BallCircleManager.SetBallCircle(mx, my);
                    }
                }
            }

            if (Input.GetKey(GameManager.Instance.EntityDeleteKey))
            {
                if (Input.GetMouseButton(0) && (Input.GetMouseButtonDown(0) || !Input.mousePosition.Equals(GameManager.Instance.PrevMousePos)))
                {
                    // delete entities
                    // remove player
                    PlayerManager.RemovePlayerAtPos(mx, my);

                    // remove coins
                    CoinManager.RemoveCoin(mx, my);

                    // remove balls
                    BallManager.RemoveBall(mx, my);
                    BallCircleManager.RemoveBallCircle(mx, my);

                    // remove keys
                    KeyManager.RemoveKey(mx, my);
                }
            }
        }

        // track drag positions and filling
        if (Input.GetMouseButtonUp(0))
        {
            GameManager.Instance.MouseDragStart = null;
            GameManager.Instance.MouseDragEnd = null;

            // reset fill marking
            foreach (Transform stroke in GameManager.Instance.FillOutlineContainer.transform)
            {
                Destroy(stroke.gameObject);
            }

            // reset preview
            foreach (Transform preview in GameManager.Instance.FillPreviewContainer.transform)
            {
                Destroy(preview.gameObject);
            }

            // enable placement preview
            GameManager.Instance.PlacementPreview.SetActive(true);
            GameManager.Instance.PlacementPreview.transform.position = GameManager.Instance.MousePosWorldSpaceRounded;

            // fill
            if (GameManager.Instance.Filling)
            {
                if (!GameManager.Instance.Playing && FieldManager.IsEditModeFieldType(editMode) && !GameManager.Instance.UIHovered)
                {

                    // fill fields
                    FieldManager.FieldType type = (FieldManager.FieldType)Enum.Parse(typeof(FieldManager.FieldType), GameManager.Instance.CurrentEditMode.ToString());
                    FillManager.FillArea(GameManager.Instance.CurrentFillRange, type);

                }
                else if (editMode == GameManager.EditMode.DELETE_FIELD)
                {
                    // fill delete
                    foreach (Vector2 pos in GameManager.Instance.CurrentFillRange)
                    {
                        int fillX = (int)pos.x;
                        int fillY = (int)pos.y;
                        FieldManager.RemoveField(fillX, fillY, updateOutlines: true);

                        // remove player if at deleted pos
                        PlayerManager.RemovePlayerAtPos(fillX, fillY);
                    }
                }
                else if (editMode == GameManager.EditMode.COIN)
                {
                    // fill coins
                    foreach (Vector2 pos in GameManager.Instance.CurrentFillRange)
                    {
                        int fillX = (int)pos.x;
                        int fillY = (int)pos.y;
                        CoinManager.SetCoin(fillX, fillY);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            GameManager.Instance.MouseDragStart = new(mx, my);
        }
        if (Input.GetMouseButton(0))
        {
            GameManager.Instance.MouseDragEnd = new(mx, my);
        }
    }
}