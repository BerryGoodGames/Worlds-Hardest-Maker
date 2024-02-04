using System;
using UnityEngine;

/// <summary>
///     Coin attributes: position
/// </summary>
[Serializable]
public class CoinData : Data
{
    public float[] Position;

    public CoinData(CoinController controller)
    {
        Vector2 controllerPosition = controller.transform.position;

        Position = new float[2];
        Position[0] = controllerPosition.x;
        Position[1] = controllerPosition.y;
    }

    public override void ImportToLevel() => ImportToLevel(new(Position[0], Position[1]));

    public override void ImportToLevel(Vector2 pos) => CoinManager.SetCoin(pos);

    public override EditMode GetEditMode() => EditModeManager.Coin;
    
    public override bool Equals(Data d)
    {
        CoinData other = (CoinData)d;
        return other.Position[0] == Position[0] && other.Position[1] == Position[1];
    }
}