using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionStuff
{
    public static Vector2 ConvertPosition(this Vector2 pos, FollowMouse.WorldPosition worldPosition)
    {
        return worldPosition == FollowMouse.WorldPosition.ANY ? pos :
            worldPosition == FollowMouse.WorldPosition.GRID ? MouseManager.PosToGrid(pos) : MouseManager.PosToMatrix(pos);
    }
}
