using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// </summary>
public class PathControllerOld : MonoBehaviour
{
    public enum PathModeType
    {
        BOUNCE,
        LOOP,
        STOP
    }

    [FormerlySerializedAs("waypoints")] public List<WaypointOld> Waypoints = new()
    {
        new WaypointOld(Vector2.zero, true, 0, 1, 0)
    };

    [FormerlySerializedAs("LineContainer")] [SerializeField]
    private Transform lineContainer;

    [FormerlySerializedAs("setElement0ToStartingPos")] [Space] [Header("Settings")] public bool SetElement0ToStartingPos = true;
    [FormerlySerializedAs("DrawLines")] [FormerlySerializedAs("drawLines")] public bool DoDrawLines = true;
    [FormerlySerializedAs("onlyMoveWhenPlaying")] public bool OnlyMoveWhenPlaying = true;
    [FormerlySerializedAs("pathMode")] public PathModeType PathMode = 0;

    private WaypointOld attributeTarget;
    private WaypointOld target;
    private int targetIndex;

    private bool stop;
    private bool onReturn;
    private IEnumerator rotationCoroutine;
    private IEnumerator moveCoroutine;

    private WaypointOld Target
    {
        set
        {
            target = value;
            attributeTarget = value;
        }
    }

    private void Awake()
    {
        if (Waypoints is { Count: > 0 })
        {
            Target = Waypoints[0];
            targetIndex = 0;
        }

        moveCoroutine = Move();
    }

    private void Start()
    {
        if (SetElement0ToStartingPos && Waypoints[0] != null) Waypoints[0].Position = transform.position;
        StartCoroutine(moveCoroutine);
    }

    public void UpdateStartingPosition()
    {
        if (SetElement0ToStartingPos && Waypoints[0] != null)
        {
            Waypoints[0].Position = transform.position;
            if (WaypointEditorControllerOld.StartPosition != null)
                WaypointEditorControllerOld.StartPosition.UpdateInputValues();
        }

        AnchorManagerOld.Instance.SelectedPathControllerOld.DrawLines();
    }

    private IEnumerator Move()
    {
        while (true)
        {
            if ((!EditModeManager.Instance.Playing && OnlyMoveWhenPlaying) || stop || Waypoints == null)
            {
                yield return null;
                continue;
            }

            if ((Vector2)transform.position == target.Position)
            {
                // delay next move
                if (target.Delay > 0)
                {
                    if (target.RotateWhileDelay)
                    {
                        rotationCoroutine = Rotate(target.RotationSpeed);
                        StartCoroutine(rotationCoroutine);
                        yield return new WaitForSeconds(target.Delay);
                        StopCoroutine(rotationCoroutine);
                    }
                    else
                        yield return new WaitForSeconds(target.Delay);
                }

                switch (PathMode)
                {
                    // bounce path mode
                    case PathModeType.BOUNCE when Waypoints.Count <= 1:
                        break;
                    case PathModeType.BOUNCE when targetIndex == Waypoints.Count - 1:
                        onReturn = true;
                        targetIndex--;
                        target = Waypoints[targetIndex];
                        break;
                    case PathModeType.BOUNCE when targetIndex == 0:
                        onReturn = false;
                        targetIndex++;
                        Target = Waypoints[targetIndex];
                        break;
                    case PathModeType.BOUNCE:
                        targetIndex += onReturn ? -1 : 1;
                        target = Waypoints[targetIndex];
                        attributeTarget = onReturn ? Waypoints[targetIndex + 1] : target;
                        break;
                    // loop path mode
                    case PathModeType.LOOP:
                        targetIndex = (targetIndex + 1) % Waypoints.Count;
                        Target = Waypoints[targetIndex];
                        break;
                    // stop path mode
                    case PathModeType.STOP:
                    {
                        targetIndex++;
                        if (targetIndex >= Waypoints.Count)
                        {
                            stop = true;
                            yield break;
                        }

                        Target = Waypoints[targetIndex];
                        break;
                    }
                }
            }

            // move towards target
            transform.position = Vector2.MoveTowards(transform.position, target.Position,
                attributeTarget.Speed * Time.deltaTime);

            transform.Rotate(new Vector3(0, 0, attributeTarget.RotationSpeed * Time.deltaTime));
            yield return null;
        }
    }

    private IEnumerator Rotate(float speed)
    {
        while (true)
        {
            transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
            yield return null;
        }
    }

    public void ResetState()
    {
        if (Waypoints.Count > 0 && Waypoints[0] != null)
        {
            transform.position = Waypoints[0].Position;
            Target = Waypoints[0];
            targetIndex = 0;
            onReturn = false;
            stop = false;
        }

        StopCoroutine(moveCoroutine);
        StartCoroutine(moveCoroutine);
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        transform.rotation = Quaternion.identity;
    }

    public void DrawLines()
    {
        ClearLines();

        for (int i = 1; i < Waypoints.Count; i++)
        {
            WaypointOld prevWaypointOld = Waypoints[i - 1];
            WaypointOld currentWaypointOld = Waypoints[i];
            LineManager.SetFill(Color.black);
            LineManager.SetWeight(0.1f);
            LineManager.SetLayerID(LineManager.DefaultLayerID);
            LineManager.SetOrderInLayer(0);
            LineManager.DrawLine(prevWaypointOld.Position, currentWaypointOld.Position, lineContainer);
        }

        if (PathMode == PathModeType.LOOP && Waypoints.Count > 1)
        {
            LineManager.DrawLine(Waypoints[^1].Position, Waypoints[0].Position, lineContainer);
        }
    }

    public void ClearLines()
    {
        foreach (Transform line in lineContainer)
        {
            Destroy(line.gameObject);
        }
    }
}