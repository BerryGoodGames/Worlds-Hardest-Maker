using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyColorExtension
{
    public static GameObject GetPrefabKey(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.GRAY, PrefabManager.Instance.GrayKey },
            { KeyManager.KeyColor.RED, PrefabManager.Instance.RedKey },
            { KeyManager.KeyColor.BLUE, PrefabManager.Instance.BlueKey },
            { KeyManager.KeyColor.GREEN, PrefabManager.Instance.GreenKey },
            { KeyManager.KeyColor.YELLOW, PrefabManager.Instance.YellowKey }
        };
        return prefabs[color];
    }

    public static GameObject GetPrefabDoor(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.GRAY, PrefabManager.Instance.GrayKeyDoorField },
            { KeyManager.KeyColor.RED, PrefabManager.Instance.RedKeyDoorField },
            { KeyManager.KeyColor.BLUE, PrefabManager.Instance.BlueKeyDoorField },
            { KeyManager.KeyColor.GREEN, PrefabManager.Instance.GreenKeyDoorField },
            { KeyManager.KeyColor.YELLOW, PrefabManager.Instance.YellowKeyDoorField }
        };
        return prefabs[color];
    }
}
