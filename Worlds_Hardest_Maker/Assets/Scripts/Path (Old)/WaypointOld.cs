using System;
using UnityEngine;

[Serializable]
public class WaypointOld
{
    public Vector2 position;
    public float speed;
    public float delay;
    public float rotationSpeed;
    public bool rotateWhileDelay;
    public WaypointEditorControllerOld WaypointEditor { get; set; }

    public WaypointOld(Vector2 position, bool rotateWhileDelay, float delay, float speed, float rotationSpeed)
    {
        this.position = position;
        this.delay = delay;
        this.rotateWhileDelay = rotateWhileDelay;
        this.speed = speed;
        this.rotationSpeed = rotationSpeed;
    }

    public WaypointOld(WaypointSerializable waypoint)
    {
        position = new(waypoint.position[0], waypoint.position[1]);

        speed = waypoint.speed;
        delay = waypoint.delay;
        rotationSpeed = waypoint.rotationSpeed;
        rotateWhileDelay = waypoint.rotateWhileDelay;
    }

    public override string ToString()
    {
        return
            $"position: {position}, speed: {speed}, delay: {delay}, rotationSpeed: {rotationSpeed}, rotateWhileDelay {rotateWhileDelay}";
    }

    public WaypointOld Clone()
    {
        return new(position, rotateWhileDelay, delay, speed, rotationSpeed);
    }
}


[Serializable]
public class WaypointSerializable
{
    public float[] position = new float[2];
    public float speed;
    public float delay;
    public float rotationSpeed;
    public bool rotateWhileDelay;

    public WaypointSerializable(WaypointOld waypointOld)
    {
        position[0] = waypointOld.position.x;
        position[1] = waypointOld.position.y;
        speed = waypointOld.speed;
        delay = waypointOld.delay;
        rotationSpeed = waypointOld.rotationSpeed;
        rotateWhileDelay = waypointOld.rotateWhileDelay;
    }
}