using System;
using UnityEngine;

[Serializable]
public class Waypoint
{
    public Vector2 position = Vector2.zero;
    public float speed = 1;
    public float delay;
    public float rotationSpeed;
    public bool rotateWhileDelay = true;
    public WaypointEditorController WaypointEditor { get; set; }

    public Waypoint(Vector2 position, bool rotateWhenDelay, float delay, float speed, float rotationSpeed)
    {
        this.position = position;
        this.delay = delay;
        this.rotateWhileDelay = rotateWhenDelay;
        this.speed = speed;
        this.rotationSpeed = rotationSpeed;
    }

    public Waypoint(WaypointSerializable waypoint)
    {
        this.position = new(waypoint.position[0], waypoint.position[1]);
        this.speed = waypoint.speed;
        this.delay = waypoint. delay;
        this.rotationSpeed = waypoint.rotationSpeed;
        this.rotateWhileDelay = waypoint.rotateWhileDelay;
    }
}


[Serializable]
public class WaypointSerializable
{
    public float[] position = new float[2];
    public float speed;
    public float delay;
    public float rotationSpeed;
    public bool rotateWhileDelay = true;

    public WaypointSerializable(Waypoint waypoint)
    {
        this.position[0] = waypoint.position.x;
        this.position[1] = waypoint.position.y;
        this.speed = waypoint.speed;
        this.delay = waypoint.delay;
        this.rotationSpeed = waypoint.rotationSpeed;
        this.rotateWhileDelay = waypoint.rotateWhileDelay;
    }
}