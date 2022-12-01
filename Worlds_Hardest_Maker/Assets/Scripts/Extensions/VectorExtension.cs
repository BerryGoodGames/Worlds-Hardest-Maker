using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtention
{
    public static Vector2 ConvertPosition(this Vector2 pos, FollowMouse.WorldPosition worldPosition)
    {
        return worldPosition == FollowMouse.WorldPosition.ANY ? pos :
            worldPosition == FollowMouse.WorldPosition.GRID ? MouseManager.PosToGrid(pos) : MouseManager.PosToMatrix(pos);
    }

    public static bool Between(this Vector2 pos, Vector2 point1, Vector2 point2)
    {
        return pos.x > point1.x && pos.x < point2.x && pos.y > point1.y && pos.y < point2.y;
    }

    public static bool Between(this Vector3 pos, Vector3 point1, Vector3 point2)
    {
        return pos.x > point1.x && pos.x < point2.x && pos.y > point1.y && pos.y < point2.y;
    }
}
