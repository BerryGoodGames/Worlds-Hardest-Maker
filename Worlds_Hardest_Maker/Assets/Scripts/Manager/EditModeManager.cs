using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeManager : MonoBehaviour
{
    public static EditModeManager Instance { get; set; }

    #region Variables & properties

    // current edit mode
    [SerializeField] private EditMode currentEditMode;
    private EditMode? prevEditMode;
    public EditMode CurrentEditMode
    {
        get => currentEditMode;
        set => Instance.SetEditMode(value);
    }

    // editing
    public bool editing;
    public bool Playing
    {
        get => !editing;
        set => editing = !value;
    }

    // edit rotation
    private int editRotation = 270;
    public int EditRotation
    {
        get => editRotation;
        set => Instance.SetEditRotation(value);
    }

    #endregion

    #region Events

    public Action OnPlay;
    public Action OnEdit;
    public event Action OnEditModeChange;

    #endregion


    public void SetEditMode(EditMode value)
    {
        currentEditMode = value;

        if (prevEditMode != null && prevEditMode != currentEditMode) OnEditModeChange?.Invoke();

        prevEditMode = currentEditMode;

        // update toolbarContainer
        GameObject[] tools = ToolbarManager.tools;
        foreach (GameObject tool in tools)
        {
            Tool t = tool.GetComponent<Tool>();
            if (t.toolName == value)
            {
                // avoid recursion
                t.SwitchGameMode(false);
            }
        }

        // enable/disable outlines and window when switching to/away from anchors/balls
        if (currentEditMode == EditMode.ANCHOR || currentEditMode == EditMode.BALL)
        {
            // enable stuff
            ReferenceManager.Instance.ballWindows.gameObject.SetActive(true);
            if (AnchorManager.Instance.SelectedAnchor != null)
            {
                // enable lines
                AnchorManager.Instance.selectedPathController.drawLines = true;
                AnchorManager.Instance.selectedPathController.DrawLines();

                // switch animation to editing
                foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
                {
                    Animator anim = anchor.GetComponentInChildren<Animator>();
                    anim.SetBool("Editing", true);
                }
            }
        }
        else if (currentEditMode != EditMode.ANCHOR && currentEditMode != EditMode.BALL)
        {
            // disable stuff
            ReferenceManager.Instance.ballWindows.SetActive(false);
            if (AnchorManager.Instance.SelectedAnchor != null)
            {
                // disable lines
                AnchorManager.Instance.selectedPathController.drawLines = false;
                AnchorManager.Instance.selectedPathController.ClearLines();

                // switch animation to editing
                foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
                {
                    Animator anim = anchor.GetComponentInChildren<Animator>();
                    anim.SetBool("Editing", false);
                }
            }
        }
    }
    private void SetEditRotation(int value)
    {
        editRotation = value;

        ReferenceManager.Instance.placementPreview.GetComponent<PreviewController>().UpdateRotation();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
