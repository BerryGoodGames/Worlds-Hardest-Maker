using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoomController : MonoBehaviour
{
    [HideInInspector] public int roomID;


    private void Start()
    {
        
    }
    private void UpdateSize()
    {
        LevelRoom room = GetRoom();
        
    }

    public LevelRoom GetRoom() => LevelRoomManager.GetRoom(roomID);
}
