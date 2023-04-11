using System;
using UnityEngine;

public static class VectorExtension
{
    public static Vector2 ConvertPosition(this Vector2 pos, FollowMouse.WorldPositionType worldPositionType)
    {
        return worldPositionType switch
        {
            FollowMouse.WorldPositionType.ANY => pos,
            FollowMouse.WorldPositionType.GRID => MouseManager.PosToGrid(pos),
            _ => MouseManager.PosToMatrix(pos)
        };
    }

    public static bool Between(this Vector2 pos, Vector2 point1, Vector2 point2) =>
        pos.x > point1.x && pos.x < point2.x && pos.y > point1.y && pos.y < point2.y;

    public static bool Between(this Vector3 pos, Vector3 point1, Vector3 point2) =>
        pos.x > point1.x && pos.x < point2.x && pos.y > point1.y && pos.y < point2.y;

    public static Vector3 Ceil(this Vector3 vector)
    {
        int ceiledX = Mathf.CeilToInt(vector.x);
        int ceiledY = Mathf.CeilToInt(vector.y);
        int ceiledZ = Mathf.CeilToInt(vector.z);

        return new(ceiledX, ceiledY, ceiledZ);
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

    public static Vector2 Round(this Vector2 vector)
    {
        int roundedX = Mathf.RoundToInt(vector.x);
        int roundedY = Mathf.RoundToInt(vector.y);

        return new(roundedX, roundedY);
    }

    public static Vector3 Round(this Vector3 vector)
    {
        int roundedX = Mathf.RoundToInt(vector.x);
        int roundedY = Mathf.RoundToInt(vector.y);
        int roundedZ = Mathf.RoundToInt(vector.z);

        return new(roundedX, roundedY, roundedZ);
    }


    public static Vector2 Clamp(this Vector2 v, float min, float max) =>
        new(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));

    public static bool PointOnScreen(this Vector2 point, bool worldPoint)
    {
        if (Camera.main == null)
            throw new Exception("Couldn't check if point is on screen because main camera is null!");

        Vector3 screenPoint = worldPoint ? Camera.main.WorldToViewportPoint(point) : point;
        bool onScreen = screenPoint.x is > 0 and < 1 && screenPoint.y is > 0 and < 1;
        return onScreen;
    }
}