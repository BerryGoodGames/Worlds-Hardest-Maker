using System;
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
    public bool editing = true;

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
    private static readonly int editingString = Animator.StringToHash("Editing");
    public event Action OnEditModeChange;

    #endregion

    #region Methods

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
        if (currentEditMode is EditMode.ANCHOR or EditMode.BALL)
        {
            // enable stuff
            ReferenceManager.Instance.ballWindows.SetActive(true);

            if (AnchorManager.Instance.SelectedAnchor == null) return;

            // enable lines
            AnchorManager.Instance.selectedPathController.drawLines = true;
            AnchorManager.Instance.selectedPathController.DrawLines();

            // switch animation to editing
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                Animator anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool(editingString, true);
            }
        }
        else if (currentEditMode != EditMode.ANCHOR && currentEditMode != EditMode.BALL)
        {
            // disable stuff
            ReferenceManager.Instance.ballWindows.SetActive(false);

            if (AnchorManager.Instance.SelectedAnchor == null) return;

            // disable lines
            AnchorManager.Instance.selectedPathController.drawLines = false;
            AnchorManager.Instance.selectedPathController.ClearLines();

            // switch animation to editing
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                Animator anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool(editingString, false);
            }
        }
    }

    private void SetEditRotation(int value)
    {
        editRotation = value;

        ReferenceManager.Instance.placementPreview.GetComponent<PreviewController>().UpdateRotation();
    }

    #endregion

    private void Start()
    {
        Instance.OnEdit += () => PlayManager.Instance.Cheated = false;

        Instance.SetEditMode(currentEditMode);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}