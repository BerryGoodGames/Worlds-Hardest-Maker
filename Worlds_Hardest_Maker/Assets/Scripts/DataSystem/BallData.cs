using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ball attributes: speed, start, bounce
/// </summary>
[System.Serializable]
public class BallData : IData
{
    public float Speed;
    public float[] StartPosition;
    public float[] BouncePosition;
    
    public BallData(BallController controller)
    {
        Speed = controller.speed;
        
        StartPosition = new float[2];
        StartPosition[0] = controller.startPosition.x;
        StartPosition[1] = controller.startPosition.y;

        BouncePosition = new float[2];
        BouncePosition[0] = controller.bounce.position.x;
        BouncePosition[1] = controller.bounce.position.y;
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

    public override EditMode GetEditMode()
    {
        return EditMode.BALL_DEFAULT;
    }
}
