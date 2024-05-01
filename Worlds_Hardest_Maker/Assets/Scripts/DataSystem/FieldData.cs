using System;
using System.Windows.Forms;
using UnityEngine;

/// <summary>
///     Field attributes: position, type
/// </summary>
[Serializable]
public class FieldData : AttachableData
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
        Rotation = 90 * Mathf.RoundToInt(transform.rotation.eulerAngles.z / 90);
        
        FieldMode = field.FieldMode.ToString();
    }
    
    public override void ImportToLevel(AnchorController sheet)
    {
        ManagerParameters args = new()
        {
            Position = new(Position[0], Position[1]),
            FieldMode = EditModeManager.GetFieldMode(FieldMode),
            Rotation = Rotation,
            Sheet = sheet,
        };
        
        ((IManager<FieldController>)FieldManager.Instance).SetInSheet(args);
    }
    
    public override void ImportToLevel(Vector2 pos)
    {
        ManagerParameters args = new()
        {
            Position = Vector2Int.RoundToInt(pos),
            FieldMode = EditModeManager.GetFieldMode(FieldMode),
            Rotation = Rotation,
        };
        
        ((IManager<FieldController>)FieldManager.Instance).Set(args);
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