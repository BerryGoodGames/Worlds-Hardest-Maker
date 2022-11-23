using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OneWayField attributes: position, rotation
/// </summary>
[System.Serializable]
public class OneWayData : IData
{
    public int[] position;
    public int rotation;

    public OneWayData(GameObject field)
    {
        position = new int[2];
        position[0] = (int)field.transform.position.x;
        position[1] = (int)field.transform.position.y;
        rotation = (int)field.transform.rotation.eulerAngles.z;
    }
    public override void ImportToLevel(Vector2 pos)
    {
        FieldManager.Instance.SetField(pos, FieldManager.FieldType.ONE_WAY_FIELD, rotation);
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(position[0], position[1]));
    }
}
