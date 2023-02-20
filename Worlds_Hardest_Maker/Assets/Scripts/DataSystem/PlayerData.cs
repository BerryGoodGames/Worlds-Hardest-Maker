using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Player attributes: speed, start pos
/// </summary>
[Serializable]
public class PlayerData : Data
{
    [FormerlySerializedAs("id")] public int ID;
    [FormerlySerializedAs("speed")] public float Speed;
    [FormerlySerializedAs("startPosition")] public float[] StartPosition;

    public PlayerData(PlayerController controller)
    {
        ID = controller.ID;
        Speed = controller.Speed;

        StartPosition = new float[2];
        StartPosition[0] = controller.StartPos.x;
        StartPosition[1] = controller.StartPos.y;
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