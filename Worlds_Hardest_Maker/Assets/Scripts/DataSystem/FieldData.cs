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
        Transform transform = field.transform;
        Vector2 fieldPosition = transform.position;

        Position = new int[2];
        Position[0] = (int)fieldPosition.x;
        Position[1] = (int)fieldPosition.y;
        Rotation = (int)transform.rotation.eulerAngles.z;

        FieldMode = field.FieldMode.ToString();
    }

    public override void ImportToLevel() => ImportToLevel(new(Position[0], Position[1]));

    public override void ImportToLevel(Vector2 pos)
    {
        FieldMode mode = EditModeManager.GetFieldMode(FieldMode);

        FieldManager.Instance.SetField(Vector2Int.RoundToInt(pos), mode, Rotation);
    }

    public override EditMode GetEditMode() => (EditMode)Enum.Parse(typeof(EditMode), FieldMode);
    
    public override bool Equals(Data d)
    {
        FieldData other = (FieldData)d;
        return other.Position[0] == Position[0]
               && other.Position[1] == Position[1]
               && other.FieldMode == FieldMode
               && other.Rotation == Rotation;
    }
}