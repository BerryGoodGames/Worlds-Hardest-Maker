using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player attributes: speed, start pos
/// </summary>
[System.Serializable]
public class PlayerData : IData
{
    public int Id;
    public float Speed;
    public float[] StartPosition;

    public PlayerData(PlayerController controller)
    {
        Id = controller.id;
        Speed = controller.speed;

        StartPosition = new float[2];
        StartPosition[0] = controller.startPos.x;
        StartPosition[1] = controller.startPos.y;
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(StartPosition[0], StartPosition[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        PlayerManager.Instance.SetPlayer(pos, Speed);
    }

    public override EditMode GetEditMode()
    {
        return EditMode.PLAYER;
    }
}
