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

    public override void ImportToLevel(Vector2 pos)
    {
        // create object
        GameObject anchor = AnchorManager.Instance.SetAnchor(pos);
        if (anchor == null) return;
        PathController pathController = anchor.GetComponentInChildren<PathController>();

        // set waypoints
        pathController.waypoints.Clear();

        foreach (WaypointSerializable waypoint in waypoints)
        {
            pathController.waypoints.Add(new(waypoint));
        }

        // set path mode
        pathController.pathMode = pathMode;

        // reset state
        pathController.ResetState();


        // set balls (hihi)
        AnchorController anchorController = anchor.GetComponentInChildren<AnchorController>();
        Transform container = anchorController.container.transform;

        for (int i = 0; i < ballPositions.Length; i += 2)
        {
            AnchorBallManager.SetAnchorBall(ballPositions[i], ballPositions[i + 1], container);
        }

        // fade balls in
        anchorController.StartCoroutine(anchorController.FadeInOnNextFrame(1, 0.1f));
    }

    public override void ImportToLevel()
    {
        ImportToLevel(new(position[0], position[1]));
    }
}
