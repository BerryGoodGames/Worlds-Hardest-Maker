using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player attributes: speed, start pos
/// </summary>
[System.Serializable]
public class PlayerData : IData
{
    public float speed;
    public int[] startPosition;

    public PlayerData(PlayerController controller)
    {
        speed = controller.speed;

        Vector2 startPos = (Vector2)GameManager.Instance.PlayerStartPos;

        startPosition = new int[2];
        startPosition[0] = (int)startPos.x;
        startPosition[1] = (int)startPos.y;
    }

    public override void CreateObject()
    {
        PlayerManager.SetPlayer(startPosition[0], startPosition[1], speed);
    }
}
