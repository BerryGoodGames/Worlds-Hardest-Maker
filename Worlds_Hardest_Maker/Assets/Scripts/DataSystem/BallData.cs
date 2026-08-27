using System;
using UnityEngine;

[Serializable]
public class BallData : AttachableData
{
    private readonly float[] position;
    
    public BallData(Vector3 ballPosition) =>
        position = new[]
        {
            ballPosition.x,
            ballPosition.y,
        };
    
    public override void ImportToLevel(Vector2 pos)
    {
        BallManager.Instance.CreateNew(pos, null);
    }
    
    public override void ImportToLevel(ISheet sheet)
    {
        Vector2 position = new(this.position[0], this.position[1]);
        
        BallManager.Instance.CreateNew(position, sheet);
    }
    
    public override EditMode GetEditMode() => EditModeManager.Ball;
    
    public override bool Equals(Data d)
    {
        BallData other = (BallData)d;
        return other.position[0] == position[0] && other.position[1] == position[1];
    }
}