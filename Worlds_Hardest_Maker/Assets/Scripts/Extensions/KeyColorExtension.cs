using System.Collections.Generic;
using UnityEngine;

public static class KeyColorExtension
{
    public static GameObject GetPrefabKey(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.GRAY, PrefabManager.Instance.grayKey },
            { KeyManager.KeyColor.RED, PrefabManager.Instance.redKey },
            { KeyManager.KeyColor.BLUE, PrefabManager.Instance.blueKey },
            { KeyManager.KeyColor.GREEN, PrefabManager.Instance.greenKey },
            { KeyManager.KeyColor.YELLOW, PrefabManager.Instance.yellowKey }
        };
        return prefabs[color];
    }

    public static GameObject GetPrefabDoor(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.GRAY, PrefabManager.Instance.grayKeyDoorField },
            { KeyManager.KeyColor.RED, PrefabManager.Instance.redKeyDoorField },
            { KeyManager.KeyColor.BLUE, PrefabManager.Instance.blueKeyDoorField },
            { KeyManager.KeyColor.GREEN, PrefabManager.Instance.greenKeyDoorField },
            { KeyManager.KeyColor.YELLOW, PrefabManager.Instance.yellowKeyDoorField }
        };
        return prefabs[color];
    }
}