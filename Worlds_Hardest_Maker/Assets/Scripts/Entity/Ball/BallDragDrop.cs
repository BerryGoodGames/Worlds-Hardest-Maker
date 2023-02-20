using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
///     script for enabling drag and drop on ball objects, also bounce pos or origin pos
///     attach to gameObject to be dragged
/// </summary>
public class BallDragDrop : MonoBehaviourPun
{
    public static Dictionary<int, GameObject> DragDropList = new();

    [SerializeField] private BallController controller;
    [SerializeField] private int id;

    private void Awake()
    {
        // assign id
        id = NextID();
        DragDropList.Add(id, gameObject);
    }

    private void OnMouseDrag()
    {
        if (!Input.GetKey(KeybindManager.Instance.EntityMoveKey)) return;

        Vector2 unitPos = MouseManager.Instance.MouseWorldPosGrid;

        if (controller == null)
        {
            transform.position = unitPos;
            return;
        }

        if (MultiplayerManager.Instance.Multiplayer)
        {
            PhotonView controllerView = controller.GetComponent<PhotonView>();
            controllerView.RPC("MoveObject", RpcTarget.All, unitPos, id);
        }
        else controller.MoveObject(unitPos, id);
    }

    private void OnDestroy()
    {
        DragDropList.Remove(id);
    }

    private static int NextID()
    {
        int res = DragDropList.Count;
        while (DragDropList.ContainsKey(res))
        {
            res++;
        }

        return res;
    }
}