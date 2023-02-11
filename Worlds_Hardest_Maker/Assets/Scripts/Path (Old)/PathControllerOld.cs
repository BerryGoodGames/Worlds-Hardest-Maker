using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// </summary>
public class PathControllerOld : MonoBehaviour
{
    public enum PathMode
    {
        BOUNCE,
        LOOP,
        STOP
    }

    public List<WaypointOld> waypoints = new()
    {
        new WaypointOld(Vector2.zero, true, 0, 1, 0)
    };

    [FormerlySerializedAs("LineContainer")] [SerializeField]
    private Transform lineContainer;

    [Space] [Header("Settings")] public bool setElement0ToStartingPos = true;
    public bool drawLines = true;
    public bool onlyMoveWhenPlaying = true;
    public PathMode pathMode = 0;

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
        if (waypoints is { Count: > 0 })
        {
            Target = waypoints[0];
            targetIndex = 0;
        }

        moveCoroutine = Move();
    }

    private void Start()
    {
        if (setElement0ToStartingPos && waypoints[0] != null) waypoints[0].position = transform.position;
        StartCoroutine(moveCoroutine);
    }

    public void UpdateStartingPosition()
    {
        if (setElement0ToStartingPos && waypoints[0] != null)
        {
            waypoints[0].position = transform.position;
            if (WaypointEditorControllerOld.StartPosition != null)
                WaypointEditorControllerOld.StartPosition.UpdateInputValues();
        }

        AnchorManagerOld.Instance.selectedPathControllerOld.DrawLines();
    }

    private IEnumerator Move()
    {
        while (true)
        {
            if ((!EditModeManager.Instance.Playing && onlyMoveWhenPlaying) || stop || waypoints == null)
            {
                yield return null;
                continue;
            }

            if ((Vector2)transform.position == target.position)
            {
                // delay next move
                if (target.delay > 0)
                {
                    if (target.rotateWhileDelay)
                    {
                        rotationCoroutine = Rotate(target.rotationSpeed);
                        StartCoroutine(rotationCoroutine);
                        yield return new WaitForSeconds(target.delay);
                        StopCoroutine(rotationCoroutine);
                    }
                    else
                        yield return new WaitForSeconds(target.delay);
                }

                switch (pathMode)
                {
                    // bounce path mode
                    case PathMode.BOUNCE when waypoints.Count <= 1:
                        break;
                    case PathMode.BOUNCE when targetIndex == waypoints.Count - 1:
                        onReturn = true;
                        targetIndex--;
                        target = waypoints[targetIndex];
                        break;
                    case PathMode.BOUNCE when targetIndex == 0:
                        onReturn = false;
                        targetIndex++;
                        Target = waypoints[targetIndex];
                        break;
                    case PathMode.BOUNCE:
                        targetIndex += onReturn ? -1 : 1;
                        target = waypoints[targetIndex];
                        attributeTarget = onReturn ? waypoints[targetIndex + 1] : target;
                        break;
                    // loop path mode
                    case PathMode.LOOP:
                        targetIndex = (targetIndex + 1) % waypoints.Count;
                        Target = waypoints[targetIndex];
                        break;
                    // stop path mode
                    case PathMode.STOP:
                    {
                        targetIndex++;
                        if (targetIndex >= waypoints.Count)
                        {
                            stop = true;
                            yield break;
                        }

                        Target = waypoints[targetIndex];
                        break;
                    }
                }
            }

            // move towards target
            transform.position = Vector2.MoveTowards(transform.position, target.position,
                attributeTarget.speed * Time.deltaTime);

            transform.Rotate(new Vector3(0, 0, attributeTarget.rotationSpeed * Time.deltaTime));
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
        if (waypoints.Count > 0 && waypoints[0] != null)
        {
            transform.position = waypoints[0].position;
            Target = waypoints[0];
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

        for (int i = 1; i < waypoints.Count; i++)
        {
            WaypointOld prevWaypointOld = waypoints[i - 1];
            WaypointOld currentWaypointOld = waypoints[i];
            LineManager.SetFill(Color.black);
            LineManager.SetWeight(0.1f);
            LineManager.SetLayerID(LineManager.defaultLayerID);
            LineManager.SetOrderInLayer(0);
            LineManager.DrawLine(prevWaypointOld.position, currentWaypointOld.position, lineContainer);
        }

        if (pathMode == PathMode.LOOP && waypoints.Count > 1)
        {
            LineManager.DrawLine(waypoints[^1].position, waypoints[0].position, lineContainer);
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