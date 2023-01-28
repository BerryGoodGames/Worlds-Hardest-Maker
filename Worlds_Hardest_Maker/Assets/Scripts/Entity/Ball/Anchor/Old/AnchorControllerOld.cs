using System.Collections;
using Photon.Pun;
using UnityEngine;

/// <summary>
///     controls the anchor (duh)
/// </summary>
public class AnchorControllerOld : Controller
{
    public GameObject outline;
    public GameObject container;

    private PathControllerOld pathControllerOld;
    public PhotonView View { get; set; }

    private void Start()
    {
        Transform t = transform;
        Transform parent = t.parent;

        t.localPosition = parent.position;

        parent.position = Vector2.zero;

        parent.SetParent(ReferenceManager.Instance.anchorContainer);

        pathControllerOld = GetComponent<PathControllerOld>();

        View = PhotonView.Get(this);
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    [PunRPC]
    private void RPCSetBall(Vector2 pos)
    {
        AnchorBallManagerOld.SetAnchorBall(pos, container.transform);
    }

    [PunRPC]
    private void RPCSetWaypointPosition(Vector2 pos, int index)
    {
        WaypointOld waypointOld = pathControllerOld.waypoints[index];
        waypointOld.position = pos;
        AnchorManagerOld.Instance.selectedPathControllerOld.DrawLines();
        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputPosition = pos;
    }

    [PunRPC]
    private void RPCSetWaypointSpeed(float speed, int index)
    {
        WaypointOld waypointOld = pathControllerOld.waypoints[index];
        waypointOld.speed = speed;

        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputSpeed = speed;
    }

    [PunRPC]
    private void RPCSetWaypointRotationSpeed(float speed, int index)
    {
        WaypointOld waypointOld = pathControllerOld.waypoints[index];
        waypointOld.rotationSpeed = speed;

        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputRotationSpeed = speed;
    }

    [PunRPC]
    private void RPCSetWaypointDelay(float delay, int index)
    {
        WaypointOld waypointOld = pathControllerOld.waypoints[index];
        waypointOld.delay = delay;

        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputDelay = delay;
    }

    [PunRPC]
    private void RPCSetWaypointRotateWhileDelay(bool rotateWhileDelay, int index)
    {
        WaypointOld waypointOld = pathControllerOld.waypoints[index];
        waypointOld.rotateWhileDelay = rotateWhileDelay;

        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputRotateWhileDelay = rotateWhileDelay;
    }

    [PunRPC]
    private void RPCDeleteWaypoint(int index)
    {
        WaypointOld waypointOld = pathControllerOld.waypoints[index];
        waypointOld.WaypointEditor.DeleteThisWaypoint();
    }

    [PunRPC]
    private void RPCAddWaypoint()
    {
        pathControllerOld.waypoints.Add(new(new(0, 0), true, 0, 1, 0));
        if (pathControllerOld.waypoints.Count > 0 && pathControllerOld.waypoints[0].WaypointEditor != null)
            ReferenceManager.Instance.ballWindows.GetComponentInChildren<PathEditorControllerOld>().UpdateUI();
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
    }

    public override Data GetData()
    {
        return new AnchorDataOld(pathControllerOld, container.transform);
    }
}