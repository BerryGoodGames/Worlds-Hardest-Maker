using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Key attributes: position, color
/// </summary>
[System.Serializable]
public class KeyData : IData
{
    public int[] position;
    public FieldManager.KeyDoorColor color;

    public KeyData(KeyController controller)
    {
        position = new int[2];
        position[0] = (int)controller.transform.position.x;
        position[1] = (int)controller.transform.position.y;
        color = controller.color;
    }

    public override void CreateObject()
    {
        KeyManager.SetKey(position[0], position[1], color);
    }
}
