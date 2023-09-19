using System.Collections.Generic;
using UnityEngine;

public static class KeyColorExtension
{
    public static GameObject GetPrefabKey(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.Gray, PrefabManager.Instance.GrayKey },
            { KeyManager.KeyColor.Red, PrefabManager.Instance.RedKey },
            { KeyManager.KeyColor.Blue, PrefabManager.Instance.BlueKey },
            { KeyManager.KeyColor.Green, PrefabManager.Instance.GreenKey },
            { KeyManager.KeyColor.Yellow, PrefabManager.Instance.YellowKey }
        };
        return prefabs[color];
    }

    public static GameObject GetPrefabDoor(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.Gray, PrefabManager.Instance.GrayKeyDoorField },
            { KeyManager.KeyColor.Red, PrefabManager.Instance.RedKeyDoorField },
            { KeyManager.KeyColor.Blue, PrefabManager.Instance.BlueKeyDoorField },
            { KeyManager.KeyColor.Green, PrefabManager.Instance.GreenKeyDoorField },
            { KeyManager.KeyColor.Yellow, PrefabManager.Instance.YellowKeyDoorField }
        };
        return prefabs[color];
    }
}