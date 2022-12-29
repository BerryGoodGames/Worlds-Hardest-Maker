using System;
using UnityEngine;

/// <summary>
///     Key attributes: position, color
/// </summary>
[Serializable]
public class KeyData : IData
{
    public float[] position;
    public KeyManager.KeyColor color;

    public KeyData(KeyController controller)
    {
        position = new float[2];
        position[0] = controller.transform.position.x;
        position[1] = controller.transform.position.y;
        color = controller.color;
    }

    public override void ImportToLevel(Vector2 pos)
    {
        KeyManager.Instance.SetKey(pos, color);
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(position[0], position[1]));
    }

    public override EditMode GetEditMode()
    {
        return color switch
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