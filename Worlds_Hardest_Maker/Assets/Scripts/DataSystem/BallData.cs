using System;
using UnityEngine;

/// <summary>
///     Ball attributes: speed, start, bounce
/// </summary>
[Serializable]
public class BallData : Data
{
    public float speed;
    public float[] startPosition;
    public float[] bouncePosition;

    public BallData(BallDefaultController defaultController)
    {
        speed = defaultController.speed;

        startPosition = new float[2];
        startPosition[0] = defaultController.startPosition.x;
        startPosition[1] = defaultController.startPosition.y;

        bouncePosition = new float[2];
        bouncePosition[0] = defaultController.bounce.position.x;
        bouncePosition[1] = defaultController.bounce.position.y;
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(startPosition[0], startPosition[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        float[] ballPos = startPosition;
        float[] bouncePos = { bouncePosition[0] - ballPos[0], bouncePosition[1] - ballPos[1] };

        BallManager.Instance.SetBall(pos.x, pos.y,
            bouncePos[0], bouncePos[1],
            speed
        );
    }

    public override EditMode GetEditMode()
    {
        return EditMode.BALL_DEFAULT;
    }
}