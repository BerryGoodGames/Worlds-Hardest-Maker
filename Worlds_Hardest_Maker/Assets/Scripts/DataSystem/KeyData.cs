using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Key attributes: position, color
/// </summary>
[System.Serializable]
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

    public override GameManager.EditMode GetEditMode()
    {
        return color switch
        {
            KeyManager.KeyColor.GRAY => GameManager.EditMode.GRAY_KEY,
            KeyManager.KeyColor.RED => GameManager.EditMode.RED_KEY,
            KeyManager.KeyColor.GREEN => GameManager.EditMode.GREEN_KEY,
            KeyManager.KeyColor.BLUE => GameManager.EditMode.BLUE_KEY,
            KeyManager.KeyColor.YELLOW => GameManager.EditMode.YELLOW_KEY,
            _ => GameManager.EditMode.GRAY_KEY,
        };
    }
}
