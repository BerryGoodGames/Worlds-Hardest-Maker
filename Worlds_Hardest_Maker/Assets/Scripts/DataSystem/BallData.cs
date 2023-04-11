using System;
using UnityEngine;

/// <summary>
///     Ball attributes: speed, start, bounce
/// </summary>
[Serializable]
public class BallData : Data
{
    public float Speed;

    public float[] StartPosition;

    public float[] BouncePosition;

    public BallData(BallDefaultController defaultController)
    {
        Speed = defaultController.Speed;

        StartPosition = new float[2];
        StartPosition[0] = defaultController.StartPosition.x;
        StartPosition[1] = defaultController.StartPosition.y;

        BouncePosition = new float[2];
        BouncePosition[0] = defaultController.Bounce.position.x;
        BouncePosition[1] = defaultController.Bounce.position.y;
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(StartPosition[0], StartPosition[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        float[] ballPos = StartPosition;
        float[] bouncePos = { BouncePosition[0] - ballPos[0], BouncePosition[1] - ballPos[1] };

        BallManager.Instance.SetBall(pos.x, pos.y,
            bouncePos[0], bouncePos[1],
            Speed
        );
    }

    public override EditMode GetEditMode() => EditMode.BALL_DEFAULT;
}