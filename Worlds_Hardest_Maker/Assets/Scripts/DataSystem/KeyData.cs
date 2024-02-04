using System;
using UnityEngine;

/// <summary>
///     Key attributes: position, color
/// </summary>
[Serializable]
public class KeyData : Data
{
    public float[] Position;
    public KeyColor Color;

    public KeyData(KeyController controller)
    {
        Vector2 keyPosition = controller.transform.position;

        Position = new float[2];
        Position[0] = keyPosition.x;
        Position[1] = keyPosition.y;
        Color = controller.Color;
    }

    public override void ImportToLevel(Vector2 pos) => KeyManager.Instance.SetKey(pos, Color);

    public override void ImportToLevel() => ImportToLevel(new(Position[0], Position[1]));

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