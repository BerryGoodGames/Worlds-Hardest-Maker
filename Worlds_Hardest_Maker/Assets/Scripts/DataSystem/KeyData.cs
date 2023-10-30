using System;
using UnityEngine;

/// <summary>
///     Key attributes: position, color
/// </summary>
[Serializable]
public class KeyData : Data
{
    public float[] Position;
    public KeyManager.KeyColor Color;

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
            KeyManager.KeyColor.Gray => EditMode.GrayKey,
            KeyManager.KeyColor.Red => EditMode.RedKey,
            KeyManager.KeyColor.Green => EditMode.GreenKey,
            KeyManager.KeyColor.Blue => EditMode.BlueKey,
            KeyManager.KeyColor.Yellow => EditMode.YellowKey,
            _ => EditMode.GrayKey,
        };
}