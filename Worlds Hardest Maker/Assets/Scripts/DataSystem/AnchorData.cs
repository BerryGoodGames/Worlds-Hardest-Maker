using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// anchor attributes: position, waypoints, mode, ball positions
/// </summary>

[System.Serializable]
public class AnchorData : IData
{
    public float[] position = new float[2];
    public WaypointSerializable[] waypoints;
    public PathController.PathMode pathMode;
    public float[] ballPositions;

    public AnchorData(PathController pathController, Transform ballContainer)
    {
        position[0] = pathController.transform.position.x;
        position[1] = pathController.transform.position.y;
        pathMode = pathController.pathMode;

        // convert Waypoints
        List<WaypointSerializable> waypointsList = new();

        foreach (Waypoint waypoint in pathController.waypoints)
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

    public override void CreateObject()
    {
        // create object
        GameObject anchor = AnchorManager.Instance.SetAnchor(position[0], position[1]);
        PathController pathController = anchor.GetComponentInChildren<PathController>();

        // set waypoints
        pathController.waypoints.Clear();

        foreach (WaypointSerializable waypoint in waypoints)
        {
            pathController.waypoints.Add(new(waypoint));
        }

        // set path mode
        pathController.pathMode = pathMode;


        // set balls (hihi)
        Transform container = anchor.GetComponentInChildren<AnchorController>().container.transform;

        for (int i = 0; i < ballPositions.Length; i += 2)
        {
            AnchorBallManager.SetAnchorBall(ballPositions[i], ballPositions[i + 1], container);
        }
    }
}
