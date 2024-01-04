using System;
using UnityEngine;

/// <summary>
///     Field attributes: position, type
/// </summary>
[Serializable]
public class FieldData : Data
{
    public int[] Position;
    public string FieldMode;
    public int Rotation;

    public FieldData(FieldController field)
    {
        Vector2 fieldPosition = field.transform.position;

        Position = new int[2];
        Position[0] = (int)fieldPosition.x;
        Position[1] = (int)fieldPosition.y;
        Rotation = (int)field.transform.rotation.eulerAngles.z;

        FieldMode mode = field.FieldMode;
        FieldMode = mode.ToString();
    }

    public override void ImportToLevel() => ImportToLevel(new(Position[0], Position[1]));

    public override void ImportToLevel(Vector2 pos)
    {
        FieldMode mode = (FieldMode)Enum.Parse(typeof(FieldMode), FieldMode);

        FieldManager.Instance.SetField(Vector2Int.RoundToInt(pos), mode, Rotation);
    }

    public override EditMode GetEditMode() => (EditMode)Enum.Parse(typeof(EditMode), FieldMode);
}