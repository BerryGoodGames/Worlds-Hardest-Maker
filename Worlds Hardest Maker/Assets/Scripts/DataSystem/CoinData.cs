using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coin attributes: position
/// </summary>
[System.Serializable]
public class CoinData : IData
{
    public int[] position;

    public CoinData(CoinController controller)
    {
        position = new int[2];
        position[0] = (int)controller.transform.position.x;
        position[1] = (int)controller.transform.position.y;
    }

    public override void CreateObject()
    {
        CoinManager.SetCoin(position[0], position[1]);
    }
}
