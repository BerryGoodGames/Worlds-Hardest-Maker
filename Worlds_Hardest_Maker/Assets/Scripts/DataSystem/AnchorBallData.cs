using System;
using UnityEngine;

[Serializable]
public class AnchorBallData : Data
{
    private readonly float[] position;

    public AnchorBallData(Vector3 ballPosition) =>
        position = new[]
        {
            ballPosition.x,
            ballPosition.y,
        };

    public override void ImportToLevel(Vector2 pos) => AnchorBallManager.SetAnchorBall(pos);

    public override void ImportToLevel() => AnchorBallManager.SetAnchorBall(new(position[0], position[1]));

    public void ImportToLevel(AnchorController anchor) => AnchorBallManager.SetAnchorBall(new(position[0], position[1]), anchor);

    public override EditMode GetEditMode() => EditModeManager.AnchorBall;
    
    public override bool Equals(Data d)
    {
        AnchorBallData other = (AnchorBallData)d;
        return other.position[0] == position[0] && other.position[1] == position[1];
    }
}