using System;
using UnityEngine;

/// <summary>
///     Player attributes: speed, start pos
/// </summary>
[Serializable]
public class PlayerData : AttachableData
{
    public float[] StartPosition;
    
    public PlayerData(PlayerController controller)
    {
        StartPosition = new float[2];
        StartPosition[0] = controller.StartPos.x;
        StartPosition[1] = controller.StartPos.y;
    }
    
    public override void ImportToLevel(AnchorController sheet)
    {
        Vector2 position = new(StartPosition[0], StartPosition[1]);
        
        PlayerManager.Instance.CreateNew(position, sheet, false);
    }
    
    public override void ImportToLevel(Vector2 pos) => PlayerManager.Instance.Set(pos);
    
    public override EditMode GetEditMode() => EditModeManager.Player;
    
    public override bool Equals(Data d)
    {
        PlayerData other = (PlayerData)d;
        return other.StartPosition[0] == StartPosition[0] && other.StartPosition[1] == StartPosition[1];
    }
}