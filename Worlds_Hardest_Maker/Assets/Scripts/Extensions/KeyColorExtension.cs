using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyColorExtension
{
    public static GameObject GetPrefabKey(this MKey.KeyColor color)
    {
        Dictionary<MKey.KeyColor, GameObject> prefabs = new()
        {
            { MKey.KeyColor.GRAY, MGame.Instance.GrayKey },
            { MKey.KeyColor.RED, MGame.Instance.RedKey },
            { MKey.KeyColor.BLUE, MGame.Instance.BlueKey },
            { MKey.KeyColor.GREEN, MGame.Instance.GreenKey },
            { MKey.KeyColor.YELLOW, MGame.Instance.YellowKey }
        };
        return prefabs[color];
    }

    public static GameObject GetPrefabDoor(this MKey.KeyColor color)
    {
        Dictionary<MKey.KeyColor, GameObject> prefabs = new()
        {
            { MKey.KeyColor.GRAY, MGame.Instance.GrayKeyDoorField },
            { MKey.KeyColor.RED, MGame.Instance.RedKeyDoorField },
            { MKey.KeyColor.BLUE, MGame.Instance.BlueKeyDoorField },
            { MKey.KeyColor.GREEN, MGame.Instance.GreenKeyDoorField },
            { MKey.KeyColor.YELLOW, MGame.Instance.YellowKeyDoorField }
        };
        return prefabs[color];
    }
}
