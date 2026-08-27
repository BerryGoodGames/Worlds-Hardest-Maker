using System;
using UnityEngine;

/// <summary>
///     Key attributes: position, color
/// </summary>
[Serializable]
public class KeyData : AttachableData
{
    public float[] Position;
    public KeyColor Color;
    
    public KeyData(KeyController controller)
    {
        Vector2 keyPosition = controller.InitialPosition;
        
        Position = new float[2];
        Position[0] = keyPosition.x;
        Position[1] = keyPosition.y;
        Color = controller.Color;
    }
    
    public override void ImportToLevel(Vector2 pos)
    {
        // TODO: should this be in current sheet instead?
        KeyManager.Instance.CreateNew(pos, null, Color);
    }
    
    public override void ImportToLevel(ISheet sheet)
    {
        Vector2 position = new(Position[0], Position[1]);

        KeyManager.Instance.CreateNew(position, sheet, Color);
    }
    
    public override EditMode GetEditMode() =>
        Color switch
        {
            KeyColor.Gray => EditModeManager.GrayKey,
            KeyColor.Red => EditModeManager.RedKey,
            KeyColor.Green => EditModeManager.GreenKey,
            KeyColor.Blue => EditModeManager.BlueKey,
            KeyColor.Yellow => EditModeManager.YellowKey,
            _ => EditModeManager.GrayKey,
        };
    
    public override bool Equals(Data d)
    {
        KeyData other = (KeyData)d;
        return other.Position[0] == Position[0] && other.Position[1] == Position[1] && other.Color == Color;
    }
}