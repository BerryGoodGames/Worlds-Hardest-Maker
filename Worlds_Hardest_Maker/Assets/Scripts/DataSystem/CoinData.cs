using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coin attributes: position
/// </summary>
[System.Serializable]
public class CoinData : IData
{
    public float[] Position;

    public CoinData(CoinController controller)
    {
        Position = new float[2];
        Position[0] = controller.transform.position.x;
        Position[1] = controller.transform.position.y;
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(Position[0], Position[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        CoinManager.Instance.SetCoin(pos);
    }

    public override EditMode GetEditMode()
    {
        return EditMode.COIN;
    }
}
