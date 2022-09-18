using System.Collections;
using System;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// controls mouse events: placing, filling, deleting
/// attach to game manager
/// </summary>
public class MouseEvents : MonoBehaviour
{
    void Update()
    {
        PhotonView pview = GameManager.Instance.photonView;
        bool multiplayer = GameManager.Instance.Multiplayer;

        // get mouse position and scale it to units
        Vector2 mousePos = MouseManager.GetMouseWorldPos();
        int matrixX = (int)Mathf.Round(mousePos.x);
        int matrixY = (int)Mathf.Round(mousePos.y);
        float gridX = Mathf.Round(mousePos.x * 2) * 0.5f;
        float gridY = Mathf.Round(mousePos.y * 2) * 0.5f;

        GameManager.EditMode editMode = GameManager.Instance.CurrentEditMode;

        // select Anchor
        if (Input.GetKey(GameManager.Instance.EditSpeedKey) && Input.GetMouseButtonDown(0))
        {
            AnchorManager.SelectAnchor(MouseManager.Instance.MouseWorldPosGrid);
        }

        // rotate one way field
        if (GameManager.Instance.CurrentEditMode == GameManager.EditMode.ONE_WAY_FIELD && Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.EditRotation = (GameManager.Instance.EditRotation + 90) % 360;
        }

        // place / delete stuff when not hovering toolbar
        if (!GameManager.Instance.UIHovered && !GameManager.Instance.Playing && !GameManager.Instance.Filling)
        {
            if (!Input.GetKey(GameManager.Instance.EntityMoveKey) &&
                !Input.GetKey(GameManager.Instance.EditSpeedKey) &&
                !Input.GetKey(GameManager.Instance.EntityDeleteKey))
            {
                // ondrag
                if (Input.GetMouseButton(0))
                {
                    if (editMode.IsFieldType())
                    {
                        // place field
                        FieldManager.FieldType type = GameManager.ConvertEnum<GameManager.EditMode, FieldManager.FieldType>(editMode);

                        // rotate it when its a one way field
                        int rotation = GameManager.Instance.CurrentEditMode == GameManager.EditMode.ONE_WAY_FIELD ? GameManager.Instance.EditRotation : 0;

                        FieldManager.Instance.SetField(matrixX, matrixY, type, rotation);
                    }
                    else if (editMode == GameManager.EditMode.DELETE_FIELD)
                    {
                        // delete field
                        if (multiplayer) pview.RPC("RemoveField", RpcTarget.All, matrixX, matrixY, true);
                        else FieldManager.Instance.RemoveField(matrixX, matrixY, updateOutlines: true);

                        // remove player if at deleted pos
                        if (multiplayer) pview.RPC("RemovePlayerAtPosIntersect", RpcTarget.All, (float)matrixX, (float)matrixY);
                        else PlayerManager.Instance.RemovePlayerAtPosIntersect(matrixX, matrixY);
                    }
                    else if (editMode == GameManager.EditMode.PLAYER)
                    {
                        // place player
                        PlayerManager.Instance.SetPlayer(gridX, gridY);
                    }
                    else if (editMode == GameManager.EditMode.COIN)
                    {
                        // place coin
                        if (multiplayer) pview.RPC("SetCoin", RpcTarget.All, gridX, gridY);
                        else CoinManager.Instance.SetCoin(gridX, gridY);
                    }
                    else if (KeyManager.IsKeyEditMode(editMode))
                    {
                        // get keycolor
                        string keyColorStr = editMode.ToString()[..^4];
                        KeyManager.KeyColor keyColor = (KeyManager.KeyColor)Enum.Parse(typeof(KeyManager.KeyColor), keyColorStr);

                        // place key
                        if (multiplayer) pview.RPC("SetKey", RpcTarget.All, gridX, gridY, keyColor);
                        else KeyManager.Instance.SetKey(gridX, gridY, keyColor);
                    }
                }

                // onclick
                if (Input.GetMouseButtonDown(0))
                {
                    if(editMode == GameManager.EditMode.ANCHOR)
                    {
                        // place new anchor
                        AnchorManager.Instance.SetAnchor(gridX, gridY);
                    }
                    if (editMode == GameManager.EditMode.BALL)
                    {
                        AnchorBallManager.SetAnchorBall(gridX, gridY);
                    }
                    if (editMode == GameManager.EditMode.BALL_DEFAULT)
                    {
                        // place new ball
                        BallManager.Instance.SetBall(gridX, gridY);
                    }
                    else if (editMode == GameManager.EditMode.BALL_CIRCLE)
                    {
                        // place new ball circle
                        BallCircleManager.Instance.SetBallCircle(gridX, gridY);
                    }
                }
            }

            if (Input.GetKey(GameManager.Instance.EntityDeleteKey))
            {
                if (Input.GetMouseButton(0) && (Input.GetMouseButtonDown(0) || !Input.mousePosition.Equals(MouseManager.Instance.PrevMousePos)))
                {
                    // delete entities
                    if (multiplayer)
                    {
                        // remove player (only own client)
                        PlayerManager.Instance.RemovePlayerAtPosIgnoreOtherClients(gridX, gridY);

                        // remove coins
                        pview.RPC("RemoveCoin", RpcTarget.All, gridX, gridY);

                        // remove balls
                        pview.RPC("RemoveBall", RpcTarget.All, gridX, gridY);
                        pview.RPC("RemoveBallCircle", RpcTarget.All, gridX, gridY);
                        pview.RPC("RemoveAnchorBall", RpcTarget.All, gridX, gridY);

                        // remove anchors
                        pview.RPC("RemoveAnchor", RpcTarget.All, gridX, gridY);

                        // remove keys
                        pview.RPC("RemoveKey", RpcTarget.All, gridX, gridY);
                    } else
                    {
                        // remove player
                        PlayerManager.Instance.RemovePlayerAtPos(gridX, gridY);

                        // remove coins
                        CoinManager.Instance.RemoveCoin(gridX, gridY);

                        // remove balls
                        BallManager.Instance.RemoveBall(gridX, gridY);
                        BallCircleManager.Instance.RemoveBallCircle(gridX, gridY);
                        AnchorBallManager.Instance.RemoveAnchorBall(gridX, gridY);
                        // AnchorBallManager.Instance.RemoveBall(new(matrixX, matrixY));

                        // remove anchors
                        AnchorManager.Instance.RemoveAnchor(gridX, gridY);

                        // remove keys
                        KeyManager.Instance.RemoveKey(gridX, gridY);
                    }
                }
            }
        }

        // track drag positions and filling
        if (Input.GetMouseButtonUp(0))
        {
            // fill
            if (GameManager.Instance.Filling)
            {
                if (!GameManager.Instance.Playing && editMode.IsFieldType() && !GameManager.Instance.UIHovered)
                {
                    // fill fields
                    FieldManager.FieldType type = GameManager.ConvertEnum<GameManager.EditMode, FieldManager.FieldType>(editMode);

                    if (multiplayer) pview.RPC("FillArea", RpcTarget.All, MouseManager.Instance.MouseDragStart, MouseManager.Instance.MouseDragEnd, type);
                    else FillManager.Instance.FillArea((Vector2)MouseManager.Instance.MouseDragStart, (Vector2)MouseManager.Instance.MouseDragEnd, type);
                }
                else if (editMode == GameManager.EditMode.DELETE_FIELD)
                {
                    // fill delete
                    foreach (Vector2 pos in GameManager.Instance.CurrentFillRange)
                    {
                        int fillX = (int)pos.x;
                        int fillY = (int)pos.y;

                        if (multiplayer)
                        {
                            // remove field
                            pview.RPC("RemoveField", RpcTarget.All, fillX, fillY, true);

                            // remove player if at deleted pos
                            pview.RPC("RemovePlayerAtPos", RpcTarget.All, fillX, fillY);
                        } else
                        {
                            // remove field
                            FieldManager.Instance.RemoveField(fillX, fillY, true);

                            // TODO: 9x worse performance
                            // remove player if at deleted pos
                            PlayerManager.Instance.RemovePlayerAtPosIntersect(fillX, fillY);
                        }
                    }
                }
                else if (editMode == GameManager.EditMode.COIN)
                {
                    // fill coins
                    foreach (Vector2 pos in GameManager.Instance.CurrentFillRange)
                    {
                        float fillX = pos.x;
                        float fillY = pos.y;

                        if(multiplayer) pview.RPC("SetCoin", RpcTarget.All, fillX, fillY);
                        else CoinManager.Instance.SetCoin(fillX, fillY);
                    }
                }
            }

            MouseManager.Instance.MouseDragStart = null;
            MouseManager.Instance.MouseDragEnd = null;

            FillManager.ResetPreview();
        }
    }
}