using System;
using UnityEngine;

[Serializable]
public class AnchorBallData : Data
{
    private readonly float[] position;

    public AnchorBallData(Vector3 ballPosition)
    {
        position = new[]
        {
            ballPosition.x, 
            ballPosition.y
        };
    }

    public override void ImportToLevel() => AnchorBallManager.SetAnchorBall(position[0], position[1]);

    public override EditMode GetEditMode() => EditMode.AnchorBall;
}
