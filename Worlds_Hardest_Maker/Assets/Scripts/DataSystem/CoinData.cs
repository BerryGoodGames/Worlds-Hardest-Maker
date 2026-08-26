using System;
using UnityEngine;

/// <summary>
///     Coin attributes: position
/// </summary>
[Serializable]
public class CoinData : AttachableData
{
    public float[] Position;
    
    public CoinData(CoinController controller)
    {
        Vector2 controllerPosition = controller.InitialPosition;
        
        Position = new float[2];
        Position[0] = controllerPosition.x;
        Position[1] = controllerPosition.y;
    }
    
    public override void ImportToLevel(AnchorController sheet)
    {
        ManagerParameters args = new()
        {
            Position = new(Position[0], Position[1]),
            Sheet = sheet,
        };
        
        CoinManager.Instance.SetInSheet(args);
    }
    
    public override void ImportToLevel(Vector2 pos)
    {
        ManagerParameters args = new() { Position = pos, };
        CoinManager.Instance.SetInSheet(ManagerParameters.FromCurrentSheet(args));
    }
    
    public override EditMode GetEditMode() => EditModeManager.Coin;
    
    public override bool Equals(Data d)
    {
        CoinData other = (CoinData)d;
        return other.Position[0] == Position[0] && other.Position[1] == Position[1];
    }
}