using System;
using UnityEngine;

/// <summary>
///     Field attributes: position, type
/// </summary>
[Serializable]
public class FieldData : IData
{
    public int[] position;
    public string fieldType;
    public int rotation;

    public FieldData(GameObject field)
    {
        position = new int[2];
        position[0] = (int)field.transform.position.x;
        position[1] = (int)field.transform.position.y;
        rotation = (int)field.transform.rotation.eulerAngles.z;

        FieldType typeEnum = (FieldType)FieldManager.GetFieldType(field);
        fieldType = typeEnum.ToString();
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(position[0], position[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        FieldType type = (FieldType)Enum.Parse(typeof(FieldType), fieldType);

        FieldManager.Instance.SetField(pos, type, rotation);
    }

    public override EditMode GetEditMode()
    {
        return (EditMode)Enum.Parse(typeof(EditMode), fieldType);
    }
}