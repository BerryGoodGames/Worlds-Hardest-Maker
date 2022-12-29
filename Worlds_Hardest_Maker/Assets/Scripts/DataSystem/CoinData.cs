using System;
using UnityEngine;

/// <summary>
///     Coin attributes: position
/// </summary>
[Serializable]
public class CoinData : IData
{
    public float[] position;

    public CoinData(CoinController controller)
    {
        position = new float[2];
        position[0] = controller.transform.position.x;
        position[1] = controller.transform.position.y;
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(position[0], position[1]));
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