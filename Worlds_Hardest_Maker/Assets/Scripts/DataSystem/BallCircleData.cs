using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BallCircle attributes: speed, radius, origin, angle
/// </summary>
[System.Serializable]
public class BallCircleData : IData
{
    public float speed;
    public float radius;
    public float[] originPosition;
    public float angle;

    public BallCircleData(BallCircleController controller)
    {
        speed = controller.speed;

        radius = controller.radius;

        originPosition = new float[2];
        originPosition[0] = controller.origin.position.x;
        originPosition[1] = controller.origin.position.y;

        angle = controller.startAngle;
    }

    public override void ImportToLevel()
    {
        BallCircleManager.Instance.SetBallCircle(originPosition[0], originPosition[1], radius, speed, angle);
    }
}
