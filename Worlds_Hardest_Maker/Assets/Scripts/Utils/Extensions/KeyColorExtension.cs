using System.Collections.Generic;
using UnityEngine;

public static class KeyColorExtension
{
    public static KeyController GetPrefabKey(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, KeyController> prefabs = new()
        {
            { KeyManager.KeyColor.Gray, PrefabManager.Instance.GrayKey },
            { KeyManager.KeyColor.Red, PrefabManager.Instance.RedKey },
            { KeyManager.KeyColor.Blue, PrefabManager.Instance.BlueKey },
            { KeyManager.KeyColor.Green, PrefabManager.Instance.GreenKey },
            { KeyManager.KeyColor.Yellow, PrefabManager.Instance.YellowKey },
        };

        return prefabs[color];
    }

    public static GameObject GetPrefabDoor(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.Gray, PrefabManager.Instance.GrayKeyDoor },
            { KeyManager.KeyColor.Red, PrefabManager.Instance.RedKeyDoor },
            { KeyManager.KeyColor.Blue, PrefabManager.Instance.BlueKeyDoor },
            { KeyManager.KeyColor.Green, PrefabManager.Instance.GreenKeyDoor },
            { KeyManager.KeyColor.Yellow, PrefabManager.Instance.YellowKeyDoor },
        };

        return prefabs[color];
    }
}