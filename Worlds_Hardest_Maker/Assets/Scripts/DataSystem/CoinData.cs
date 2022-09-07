using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coin attributes: position
/// </summary>
[System.Serializable]
public class CoinData : IData
{
    public float[] position;

    public CoinData(CoinController controller)
    {
        position = new float[2];
        position[0] = controller.transform.position.x;
        position[1] = controller.transform.position.y;
    }

    public override void CreateObject()
    {
        CoinManager.Instance.SetCoin(position[0], position[1]);
    }
}
