using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     anchor attributes: position, waypoints, mode, ball positions
/// </summary>
[Serializable]
public class AnchorDataOld : Data
{
    public float[] position = new float[2];
    public WaypointSerializable[] waypoints;
    public PathControllerOld.PathMode pathMode;
    public float[] ballPositions;

    public AnchorDataOld(PathControllerOld pathControllerOld, Transform ballContainer)
    {
        position[0] = pathControllerOld.transform.position.x;
        position[1] = pathControllerOld.transform.position.y;
        pathMode = pathControllerOld.pathMode;

        // convert Waypoints
        List<WaypointSerializable> waypointsList = new();

        foreach (WaypointOld waypoint in pathControllerOld.waypoints)
        {
            waypointsList.Add(new(waypoint));
        }

        waypoints = waypointsList.ToArray();

        // convert balls (hihi)
        List<float> ballPositionsList = new();

        foreach (Transform ball in ballContainer)
        {
            Transform child = ball.GetChild(0);
            ballPositionsList.Add(child.position.x);
            ballPositionsList.Add(child.position.y);
        }

        ballPositions = ballPositionsList.ToArray();
    }

    public override void ImportToLevel(Vector2 pos)
    {
        // create object
        GameObject anchor = AnchorManagerOld.Instance.SetAnchor(pos);
        if (anchor == null) return;
        PathControllerOld pathControllerOld = anchor.GetComponentInChildren<PathControllerOld>();

        // set waypoints
        pathControllerOld.waypoints.Clear();

        foreach (WaypointSerializable waypoint in waypoints)
        {
            pathControllerOld.waypoints.Add(new(waypoint));
        }

        // set path mode
        pathControllerOld.pathMode = pathMode;

        // reset state
        pathControllerOld.ResetState();


        // set balls (hihi)
        AnchorControllerOld anchorControllerOld = anchor.GetComponentInChildren<AnchorControllerOld>();
        Transform container = anchorControllerOld.container.transform;

        for (int i = 0; i < ballPositions.Length; i += 2)
        {
            AnchorBallManagerOld.SetAnchorBall(ballPositions[i], ballPositions[i + 1], container);
        }

        // fade balls in
        anchorControllerOld.StartCoroutine(anchorControllerOld.FadeInOnNextFrame(1, 0.1f));
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(position[0], position[1]));
    }

    public override EditMode GetEditMode()
    {
        return EditMode.ANCHOR;
    }
}