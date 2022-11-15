using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Field attributes: position, type
/// </summary>
[System.Serializable]
public class FieldData : IData
{
    public int[] position;
    public string fieldType;

    public FieldData(GameObject field)
    {
        position = new int[2];
        position[0] = (int)field.transform.position.x;
        position[1] = (int)field.transform.position.y;

        MField.FieldType typeEnum = (MField.FieldType)MField.GetFieldType(field);
        fieldType = typeEnum.ToString();
    }

    public override void ImportToLevel()
    {
        MField.FieldType type = (MField.FieldType)System.Enum.Parse(typeof(MField.FieldType), fieldType);

        MField.Instance.SetField(position[0], position[1], type);
    }
}
