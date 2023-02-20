using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Key attributes: position, color
/// </summary>
[Serializable]
public class KeyData : Data
{
    [FormerlySerializedAs("position")] public float[] Position;
    [FormerlySerializedAs("color")] public KeyManager.KeyColor Color;

    public KeyData(KeyController controller)
    {
        Position = new float[2];
        Position[0] = controller.transform.position.x;
        Position[1] = controller.transform.position.y;
        Color = controller.Color;
    }

    public override void ImportToLevel(Vector2 pos)
    {
        KeyManager.Instance.SetKey(pos, Color);
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(Position[0], Position[1]));
    }

    public override EditMode GetEditMode()
    {
        return Color switch
        {
            KeyManager.KeyColor.GRAY => EditMode.GRAY_KEY,
            KeyManager.KeyColor.RED => EditMode.RED_KEY,
            KeyManager.KeyColor.GREEN => EditMode.GREEN_KEY,
            KeyManager.KeyColor.BLUE => EditMode.BLUE_KEY,
            KeyManager.KeyColor.YELLOW => EditMode.YELLOW_KEY,
            _ => EditMode.GRAY_KEY
        };
    }
}