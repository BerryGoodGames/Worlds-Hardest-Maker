using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Field attributes: position, type
/// </summary>
[Serializable]
public class FieldData : Data
{
    [FormerlySerializedAs("position")] public int[] Position;
    [FormerlySerializedAs("fieldType")] public string FieldType;
    [FormerlySerializedAs("rotation")] public int Rotation;

    public FieldData(GameObject field)
    {
        Position = new int[2];
        Position[0] = (int)field.transform.position.x;
        Position[1] = (int)field.transform.position.y;
        Rotation = (int)field.transform.rotation.eulerAngles.z;

        FieldType typeEnum = (FieldType)FieldManager.GetFieldType(field);
        FieldType = typeEnum.ToString();
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(Position[0], Position[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        FieldType type = (FieldType)Enum.Parse(typeof(FieldType), FieldType);

        FieldManager.Instance.SetField(pos, type, Rotation);
    }

    public override EditMode GetEditMode()
    {
        return (EditMode)Enum.Parse(typeof(EditMode), FieldType);
    }
}