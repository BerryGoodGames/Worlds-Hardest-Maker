using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     BallCircle attributes: speed, radius, origin, angle
/// </summary>
[Serializable]
public class BallCircleData : Data
{
    public float Speed;
    public float Radius;

    public float[] OriginPosition;

    public float Angle;

    public BallCircleData(BallCircleController controller)
    {
        Speed = controller.Speed;

        Radius = controller.Radius;

        Vector2 originPos = controller.Origin.position;
        OriginPosition = new float[2];
        OriginPosition[0] = originPos.x;
        OriginPosition[1] = originPos.y;

        Angle = controller.StartAngle;
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