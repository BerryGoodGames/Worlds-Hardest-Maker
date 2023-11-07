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

    public override EditMode GetEditMode() => EditMode.AnchorBall;
}