using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension
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

    public static Vector3 Ceil(this Vector3 vector)
    {
        int ceiledX = Mathf.CeilToInt(vector.x);
        int ceiledY = Mathf.CeilToInt(vector.y);
        int ceiledZ = Mathf.CeilToInt(vector.z);

        return new (ceiledX, ceiledY, ceiledZ);
    }

    public static Vector3 Floor(this Vector3 vector)
    {
        int flooredX = Mathf.FloorToInt(vector.x);
        int flooredY = Mathf.FloorToInt(vector.y);
        int flooredZ = Mathf.FloorToInt(vector.z);

        return new(flooredX, flooredY, flooredZ);
    }

    public static Vector2 Ceil(this Vector2 vector)
    {
        int ceiledX = Mathf.CeilToInt(vector.x);
        int ceiledY = Mathf.CeilToInt(vector.y);

        return new(ceiledX, ceiledY);
    }

    public static Vector2 Floor(this Vector2 vector)
    {
        int flooredX = Mathf.FloorToInt(vector.x);
        int flooredY = Mathf.FloorToInt(vector.y);

        return new(flooredX, flooredY);
    }

    public static Vector2 Clamp(this Vector2 v, float min, float max)
    {
        return new(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
    }

    public static bool PointOnScreen(this Vector2 point, bool worldPoint)
    {
        Vector3 screenPoint = worldPoint ? Camera.main.WorldToViewportPoint(point) : point;
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }
}
