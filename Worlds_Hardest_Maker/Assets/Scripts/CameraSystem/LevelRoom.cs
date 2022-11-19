using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoom
{
    public static Vector2 size;
    public int id;
    public Vector2 center;
    public (int up, int left, int down, int right) links;


    public void SnapCamera()
    {
        // moves and resizes camera to fit in this room



    }
    public void MovePlayer(int dir)
    {

    }
    public List<Vector2> AvailibleSpaces(int dir)
    {
        return null;
    }
    public void AppendRoom(int dir)
    {
        LevelRoom room = new()
        {
            id = LevelRoomManager.NewID()
        };
        switch (dir)
        {
            case 0:
                // up
                room.links.up = id;
                room.center = center + Vector2.up * size.y;
                break;
            case 1:
                // left
                room.links.left = id;
                room.center = center + Vector2.left * size.x;
                break;
            case 2:
                // down
                room.links.down = id;
                room.center = center + Vector2.down * size.y;
                break;
            case 3:
                // right
                room.links.right = id;
                room.center = center + Vector2.right * size.x;
                break;

        }

        LevelRoomManager.AddRoom(room);
    }
}
