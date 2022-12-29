using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaypointEditorController : MonoBehaviour
{
    public static WaypointEditorController StartPosition { get; private set; }

    public int waypointIndex;

    [Header("references")] [SerializeField]
    private TMP_Text noTxt;

    [SerializeField] private TMP_InputField positionX;
    [SerializeField] private TMP_InputField positionY;
    [SerializeField] private TMP_InputField speed;
    [SerializeField] private TMP_InputField rotationSpeed;
    [SerializeField] private TMP_InputField turns;
    [SerializeField] private TMP_InputField delay;
    [SerializeField] private Toggle rotateWhileDelay;

    private AnchorController anchorController;

    public Vector2 InputPosition
    {
        get
        {
            // get x and y pos from input fields, return 0 if input field is empty
            float x;
            if (positionX.text != string.Empty) x = float.Parse(positionX.text);

            else x = 0;

            float y;
            if (positionY.text != string.Empty) y = float.Parse(positionY.text);

            else y = 0;

            return new Vector2(x, y);
        }
        set
        {
            // set input fields to position
            positionX.text = value.x.ToString();
            positionY.text = value.y.ToString();
            if (Waypoints.Count > 1)
                turns.text = GetTurns().ToString();
        }
    }

    public float InputSpeed
    {
        get => speed.text != string.Empty ? float.Parse(speed.text) : 0f;
        set
        {
            speed.text = value.ToString();
            turns.text = GetTurns().ToString();
        }
    }

    public float InputRotationSpeed
    {
        get => rotationSpeed.text != string.Empty ? float.Parse(rotationSpeed.text) : 0f;
        set
        {
            rotationSpeed.text = value.ToString();
            if (Waypoints.Count > 1) turns.text = GetTurns().ToString();
        }
    }

    public float InputTurns
    {
        get => turns.text != string.Empty ? float.Parse(turns.text) : 0f;
        set
        {
            if (Waypoints.Count <= 1) return;

            turns.text = value.ToString();
            rotationSpeed.text = GetRotationSpeed().ToString();
        }
    }

    public float InputDelay
    {
        get => delay.text != string.Empty ? float.Parse(delay.text) : 0f;
        set => delay.text = value.ToString();
    }

    public bool InputRotateWhileDelay
    {
        get => rotateWhileDelay.isOn;
        set => rotateWhileDelay.isOn = value;
    }

    private Waypoint Waypoint => AnchorManager.Instance.selectedPathController.waypoints[waypointIndex];

    private static List<Waypoint> Waypoints => AnchorManager.Instance.selectedPathController.waypoints;

    private void Start()
    {
        noTxt.text = $"No. {waypointIndex + 1}";

        if (waypointIndex == 0) StartPosition = this;

        Waypoint.WaypointEditor = this;

        anchorController = AnchorManager.Instance.SelectedAnchor.GetComponent<AnchorController>();
    }

    public void UpdatePosition()
    {
        SetPosition(InputPosition);

        if (!GameManager.Instance.Multiplayer) return;

        if (anchorController != null)
            anchorController.View.RPC("RPCSetWaypointPosition", RpcTarget.Others, InputPosition, waypointIndex);
    }

    private void SetPosition(Vector2 pos)
    {
        Waypoint.position = pos;

        AnchorManager.Instance.selectedPathController.DrawLines();
    }

    public void UpdateSpeed()
    {
        SetSpeed(InputSpeed);

        if (!GameManager.Instance.Multiplayer) return;

        if (anchorController != null)
            anchorController.View.RPC("RPCSetWaypointSpeed", RpcTarget.Others, InputSpeed, waypointIndex);
    }

    private void SetSpeed(float speed)
    {
        Waypoint.speed = speed;
    }

    public void UpdateRotationSpeed()
    {
        SetRotationSpeed(InputRotationSpeed);

        if (!GameManager.Instance.Multiplayer) return;

        if (anchorController != null)
            anchorController.View.RPC("RPCSetWaypointRotationSpeed", RpcTarget.Others, InputRotationSpeed,
                waypointIndex);
    }

    private void SetRotationSpeed(float speed)
    {
        Waypoint.rotationSpeed = speed;
        if (Waypoints.Count > 1)
            turns.text = GetTurns().ToString();
    }

    public void UpdateTurns()
    {
        print("disabled turns cause im 2 lazy to implement them so they are consistent AND work in multiplayer");

        //SetTurns(InputTurns);

        //if (GameManager.Instance.Multiplayer)
        //{
        //    if (anchorController != null)
        //        anchorController.View.RPC("RPCSetWaypointRotationSpeed", RpcTarget.Others, InputRotationSpeed, waypointIndex);
        //}
    }

    public void SetTurns(float turns)
    {
        Waypoint.rotationSpeed = turns;
        if (Waypoints.Count > 1)
            rotationSpeed.text = GetRotationSpeed().ToString();
    }

    public void UpdateDelay()
    {
        SetDelay(InputDelay);

        if (!GameManager.Instance.Multiplayer) return;

        if (anchorController != null)
            anchorController.View.RPC("RPCSetWaypointDelay", RpcTarget.Others, InputDelay, waypointIndex);
    }

    private void SetDelay(float delay)
    {
        Waypoint.delay = delay;
    }

    public void UpdateRotateWhileDelay()
    {
        SetRotateWhileDelay(InputRotateWhileDelay);

        if (!GameManager.Instance.Multiplayer) return;

        if (anchorController != null)
            anchorController.View.RPC("RPCSetWaypointRotateWhileDelay", RpcTarget.Others, InputRotateWhileDelay,
                waypointIndex);
    }

    private void SetRotateWhileDelay(bool rotateWhileDelay)
    {
        Waypoint.rotateWhileDelay = rotateWhileDelay;
    }

    public void DeleteThisWaypoint()
    {
        if (GameManager.Instance.Multiplayer)
        {
            if (anchorController != null)
                anchorController.View.RPC("RPCDeleteWaypoint", RpcTarget.Others, waypointIndex);
        }

        AnchorManager.Instance.selectedPathController.waypoints.Remove(Waypoint);
        transform.parent.parent.parent.parent.GetComponent<PathEditorController>().UpdateUI();
    }

    public void UpdateInputValues()
    {
        InputRotationSpeed = Waypoint.rotationSpeed;
        InputPosition = Waypoint.position;
        InputSpeed = Waypoint.speed;
        InputDelay = Waypoint.delay;
        InputRotateWhileDelay = Waypoint.rotateWhileDelay;
    }

    public float GetTurns()
    {
        Waypoint nextWaypoint = Waypoints[(waypointIndex + 1) % Waypoints.Count];
        if (Waypoints.Count > 1 && Vector2.Distance(Waypoint.position, nextWaypoint.position) != 0)
            return Vector2.Distance(Waypoint.position, nextWaypoint.position) / Waypoint.speed *
                Waypoint.rotationSpeed / 360f;
        return 0f;
    }

    public float GetRotationSpeed()
    {
        Waypoint nextWaypoint = Waypoints[(waypointIndex + 1) % Waypoints.Count];
        if (Waypoints.Count > 1 && Vector2.Distance(Waypoint.position, nextWaypoint.position) != 0)
            return InputTurns * 360f / (Vector2.Distance(Waypoint.position, nextWaypoint.position) / Waypoint.speed);
        return 0f;
    }
}