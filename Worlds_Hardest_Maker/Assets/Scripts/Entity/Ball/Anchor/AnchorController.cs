using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// controlles the anchor (duh)
/// </summary>
public class AnchorController : Controller
{
    public GameObject outline;
    public GameObject container;

    private PathController pathController;
    public PhotonView View { get; set; }

    private void Start()
    {

        transform.localPosition = transform.parent.position;

        transform.parent.position = Vector2.zero;

        transform.parent.SetParent(ReferenceManager.Instance.AnchorContainer);

        pathController = GetComponent<PathController>();

        View = PhotonView.Get(this);
    }
    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    [PunRPC]
    private void RPCSetBall(Vector2 pos)
    {
        AnchorBallManager.SetAnchorBall(pos, container.transform);
    }

    [PunRPC]
    private void RPCSetWaypointPosition(Vector2 pos, int index)
    {
        Waypoint waypoint = pathController.waypoints[index];
        waypoint.position = pos;
        AnchorManager.Instance.selectedPathController.DrawLines();
        if (waypoint.WaypointEditor != null)
            waypoint.WaypointEditor.InputPosition = pos;
    }

    [PunRPC]
    private void RPCSetWaypointSpeed(float speed, int index)
    {
        Waypoint waypoint = pathController.waypoints[index];
        waypoint.speed = speed;

        if (waypoint.WaypointEditor != null)
            waypoint.WaypointEditor.InputSpeed = speed;
    }

    [PunRPC]
    private void RPCSetWaypointRotationSpeed(float speed, int index)
    {
        Waypoint waypoint = pathController.waypoints[index];
        waypoint.rotationSpeed = speed;

        if (waypoint.WaypointEditor != null)
            waypoint.WaypointEditor.InputRotationSpeed = speed;
    }

    [PunRPC]
    private void RPCSetWaypointDelay(float delay, int index)
    {
        Waypoint waypoint = pathController.waypoints[index];
        waypoint.delay = delay;

        if (waypoint.WaypointEditor != null)
            waypoint.WaypointEditor.InputDelay = delay;
    }

    [PunRPC]
    private void RPCSetWaypointRotateWhileDelay(bool rotateWhileDelay, int index)
    {
        Waypoint waypoint = pathController.waypoints[index];
        waypoint.rotateWhileDelay = rotateWhileDelay;

        if (waypoint.WaypointEditor != null)
            waypoint.WaypointEditor.InputRotateWhileDelay = rotateWhileDelay;
    }

    [PunRPC]
    private void RPCDeleteWaypoint(int index)
    {
        Waypoint waypoint = pathController.waypoints[index];
        waypoint.WaypointEditor.DeleteThisWaypoint();

    }

    [PunRPC]
    private void RPCAddWaypoint()
    {
        pathController.waypoints.Add(new(new(0, 0), true, 0, 1, 0));
        if (pathController.waypoints.Count > 0 && pathController.waypoints[0].WaypointEditor != null)
            ReferenceManager.Instance.BallWindows.GetComponentInChildren<PathEditorController>().UpdateUI();
    }

    public void BallFadeOut(AnimationEvent animationEvent)
    {
        float endOpacity = animationEvent.floatParameter;
        if (float.TryParse(animationEvent.stringParameter, out float time))
            StartCoroutine(container.GetComponent<ChildrenOpacity>().FadeOut(endOpacity, time));
    }

    public void BallFadeIn(AnimationEvent animationEvent)
    {
        float endOpacity = animationEvent.floatParameter;
        if (float.TryParse(animationEvent.stringParameter, out float time))
            BallFadeIn(endOpacity, time);
    }

    public void BallFadeIn(float endOpacity, float time)
    {
        StartCoroutine(container.GetComponent<ChildrenOpacity>().FadeIn(endOpacity, time));
    }

    public IEnumerator FadeInOnNextFrame(float endOpacity, float time)
    {
        yield return null;
        BallFadeIn(endOpacity, time);
        yield break;
    }

    public override IData GetData()
    {
        return new AnchorData(pathController, container.transform);
    }
}
