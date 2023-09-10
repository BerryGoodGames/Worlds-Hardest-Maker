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

        Vector2 controllerStartPosition = defaultController.StartPosition;
        StartPosition = new float[2];
        StartPosition[0] = controllerStartPosition.x;
        StartPosition[1] = controllerStartPosition.y;

        Vector2 controllerBouncePosition = defaultController.Bounce.position;
        BouncePosition = new float[2];
        BouncePosition[0] = controllerBouncePosition.x;
        BouncePosition[1] = controllerBouncePosition.y;
    }

    public override void ImportToLevel() => ImportToLevel(new(StartPosition[0], StartPosition[1]));

    public override void ImportToLevel(Vector2 pos)
    {
        float[] ballPos = StartPosition;
        float[] bouncePos = { BouncePosition[0] - ballPos[0], BouncePosition[1] - ballPos[1] };

        BallManager.Instance.SetBall(pos.x, pos.y,
            bouncePos[0], bouncePos[1],
            Speed
        );
    }

    public override EditMode GetEditMode() => EditMode.BallDefault;
}