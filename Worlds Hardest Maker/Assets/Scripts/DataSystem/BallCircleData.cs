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
    public int radius;
    public int[] originPosition;
    public float angle;

    public BallCircleData(BallCircleController controller)
    {
        speed = controller.speed;

        radius = controller.radius;

        originPosition = new int[2];
        originPosition[0] = (int)controller.GetOriginPos().x;
        originPosition[1] = (int)controller.GetOriginPos().y;

        angle = controller.startAngle;
    }

    public override void CreateObject()
    {
        BallCircleManager.SetBallCircle(originPosition[0], originPosition[1], radius, speed, angle);
    }
}
