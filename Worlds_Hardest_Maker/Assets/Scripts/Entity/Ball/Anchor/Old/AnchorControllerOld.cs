using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     controls the anchor (duh)
/// </summary>
public class AnchorControllerOld : Controller
{
    [FormerlySerializedAs("outline")] public GameObject Outline;
    [FormerlySerializedAs("container")] public GameObject Container;

    private PathControllerOld pathControllerOld;
    public PhotonView View { get; set; }

    private void Start()
    {
        Transform t = transform;
        Transform parent = t.parent;

        t.localPosition = parent.position;

        parent.position = Vector2.zero;

        parent.SetParent(ReferenceManager.Instance.AnchorContainer);

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
        AnchorBallManagerOld.SetAnchorBall(pos, Container.transform);
    }

    [PunRPC]
    private void RPCSetWaypointPosition(Vector2 pos, int index)
    {
        WaypointOld waypointOld = pathControllerOld.Waypoints[index];
        waypointOld.Position = pos;
        AnchorManagerOld.Instance.SelectedPathControllerOld.DrawLines();
        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputPosition = pos;
    }

    [PunRPC]
    private void RPCSetWaypointSpeed(float speed, int index)
    {
        WaypointOld waypointOld = pathControllerOld.Waypoints[index];
        waypointOld.Speed = speed;

        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputSpeed = speed;
    }

    [PunRPC]
    private void RPCSetWaypointRotationSpeed(float speed, int index)
    {
        WaypointOld waypointOld = pathControllerOld.Waypoints[index];
        waypointOld.RotationSpeed = speed;

        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputRotationSpeed = speed;
    }

    [PunRPC]
    private void RPCSetWaypointDelay(float delay, int index)
    {
        WaypointOld waypointOld = pathControllerOld.Waypoints[index];
        waypointOld.Delay = delay;

        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputDelay = delay;
    }

    [PunRPC]
    private void RPCSetWaypointRotateWhileDelay(bool rotateWhileDelay, int index)
    {
        WaypointOld waypointOld = pathControllerOld.Waypoints[index];
        waypointOld.RotateWhileDelay = rotateWhileDelay;

        if (waypointOld.WaypointEditor != null)
            waypointOld.WaypointEditor.InputRotateWhileDelay = rotateWhileDelay;
    }

    [PunRPC]
    private void RPCDeleteWaypoint(int index)
    {
        WaypointOld waypointOld = pathControllerOld.Waypoints[index];
        waypointOld.WaypointEditor.DeleteThisWaypoint();
    }

    [PunRPC]
    private void RPCAddWaypoint()
    {
        pathControllerOld.Waypoints.Add(new(new(0, 0), true, 0, 1, 0));
        // if (pathControllerOld.Waypoints.Count > 0 && pathControllerOld.Waypoints[0].WaypointEditor != null)
            // ReferenceManager.Instance.BallWindows.GetComponentInChildren<PathEditorControllerOld>().UpdateUI();
    }

    public void BallFadeOut(AnimationEvent animationEvent)
    {
        float endOpacity = animationEvent.floatParameter;
        if (float.TryParse(animationEvent.stringParameter, out float time))
            StartCoroutine(Container.GetComponent<ChildrenOpacity>().FadeOut(endOpacity, time));
    }

    public void BallFadeIn(AnimationEvent animationEvent)
    {
        float endOpacity = animationEvent.floatParameter;
        if (float.TryParse(animationEvent.stringParameter, out float time))
            BallFadeIn(endOpacity, time);
    }

    public void BallFadeIn(float endOpacity, float time)
    {
        StartCoroutine(Container.GetComponent<ChildrenOpacity>().FadeIn(endOpacity, time));
    }

    public IEnumerator FadeInOnNextFrame(float endOpacity, float time)
    {
        yield return null;
        BallFadeIn(endOpacity, time);
    }

    public override Data GetData()
    {
        return new AnchorDataOld(pathControllerOld, Container.transform);
    }
}