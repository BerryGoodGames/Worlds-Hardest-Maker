using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoomManager : MonoBehaviour
{
    public static LevelRoomManager Instance { get; private set; }

    public int currentID = 0;
    public List<LevelRoom> levelRooms = new();


    public static void AddRoom(LevelRoom newRoom)
    {
        Instance.levelRooms.Add(newRoom);
    }
    public static void DeleteRoom(int id)
    {

    }

    public static int NewID()
    {
        Instance.currentID++;
        return Instance.currentID - 1;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
