using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoomController : MonoBehaviour
{
    [HideInInspector] public int roomID;

    private UIAttachToPoint positionController;
    private RectTransform rt;

    private void Start()
    {
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        Camera camera = Camera.main;
        LevelRoom room = GetRoom();

        rt.sizeDelta = 8 * camera.orthographicSize / camera.aspect * new Vector2(LevelRoom.size.x, LevelRoom.size.y);
        positionController.point = room.center;
    }

    public LevelRoom GetRoom() => LevelRoomManager.GetRoom(roomID);

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        positionController = GetComponent<UIAttachToPoint>();
    }
}
