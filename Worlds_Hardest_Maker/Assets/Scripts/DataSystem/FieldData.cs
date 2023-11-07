using System;
using UnityEngine;

/// <summary>
///     Field attributes: position, type
/// </summary>
[Serializable]
public class FieldData : Data
{
    public int[] Position;
    public string FieldType;
    public int Rotation;

    public FieldData(GameObject field)
    {
        Vector2 fieldPosition = field.transform.position;

        Position = new int[2];
        Position[0] = (int)fieldPosition.x;
        Position[1] = (int)fieldPosition.y;
        Rotation = (int)field.transform.rotation.eulerAngles.z;

        FieldType typeEnum = (FieldType)FieldManager.GetFieldType(field);
        FieldType = typeEnum.ToString();
    }

    public override void ImportToLevel() => ImportToLevel(new(Position[0], Position[1]));

    public override void ImportToLevel(Vector2 pos)
    {
        FieldType type = (FieldType)Enum.Parse(typeof(FieldType), FieldType);

        FieldManager.Instance.SetField(Vector2Int.RoundToInt(pos), type, Rotation);
    }

    public override EditMode GetEditMode() => (EditMode)Enum.Parse(typeof(EditMode), FieldType);
}