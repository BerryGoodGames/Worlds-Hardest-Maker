using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class WaypointOld
{
    [FormerlySerializedAs("position")] public Vector2 Position;
    [FormerlySerializedAs("speed")] public float Speed;
    [FormerlySerializedAs("delay")] public float Delay;

    [FormerlySerializedAs("rotationSpeed")]
    public float RotationSpeed;

    [FormerlySerializedAs("rotateWhileDelay")]
    public bool RotateWhileDelay;

    public WaypointEditorControllerOld WaypointEditor { get; set; }

    public WaypointOld(Vector2 position, bool rotateWhileDelay, float delay, float speed, float rotationSpeed)
    {
        Position = position;
        Delay = delay;
        RotateWhileDelay = rotateWhileDelay;
        Speed = speed;
        RotationSpeed = rotationSpeed;
    }

    public WaypointOld(WaypointSerializable waypoint)
    {
        Position = new(waypoint.Position[0], waypoint.Position[1]);

        Speed = waypoint.Speed;
        Delay = waypoint.Delay;
        RotationSpeed = waypoint.RotationSpeed;
        RotateWhileDelay = waypoint.RotateWhileDelay;
    }

    public override string ToString() =>
        $"position: {Position}, speed: {Speed}, delay: {Delay}, rotationSpeed: {RotationSpeed}, rotateWhileDelay {RotateWhileDelay}";

    public WaypointOld Clone() => new(Position, RotateWhileDelay, Delay, Speed, RotationSpeed);
}


[Serializable]
public class WaypointSerializable
{
    [FormerlySerializedAs("position")] public float[] Position = new float[2];
    [FormerlySerializedAs("speed")] public float Speed;
    [FormerlySerializedAs("delay")] public float Delay;

    [FormerlySerializedAs("rotationSpeed")]
    public float RotationSpeed;

    [FormerlySerializedAs("rotateWhileDelay")]
    public bool RotateWhileDelay;

    public WaypointSerializable(WaypointOld waypointOld)
    {
        Position[0] = waypointOld.Position.x;
        Position[1] = waypointOld.Position.y;
        Speed = waypointOld.Speed;
        Delay = waypointOld.Delay;
        RotationSpeed = waypointOld.RotationSpeed;
        RotateWhileDelay = waypointOld.RotateWhileDelay;
    }
}