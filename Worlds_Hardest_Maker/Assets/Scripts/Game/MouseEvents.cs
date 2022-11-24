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

        // place / delete stuff when not hovering toolbar
        if (!GameManager.Instance.UIHovered && !GameManager.Instance.Playing && !GameManager.Instance.Selecting && !CopyManager.pasting)
        {
            if (!Input.GetKey(GameManager.Instance.EntityMoveKey) &&
                !Input.GetKey(GameManager.Instance.EditSpeedKey) &&
                !Input.GetKey(GameManager.Instance.EntityDeleteKey) &&
                !CopyManager.pasting &&
                !GameManager.Instance.Selecting) 
            {
                // ondrag
                if (Input.GetMouseButton(0))
                {
                    if (editMode.IsFieldType())
                    {
                        // place field

                        int rotation = FieldManager.IsRotatable(editMode) ? GameManager.Instance.EditRotation : 0;

                        FieldManager.FieldType type = GameManager.ConvertEnum<GameManager.EditMode, FieldManager.FieldType>(editMode);

                        // fill path between two mouse pos for smoother placing on low framerate
                        if (Vector2.Distance(MouseManager.Instance.MouseWorldPos, MouseManager.Instance.PrevMouseWorldPos) > 1.414f)
                        {
                            // generalized Bresenham's Line Algorithm optimized without /, find (unoptimized) algorithm here: https://www.uobabylon.edu.iq/eprints/publication_2_22893_6215.pdf
                            // I tried my best to explain the variables, but I have no idea how it works

                            Vector2 A = MouseManager.Instance.MouseWorldPos;
                            Vector2 B = MouseManager.Instance.PrevMouseWorldPos;

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
                                FieldManager.Instance.SetField((int)X, (int)Y, type, rotation);
                                cmpt -= 1;

                                if (error >= 0 || XaY) X += incX;
                                if (error >= 0 || !XaY) Y += incY;
                                if (error >= 0) error += incD;
                                else error += incS;
                            }
                        }
                        else
                            FieldManager.Instance.SetField(matrixX, matrixY, type, rotation);
                    }
                    else
                        GameManager.Set(editMode, mousePos);
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

        // track drag positions
        if (Input.GetMouseButtonUp(0))
        {
            MouseManager.Instance.MouseDragStart = null;
            MouseManager.Instance.MouseDragCurrent = null;
            MouseManager.Instance.MouseDragEnd = null;
        }
    }
}