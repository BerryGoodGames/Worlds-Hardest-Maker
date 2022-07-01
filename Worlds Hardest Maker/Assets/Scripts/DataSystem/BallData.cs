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
    public int[] startPosition;
    public int[] bouncePosition;
    
    public BallData(BallController controller)
    {
        speed = controller.speed;
        
        startPosition = new int[2];
        startPosition[0] = (int)controller.startPosition.x;
        startPosition[1] = (int)controller.startPosition.y;

        bouncePosition = new int[2];
        bouncePosition[0] = (int)controller.GetBouncePos().x;
        bouncePosition[1] = (int)controller.GetBouncePos().y;
    }

    public override void CreateObject()
    {
        int[] ballPos = startPosition;
        int[] bouncePos = { bouncePosition[0] - ballPos[0], bouncePosition[1] - ballPos[1] };

        BallManager.SetBall(startPosition[0], startPosition[1],
            bouncePos[0], bouncePos[1],
            speed);
    }
}
