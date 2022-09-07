using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ball attributes: speed, start, bounce
/// </summary>
[System.Serializable]
public class BallData : IData
{
    public float speed;
    public float[] startPosition;
    public float[] bouncePosition;
    
    public BallData(BallController controller)
    {
        speed = controller.speed;
        
        startPosition = new float[2];
        startPosition[0] = controller.startPosition.x;
        startPosition[1] = controller.startPosition.y;

        bouncePosition = new float[2];
        bouncePosition[0] = controller.bounce.position.x;
        bouncePosition[1] = controller.bounce.position.y;
    }

    public override void CreateObject()
    {
        float[] ballPos = startPosition;
        float[] bouncePos = { bouncePosition[0] - ballPos[0], bouncePosition[1] - ballPos[1] };

        BallManager.Instance.SetBall(startPosition[0], startPosition[1],
            bouncePos[0], bouncePos[1],
            speed
        );
    }
}
