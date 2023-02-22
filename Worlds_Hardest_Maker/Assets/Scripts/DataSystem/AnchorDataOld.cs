using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     anchor attributes: position, waypoints, mode, ball positions
/// </summary>
[Serializable]
public class AnchorDataOld : Data
{
    [FormerlySerializedAs("position")] public float[] Position = new float[2];
    [FormerlySerializedAs("waypoints")] public WaypointSerializable[] Waypoints;
    [FormerlySerializedAs("pathMode")] public PathControllerOld.PathModeType PathMode;

    [FormerlySerializedAs("ballPositions")]
    public float[] BallPositions;

    public AnchorDataOld(PathControllerOld pathControllerOld, Transform ballContainer)
    {
        Position[0] = pathControllerOld.transform.position.x;
        Position[1] = pathControllerOld.transform.position.y;
        PathMode = pathControllerOld.PathMode;

        // convert Waypoints
        List<WaypointSerializable> waypointsList = new();

        foreach (WaypointOld waypoint in pathControllerOld.Waypoints)
        {
            waypointsList.Add(new(waypoint));
        }

        Waypoints = waypointsList.ToArray();

        // convert balls (hihi)
        List<float> ballPositionsList = new();

        foreach (Transform ball in ballContainer)
        {
            Transform child = ball.GetChild(0);
            ballPositionsList.Add(child.position.x);
            ballPositionsList.Add(child.position.y);
        }

        BallPositions = ballPositionsList.ToArray();
    }

    public override void ImportToLevel(Vector2 pos)
    {
        // create object
        GameObject anchor = AnchorManagerOld.Instance.SetAnchor(pos);
        if (anchor == null) return;
        PathControllerOld pathControllerOld = anchor.GetComponentInChildren<PathControllerOld>();

        // set waypoints
        pathControllerOld.Waypoints.Clear();

        foreach (WaypointSerializable waypoint in Waypoints)
        {
            pathControllerOld.Waypoints.Add(new(waypoint));
        }

        // set path mode
        pathControllerOld.PathMode = PathMode;

        // reset state
        pathControllerOld.ResetState();


        // set balls (hihi)
        AnchorControllerOld anchorControllerOld = anchor.GetComponentInChildren<AnchorControllerOld>();
        Transform container = anchorControllerOld.Container.transform;

        for (int i = 0; i < BallPositions.Length; i += 2)
        {
            AnchorBallManagerOld.SetAnchorBall(BallPositions[i], BallPositions[i + 1], container);
        }

        // fade balls in
        anchorControllerOld.StartCoroutine(anchorControllerOld.FadeInOnNextFrame(1, 0.1f));
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(Position[0], Position[1]));
    }

    public override EditMode GetEditMode()
    {
        return EditMode.ANCHOR;
    }
}