using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player attributes: speed, start pos
/// </summary>
[System.Serializable]
public class PlayerData : IData
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
        PlayerManager.Instance.SetPlayer(startPosition[0], startPosition[1], speed);
    }
}
