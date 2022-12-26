using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BallCircle attributes: speed, radius, origin, angle
/// </summary>
[System.Serializable]
public class BallCircleData : IData
{
    public float Speed;
    public float Radius;
    public float[] OriginPosition;
    public float Angle;

    public BallCircleData(BallCircleController controller)
    {
        Speed = controller.speed;

        Radius = controller.radius;

        OriginPosition = new float[2];
        OriginPosition[0] = controller.origin.position.x;
        OriginPosition[1] = controller.origin.position.y;

        Angle = controller.startAngle;
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(OriginPosition[0], OriginPosition[1]));
    }

    public override void ImportToLevel(Vector2 pos)
    {
        BallCircleManager.Instance.SetBallCircle(pos.x, pos.y, Radius, Speed, Angle);
    }

    public override EditMode GetEditMode()
    {
        return EditMode.BALL_CIRCLE;
    }
}
