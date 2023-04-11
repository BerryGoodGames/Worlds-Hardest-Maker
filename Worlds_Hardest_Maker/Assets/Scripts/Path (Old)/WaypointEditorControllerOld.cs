using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WaypointEditorControllerOld : MonoBehaviour
{
    public static WaypointEditorControllerOld StartPosition { get; private set; }

    [FormerlySerializedAs("waypointIndex")]
    public int WaypointIndex;

    [Header("references")] [SerializeField]
    private TMP_Text noTxt;

    [SerializeField] private TMP_InputField positionX;
    [SerializeField] private TMP_InputField positionY;
    [SerializeField] private TMP_InputField speed;
    [SerializeField] private TMP_InputField rotationSpeed;
    [SerializeField] private TMP_InputField turns;
    [SerializeField] private TMP_InputField delay;
    [SerializeField] private Toggle rotateWhileDelay;

    private AnchorControllerOld anchorControllerOld;

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

    private WaypointOld WaypointOld => AnchorManagerOld.Instance.SelectedPathControllerOld.Waypoints[WaypointIndex];

    private static List<WaypointOld> Waypoints => AnchorManagerOld.Instance.SelectedPathControllerOld.Waypoints;

    private void Start()
    {
        noTxt.text = $"No. {WaypointIndex + 1}";

        if (WaypointIndex == 0) StartPosition = this;

        WaypointOld.WaypointEditor = this;

        anchorControllerOld = AnchorManagerOld.Instance.SelectedAnchor.GetComponent<AnchorControllerOld>();
    }

    public void UpdatePosition()
    {
        SetPosition(InputPosition);

        if (!MultiplayerManager.Instance.Multiplayer) return;

        if (anchorControllerOld != null)
            anchorControllerOld.View.RPC("RPCSetWaypointPosition", RpcTarget.Others, InputPosition, WaypointIndex);
    }

    private void SetPosition(Vector2 pos)
    {
        WaypointOld.Position = pos;

        AnchorManagerOld.Instance.SelectedPathControllerOld.DrawLines();
    }

    public void UpdateSpeed()
    {
        SetSpeed(InputSpeed);

        if (!MultiplayerManager.Instance.Multiplayer) return;

        if (anchorControllerOld != null)
            anchorControllerOld.View.RPC("RPCSetWaypointSpeed", RpcTarget.Others, InputSpeed, WaypointIndex);
    }

    private void SetSpeed(float speed)
    {
        WaypointOld.Speed = speed;
    }

    public void UpdateRotationSpeed()
    {
        SetRotationSpeed(InputRotationSpeed);

        if (!MultiplayerManager.Instance.Multiplayer) return;

        if (anchorControllerOld != null)
            anchorControllerOld.View.RPC("RPCSetWaypointRotationSpeed", RpcTarget.Others, InputRotationSpeed,
                WaypointIndex);
    }

    private void SetRotationSpeed(float speed)
    {
        WaypointOld.RotationSpeed = speed;
        if (Waypoints.Count > 1)
            turns.text = GetTurns().ToString();
    }

    public void UpdateTurns()
    {
        print("disabled turns cause im 2 lazy to implement them so they are consistent AND work in multiplayer");

        //SetTurns(InputTurns);

        //if (MultiplayerManager.Instance.Multiplayer)
        //{
        //    if (anchorController != null)
        //        anchorController.View.RPC("RPCSetWaypointRotationSpeed", RpcTarget.Others, InputRotationSpeed, waypointIndex);
        //}
    }

    public void SetTurns(float turns)
    {
        WaypointOld.RotationSpeed = turns;
        if (Waypoints.Count > 1)
            rotationSpeed.text = GetRotationSpeed().ToString();
    }

    public void UpdateDelay()
    {
        SetDelay(InputDelay);

        if (!MultiplayerManager.Instance.Multiplayer) return;

        if (anchorControllerOld != null)
            anchorControllerOld.View.RPC("RPCSetWaypointDelay", RpcTarget.Others, InputDelay, WaypointIndex);
    }

    private void SetDelay(float delay)
    {
        WaypointOld.Delay = delay;
    }

    public void UpdateRotateWhileDelay()
    {
        SetRotateWhileDelay(InputRotateWhileDelay);

        if (!MultiplayerManager.Instance.Multiplayer) return;

        if (anchorControllerOld != null)
            anchorControllerOld.View.RPC("RPCSetWaypointRotateWhileDelay", RpcTarget.Others, InputRotateWhileDelay,
                WaypointIndex);
    }

    private void SetRotateWhileDelay(bool rotateWhileDelay)
    {
        WaypointOld.RotateWhileDelay = rotateWhileDelay;
    }

    public void DeleteThisWaypoint()
    {
        if (MultiplayerManager.Instance.Multiplayer)
        {
            if (anchorControllerOld != null)
                anchorControllerOld.View.RPC("RPCDeleteWaypoint", RpcTarget.Others, WaypointIndex);
        }

        AnchorManagerOld.Instance.SelectedPathControllerOld.Waypoints.Remove(WaypointOld);
        transform.parent.parent.parent.parent.GetComponent<PathEditorControllerOld>().UpdateUI();
    }

    public void UpdateInputValues()
    {
        InputRotationSpeed = WaypointOld.RotationSpeed;
        InputPosition = WaypointOld.Position;
        InputSpeed = WaypointOld.Speed;
        InputDelay = WaypointOld.Delay;
        InputRotateWhileDelay = WaypointOld.RotateWhileDelay;
    }

    public float GetTurns()
    {
        WaypointOld nextWaypointOld = Waypoints[(WaypointIndex + 1) % Waypoints.Count];
        if (Waypoints.Count > 1 && Vector2.Distance(WaypointOld.Position, nextWaypointOld.Position) != 0)
            return Vector2.Distance(WaypointOld.Position, nextWaypointOld.Position) / WaypointOld.Speed *
                WaypointOld.RotationSpeed / 360f;
        return 0f;
    }

    public float GetRotationSpeed()
    {
        WaypointOld nextWaypointOld = Waypoints[(WaypointIndex + 1) % Waypoints.Count];
        if (Waypoints.Count > 1 && Vector2.Distance(WaypointOld.Position, nextWaypointOld.Position) != 0)
            return InputTurns * 360f /
                   (Vector2.Distance(WaypointOld.Position, nextWaypointOld.Position) / WaypointOld.Speed);
        return 0f;
    }
}