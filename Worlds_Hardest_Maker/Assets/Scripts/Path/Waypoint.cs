using System;
using UnityEngine;

[Serializable]
public class Waypoint
{
    public Vector2 position;
    public float speed;
    public float delay;
    public float rotationSpeed;
    public bool rotateWhileDelay;
    public WaypointEditorController WaypointEditor { get; set; }

    public Waypoint(Vector2 position, bool rotateWhileDelay, float delay, float speed, float rotationSpeed)
    {
        this.position = position;
        this.delay = delay;
        this.rotateWhileDelay = rotateWhileDelay;
        this.speed = speed;
        this.rotationSpeed = rotationSpeed;
    }

    public Waypoint(WaypointSerializable waypoint)
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

    public Waypoint Clone()
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

    public WaypointSerializable(Waypoint waypoint)
    {
        position[0] = waypoint.position.x;
        position[1] = waypoint.position.y;
        speed = waypoint.speed;
        delay = waypoint.delay;
        rotationSpeed = waypoint.rotationSpeed;
        rotateWhileDelay = waypoint.rotateWhileDelay;
    }
}