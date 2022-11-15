using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Key attributes: position, color
/// </summary>
[System.Serializable]
public class KeyData : IData
{
    public float[] position;
    public KeyManager.KeyColor color;

    public KeyData(KeyController controller)
    {
        position = new float[2];
        position[0] = controller.transform.position.x;
        position[1] = controller.transform.position.y;
        color = controller.color;
    }

    public override void ImportToLevel()
    {
        KeyManager.Instance.SetKey(position[0], position[1], color);
    }
}
