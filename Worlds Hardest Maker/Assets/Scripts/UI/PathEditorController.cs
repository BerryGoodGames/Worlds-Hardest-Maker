using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathEditorController : MonoBehaviour
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
        if (AnchorManager.Instance.SelectedAnchor == null) return;
        PathController pathController = AnchorManager.Instance.SelectedAnchor.GetComponent<PathController>();
        pathController.pathMode = (PathController.PathMode)mode;
        UpdateUI();
    }

    public void AddWaypoint()
    {
        if (AnchorManager.Instance.SelectedAnchor == null) return;
        AnchorManager.Instance.selectedPathController.waypoints.Add(new(new(0, 0), true, 0, 1, 0));
        UpdateUI();

        if(GameManager.Instance.Multiplayer)
        {
            AnchorManager.Instance.selectedPathController.GetComponent<AnchorController>().View.RPC("RPCAddWaypoint", Photon.Pun.RpcTarget.Others);
        }
    }

    public void UpdateUI()
    {
        PathController pathController = AnchorManager.Instance.SelectedAnchor.GetComponent<PathController>();

        AnchorManager.Instance.selectedPathController.DrawLines();
        // update mode
        modeDropdown.value = (int)pathController.pathMode;
        // clear waypoint editors
        foreach (Transform child in waypointEditorContainer)
        {
            if(!child.CompareTag("DontDestroy"))
                Destroy(child.gameObject);
        }

        for (int i = 0; i < AnchorManager.Instance.selectedPathController.waypoints.Count; i++)
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
