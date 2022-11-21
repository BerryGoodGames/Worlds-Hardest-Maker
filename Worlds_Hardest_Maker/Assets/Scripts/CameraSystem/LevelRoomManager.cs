using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoomManager : MonoBehaviour
{
    public static LevelRoomManager Instance { get; private set; }

    [SerializeField] private Vector2 startRoomSize;

    public int currentID = 0;
    public List<LevelRoom> levelRooms = new();

    public static GameObject CreateRoom(Vector2 center, params (int, int)[] links)
    {
        GameObject levelRoom = Instantiate(GameManager.Instance.LevelRoom, Vector2.zero, Quaternion.identity, GameManager.Instance.LevelRoomContainer.transform);
        LevelRoomController roomController = levelRoom.GetComponent<LevelRoomController>();

        LevelRoom roomObject = AddRoom(center, links);

        roomController.roomID = roomObject.id;

        return levelRoom;
    }
    public static LevelRoom AddRoom(LevelRoom newRoom)
    {
        Instance.levelRooms.Add(newRoom);
        return newRoom;
    }
    private static LevelRoom AddRoom(Vector2 center, params (int, int)[] links)
    {
        LevelRoom room = new(center);

        foreach((int dir, int id) in links)
        {
            room.SetLink(dir, id);
        }

        AddRoom(room);

        return room;
    }
    public static LevelRoom GetRoom(int id)
    {
        return Instance.levelRooms.Find(e => e.id == id);
    }
    public static void DeleteRoom(int id)
    {
        Instance.levelRooms.Remove(GetRoom(id));
    }
    public static void DeleteRoom(LevelRoom room)
    {
        Instance.levelRooms.Remove(room);
    }

    public static void SetRoomSize(float width, float height)
    {
        LevelRoom.size = new(width, height);
    }


    public static int NewID()
    {
        Instance.currentID++;
        return Instance.currentID - 1;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        LevelRoom.size = startRoomSize;
    }
}
