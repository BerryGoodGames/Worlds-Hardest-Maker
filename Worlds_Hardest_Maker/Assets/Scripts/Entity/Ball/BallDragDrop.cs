using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// script for enabling drag and drop on ball objects, also bounce pos or origin pos
/// attach to gameobject to be dragged
/// </summary>
public class BallDragDrop : MonoBehaviourPun
{
    public static Dictionary<int, GameObject> DragDropList = new();

    [SerializeField] private IBallController controller;
    [SerializeField] private int id;
    private void Awake()
    {
        // assign id
        id = NextID();
        DragDropList.Add(id, gameObject);
    }

    private void OnMouseDrag()
    {
        if (Input.GetKey(KeybindManager.Instance.EntityMoveKey))
        {
            Vector2 unitPos = MouseManager.Instance.MouseWorldPosGrid;
            if(GameManager.Instance.Multiplayer)
            {
                PhotonView controllerView = controller.GetComponent<PhotonView>();
                controllerView.RPC("MoveObject", RpcTarget.All, unitPos, id);
            }
            else controller.MoveObject(unitPos, id);
        }
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
