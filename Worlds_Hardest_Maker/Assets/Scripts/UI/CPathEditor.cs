using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPathEditor : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Transform waypointEditorContainer;
    [SerializeField] private TMPro.TMP_Dropdown modeDropdown;
    [Space] 
    [SerializeField] private GameObject waypointEditor;

    private RectTransform rt;

    private Vector2 startPos;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
    }


    public void ChangeMode(int mode)
    {
        if (MAnchor.Instance.SelectedAnchor == null) return;
        PathController pathController = MAnchor.Instance.SelectedAnchor.GetComponent<PathController>();
        pathController.pathMode = (PathController.PathMode)mode;
        UpdateUI();
    }

    public void AddWaypoint()
    {
        if (MAnchor.Instance.SelectedAnchor == null) return;
        int waypointCount = MAnchor.Instance.selectedPathController.waypoints.Count;

        Waypoint prevWaypoint = MAnchor.Instance.selectedPathController.waypoints[waypointCount - 1];

        MAnchor.Instance.selectedPathController.waypoints.Add(prevWaypoint.Clone());
        UpdateUI();

        if(MGame.Instance.Multiplayer)
        {
            MAnchor.Instance.selectedPathController.GetComponent<CAnchor>().View.RPC("RPCAddWaypoint", Photon.Pun.RpcTarget.Others);
        }
    }

    public void UpdateUI()
    {
        PathController pathController = MAnchor.Instance.SelectedAnchor.GetComponent<PathController>();

        MAnchor.Instance.selectedPathController.DrawLines();
        // update mode
        modeDropdown.value = (int)pathController.pathMode;
        // clear waypoint editors
        foreach (Transform child in waypointEditorContainer)
        {
            if(!child.CompareTag("DontDestroy"))
                Destroy(child.gameObject);
        }

        for (int i = 0; i < MAnchor.Instance.selectedPathController.waypoints.Count; i++)
        {
            GameObject editor = Instantiate(waypointEditor, Vector2.zero, Quaternion.identity, waypointEditorContainer);
            WaypointEditorController component = editor.GetComponent<WaypointEditorController>();
            component.waypointIndex = i;
            component.UpdateInputValues();
        }
    }

    public void ResetPosition()
    {
        if(rt != null)
            rt.anchoredPosition = startPos;
    }
}
