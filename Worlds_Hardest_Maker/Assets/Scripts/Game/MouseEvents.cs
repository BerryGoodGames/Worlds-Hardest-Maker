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
        PhotonView pview = MGame.Instance.photonView;
        bool multiplayer = MGame.Instance.Multiplayer;

        // get mouse position and scale it to units
        Vector2 mousePos = MMouse.GetMouseWorldPos();
        int matrixX = (int)Mathf.Round(mousePos.x);
        int matrixY = (int)Mathf.Round(mousePos.y);
        float gridX = Mathf.Round(mousePos.x * 2) * 0.5f;
        float gridY = Mathf.Round(mousePos.y * 2) * 0.5f;

        MGame.EditMode editMode = MGame.Instance.CurrentEditMode;

        // select Anchor
        if (Input.GetKey(MGame.Instance.EditSpeedKey) && Input.GetMouseButtonDown(0))
        {
            MAnchor.SelectAnchor(MMouse.Instance.MouseWorldPosGrid);
        }

        // rotate rotatable fields
        if (MField.IsRotatable(MGame.Instance.CurrentEditMode) && Input.GetKeyDown(KeyCode.R))
        {
            MGame.Instance.EditRotation = (MGame.Instance.EditRotation - 90) % 360;
        }

        // place / delete stuff when not hovering toolbar
        if (!MGame.Instance.UIHovered && !MGame.Instance.Playing && !MGame.Instance.Filling)
        {
            if (!Input.GetKey(MGame.Instance.EntityMoveKey) &&
                !Input.GetKey(MGame.Instance.EditSpeedKey) &&
                !Input.GetKey(MGame.Instance.EntityDeleteKey))
            {
                // ondrag
                if (Input.GetMouseButton(0))
                {
                    if (editMode.IsFieldType())
                    {
                        // place field

                        int rotation = MField.IsRotatable(MGame.Instance.CurrentEditMode) ? MGame.Instance.EditRotation : 0;

                        MField.FieldType type = MGame.ConvertEnum<MGame.EditMode, MField.FieldType>(editMode);

                        // fill path between two mouse pos for smoother placing on low framerate
                        if (Vector2.Distance(MMouse.Instance.MouseWorldPos, MMouse.Instance.PrevMouseWorldPos) > 1.414f)
                        {
                            // generalized Bresenham's Line Algorithm optimized without /, find (unoptimized) algorithm here: https://www.uobabylon.edu.iq/eprints/publication_2_22893_6215.pdf
                            // I tried my best to explain the variables, but I have no idea how it works

                            Vector2 A = MMouse.Instance.MouseWorldPos;
                            Vector2 B = MMouse.Instance.PrevMouseWorldPos;

                            // increment and delta x
                            float incX = Mathf.Sign(B.x - A.x);
                            float dX = Mathf.Abs(B.x - A.x);

                            // increment and delta y
                            float incY = Mathf.Sign(B.y - A.y);
                            float dY = Mathf.Abs(B.y - A.y);

                            bool XaY = dX > dY; // if delta x is bigger than y
                            float cmpt = Mathf.Max(dX, dY); // max of both numbers
                            float incD = -2 * Mathf.Abs(dX - dY); // increment of delta
                            float incS = 2 * Mathf.Min(dX, dY); // I have no idea

                            float error = incD + cmpt; // error of line
                            float X = A.x; // where we are x
                            float Y = A.y; // where we are y

                            while (cmpt >= 0)
                            {
                                MField.Instance.SetField((int)X, (int)Y, type, rotation);
                                cmpt -= 1;

                                if (error >= 0 || XaY) X += incX;
                                if (error >= 0 || !XaY) Y += incY;
                                if (error >= 0) error += incD;
                                else error += incS;
                            }
                        }
                        else 
                            MField.Instance.SetField(matrixX, matrixY, type, rotation);
                    }
                    else if (editMode == MGame.EditMode.DELETE_FIELD)
                    {
                        // delete field
                        if (multiplayer) pview.RPC("RemoveField", RpcTarget.All, matrixX, matrixY, true);
                        else MField.Instance.RemoveField(matrixX, matrixY, updateOutlines: true);

                        // remove player if at deleted pos
                        if (multiplayer) pview.RPC("RemovePlayerAtPosIntersect", RpcTarget.All, (float)matrixX, (float)matrixY);
                        else MPlayer.Instance.RemovePlayerAtPosIntersect(matrixX, matrixY);
                    }
                    else if (editMode == MGame.EditMode.PLAYER)
                    {
                        // place player
                        MPlayer.Instance.SetPlayer(gridX, gridY);
                    }
                    else if (editMode == MGame.EditMode.COIN)
                    {
                        // place coin
                        if (multiplayer) pview.RPC("SetCoin", RpcTarget.All, gridX, gridY);
                        else MCoin.Instance.SetCoin(gridX, gridY);
                    }
                    else if (MKey.IsKeyEditMode(editMode))
                    {
                        // get keycolor
                        string keyColorStr = editMode.ToString()[..^4];
                        MKey.KeyColor keyColor = (MKey.KeyColor)Enum.Parse(typeof(MKey.KeyColor), keyColorStr);

                        // place key
                        if (multiplayer) pview.RPC("SetKey", RpcTarget.All, gridX, gridY, keyColor);
                        else MKey.Instance.SetKey(gridX, gridY, keyColor);
                    }
                }

                // onclick
                if (Input.GetMouseButtonDown(0))
                {
                    if(editMode == MGame.EditMode.ANCHOR)
                    {
                        // place new anchor
                        MAnchor.Instance.SetAnchor(gridX, gridY);
                    }
                    if (editMode == MGame.EditMode.BALL)
                    {
                        MAnchorBall.SetAnchorBall(gridX, gridY);
                    }
                    if (editMode == MGame.EditMode.BALL_DEFAULT)
                    {
                        // place new ball
                        MBall.Instance.SetBall(gridX, gridY);
                    }
                    else if (editMode == MGame.EditMode.BALL_CIRCLE)
                    {
                        // place new ball circle
                        MBallCircle.Instance.SetBallCircle(gridX, gridY);
                    }
                }
            }

            if (Input.GetKey(MGame.Instance.EntityDeleteKey))
            {
                if (Input.GetMouseButton(0) && (Input.GetMouseButtonDown(0) || !Input.mousePosition.Equals(MMouse.Instance.PrevMousePos)))
                {
                    // delete entities
                    if (multiplayer)
                    {
                        // remove player (only own client)
                        MPlayer.Instance.RemovePlayerAtPosIgnoreOtherClients(gridX, gridY);

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
                        MPlayer.Instance.RemovePlayerAtPos(gridX, gridY);

                        // remove coins
                        MCoin.Instance.RemoveCoin(gridX, gridY);

                        // remove balls
                        MBall.Instance.RemoveBall(gridX, gridY);
                        MBallCircle.Instance.RemoveBallCircle(gridX, gridY);
                        MAnchorBall.Instance.RemoveAnchorBall(gridX, gridY);
                        // MAnchorBall.Instance.RemoveBall(new(matrixX, matrixY));

                        // remove anchors
                        MAnchor.Instance.RemoveAnchor(gridX, gridY);

                        // remove keys
                        MKey.Instance.RemoveKey(gridX, gridY);
                    }
                }
            }
        }

        // track drag positions and filling
        if (Input.GetMouseButtonUp(0))
        {
            // fill
            if (MGame.Instance.Filling)
            {
                if (!MGame.Instance.Playing && editMode.IsFieldType() && !MGame.Instance.UIHovered)
                {
                    // fill fields
                    MField.FieldType type = MGame.ConvertEnum<MGame.EditMode, MField.FieldType>(editMode);

                    if (multiplayer) pview.RPC("FillArea", RpcTarget.All, MMouse.Instance.MouseDragStart, MMouse.Instance.MouseDragEnd, type);
                    else MFill.Instance.FillArea((Vector2)MMouse.Instance.MouseDragStart, (Vector2)MMouse.Instance.MouseDragEnd, type);
                }
                else if (editMode == MGame.EditMode.DELETE_FIELD)
                {
                    // fill delete
                    foreach (Vector2 pos in MGame.Instance.CurrentFillRange)
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
                            MField.Instance.RemoveField(fillX, fillY, true);

                            // TODO: 9x worse performance
                            // remove player if at deleted pos
                            MPlayer.Instance.RemovePlayerAtPosIntersect(fillX, fillY);
                        }
                    }
                }
                else if (editMode == MGame.EditMode.COIN)
                {
                    // fill coins
                    foreach (Vector2 pos in MGame.Instance.CurrentFillRange)
                    {
                        float fillX = pos.x;
                        float fillY = pos.y;

                        if(multiplayer) pview.RPC("SetCoin", RpcTarget.All, fillX, fillY);
                        else MCoin.Instance.SetCoin(fillX, fillY);
                    }
                }
            }

            MMouse.Instance.MouseDragStart = null;
            MMouse.Instance.MouseDragEnd = null;

            MFill.ResetPreview();
        }
    }
}