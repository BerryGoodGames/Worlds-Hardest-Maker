using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyColorExtension
{
    public static GameObject GetPrefabKey(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.GRAY, GameManager.Instance.GrayKey },
            { KeyManager.KeyColor.RED, GameManager.Instance.RedKey },
            { KeyManager.KeyColor.BLUE, GameManager.Instance.BlueKey },
            { KeyManager.KeyColor.GREEN, GameManager.Instance.GreenKey },
            { KeyManager.KeyColor.YELLOW, GameManager.Instance.YellowKey }
        };
        return prefabs[color];
    }

    public static GameObject GetPrefabDoor(this KeyManager.KeyColor color)
    {
        Dictionary<KeyManager.KeyColor, GameObject> prefabs = new()
        {
            { KeyManager.KeyColor.GRAY, GameManager.Instance.GrayKeyDoorField },
            { KeyManager.KeyColor.RED, GameManager.Instance.RedKeyDoorField },
            { KeyManager.KeyColor.BLUE, GameManager.Instance.BlueKeyDoorField },
            { KeyManager.KeyColor.GREEN, GameManager.Instance.GreenKeyDoorField },
            { KeyManager.KeyColor.YELLOW, GameManager.Instance.YellowKeyDoorField }
        };
        return prefabs[color];
    }
}
