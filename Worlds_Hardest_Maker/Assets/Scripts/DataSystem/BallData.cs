using System;
using UnityEngine;

[Serializable]
public class BallData : Data
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
        ManagerParameters args = new() { Position = pos, };
        BallManager.Instance.SetInSheet(args);
    }
    
    public override void ImportToLevel()
    {
        ManagerParameters args = new() { Position = new(position[0], position[1]), };
        BallManager.Instance.SetInSheet(args);
    }
    
    public override EditMode GetEditMode() => EditModeManager.Ball;
    
    public override bool Equals(Data d)
    {
        BallData other = (BallData)d;
        return other.position[0] == position[0] && other.position[1] == position[1];
    }
}