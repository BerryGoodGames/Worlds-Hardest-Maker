using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoom
{
    public static Vector2 size;
    public int id;
    public Vector2 center;
    public (int up, int left, int down, int right) links;

    public LevelRoom()
    {
        id = LevelRoomManager.NewID();
    }
    public LevelRoom(Vector2 center) : this()
    {
        this.center = center;
    }


    public void SnapCamera()
    {
        // moves and resizes camera to fit in this room



    }
    public void MovePlayer(int dir)
    {
        // spawns player at direction of dir viewed from center of room



    }
    public List<Vector2> AvailibleSpaces(int dir)
    {

        return null;
    }
    public void SetLink(int dir, int roomID)
    {
        // connects this room to room with roomID at direction dir
        LevelRoom room = LevelRoomManager.GetRoom(roomID);

        switch (dir)
        {
            case 0:
                // up
                links.up = room.id;
                break;
            case 1:
                // left
                links.left = room.id;
                break;
            case 2:
                // down
                links.down = room.id;
                break;
            case 3:
                // right
                links.right = room.id;
                break;
        }
    }
    public void PlaceBesideRoom(int dir, int roomID)
    {
        // places this room to room with roomID at direction dir
        LevelRoom room = LevelRoomManager.GetRoom(roomID);

        switch (dir)
        {
            case 0:
                // up
                center = room.center + Vector2.up * size.y;
                break;
            case 1:
                // left
                center = room.center + Vector2.left * size.x;
                break;
            case 2:
                // down
                center = room.center + Vector2.down * size.y;
                break;
            case 3:
                // right
                center = room.center + Vector2.right * size.x;
                break;
        }
    }
    public void AppendRoom(int dir)
    {
        LevelRoom room = new();

        SetLink(dir, room.id);
        room.PlaceBesideRoom(dir, id);

        LevelRoomManager.AddRoom(room);
    }
}
