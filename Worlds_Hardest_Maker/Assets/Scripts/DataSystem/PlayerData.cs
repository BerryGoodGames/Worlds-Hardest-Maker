using System;
using UnityEngine;

/// <summary>
///     Player attributes: speed, start pos
/// </summary>
[Serializable]
public class PlayerData : Data
{
    public int id;
    public float speed;
    public float[] startPosition;

    public PlayerData(PlayerController controller)
    {
        id = controller.id;
        speed = controller.speed;

        startPosition = new float[2];
        startPosition[0] = controller.startPos.x;
        startPosition[1] = controller.startPos.y;
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(startPosition[0], startPosition[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        PlayerManager.Instance.SetPlayer(pos, speed);
    }

    public override EditMode GetEditMode()
    {
        return EditMode.PLAYER;
    }
}