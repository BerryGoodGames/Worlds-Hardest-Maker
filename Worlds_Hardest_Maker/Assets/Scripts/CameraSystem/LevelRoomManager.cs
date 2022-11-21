using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoomManager : MonoBehaviour
{
    public static LevelRoomManager Instance { get; private set; }

    [SerializeField] private Vector2 startRoomSize;

    public int currentID = 0;
    public List<LevelRoom> levelRooms = new();


    public static void AddRoom(LevelRoom newRoom)
    {
        Instance.levelRooms.Add(newRoom);
    }
    public static void AddRoom(Vector2 center, params (int, int)[] links)
    {
        LevelRoom room = new(center);

        foreach((int dir, int id) in links)
        {
            room.SetLink(dir, id);
        }

        AddRoom(room);
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
