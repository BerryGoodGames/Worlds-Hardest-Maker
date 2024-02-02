using System.Collections.Generic;

public static class KeyColorExtension
{
    public static KeyController GetPrefabKey(this KeyColor color)
    {
        Dictionary<KeyColor, KeyController> prefabs = new()
        {
            { KeyColor.Gray, PrefabManager.Instance.GrayKey },
            { KeyColor.Red, PrefabManager.Instance.RedKey },
            { KeyColor.Blue, PrefabManager.Instance.BlueKey },
            { KeyColor.Green, PrefabManager.Instance.GreenKey },
            { KeyColor.Yellow, PrefabManager.Instance.YellowKey },
        };

        return prefabs[color];
    }
}