using Photon.Pun;
using TMPro;
using UnityEngine;

public class PathEditorControllerOld : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Transform waypointEditorContainer;
    [SerializeField] private TMP_Dropdown modeDropdown;
    [Space] [SerializeField] private GameObject waypointEditor;

    private RectTransform rt;

    private Vector2 startPos;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
    }


    public void ChangeMode(int mode)
    {
        if (AnchorManagerOld.Instance.SelectedAnchor == null) return;
        PathControllerOld pathControllerOld = AnchorManagerOld.Instance.SelectedAnchor.GetComponent<PathControllerOld>();
        pathControllerOld.pathMode = (PathControllerOld.PathMode)mode;
        UpdateUI();
    }

    public void AddWaypoint()
    {
        if (AnchorManagerOld.Instance.SelectedAnchor == null) return;
        int waypointCount = AnchorManagerOld.Instance.selectedPathControllerOld.waypoints.Count;

        WaypointOld prevWaypointOld = AnchorManagerOld.Instance.selectedPathControllerOld.waypoints[waypointCount - 1];

        AnchorManagerOld.Instance.selectedPathControllerOld.waypoints.Add(prevWaypointOld.Clone());
        UpdateUI();

        if (MultiplayerManager.Instance.Multiplayer)
        {
            AnchorManagerOld.Instance.selectedPathControllerOld.GetComponent<AnchorControllerOld>().View
                .RPC("RPCAddWaypoint", RpcTarget.Others);
        }
    }

    public void UpdateUI()
    {
        PathControllerOld pathControllerOld = AnchorManagerOld.Instance.SelectedAnchor.GetComponent<PathControllerOld>();

        AnchorManagerOld.Instance.selectedPathControllerOld.DrawLines();
        // update mode
        modeDropdown.value = (int)pathControllerOld.pathMode;
        // clear waypoint editors
        foreach (Transform child in waypointEditorContainer)
        {
            if (!child.CompareTag("DontDestroy"))
                Destroy(child.gameObject);
        }

        for (int i = 0; i < AnchorManagerOld.Instance.selectedPathControllerOld.waypoints.Count; i++)
        {
            GameObject editor = Instantiate(waypointEditor, Vector2.zero, Quaternion.identity, waypointEditorContainer);
            WaypointEditorControllerOld component = editor.GetComponent<WaypointEditorControllerOld>();
            component.waypointIndex = i;
            component.UpdateInputValues();
        }
    }

    public void ResetPosition()
    {
        if (rt != null)
            rt.anchoredPosition = startPos;
    }
}