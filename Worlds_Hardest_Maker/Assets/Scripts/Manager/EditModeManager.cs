using System;
using MyBox;
using UnityEngine;

public class EditModeManager : MonoBehaviour
{
    public static EditModeManager Instance { get; set; }

    #region Variables & properties

    // current edit mode
    [SerializeField] [SearchableEnum] private EditMode currentEditMode;
    private EditMode? prevEditMode;

    public EditMode CurrentEditMode
    {
        get => currentEditMode;
        set => Instance.SetEditMode(value);
    }

    // editing
    [field: SerializeField] public bool Editing { get; set; }

    public bool Playing
    {
        get => !Editing;
        set => Editing = !value;
    }

    // edit rotation
    [SerializeField][ReadOnly] private int editRotation = 270;

    public int EditRotation
    {
        get => editRotation;
        set => Instance.SetEditRotation(value);
    }

    #endregion

    #region Events

    public event Action OnPlay;
    public event Action OnEdit;
    private static readonly int editingString = Animator.StringToHash("Editing");
    public event Action OnEditModeChange;

    #endregion

    #region Methods

    public void SetEditMode(EditMode value)
    {
        currentEditMode = value;

        // invoke OnEditModeChanged
        if (prevEditMode != null && prevEditMode != currentEditMode) OnEditModeChange?.Invoke();
        prevEditMode = currentEditMode;

        // update toolbarContainer
        GameObject[] tools = ToolbarManager.Tools;
        foreach (GameObject tool in tools)
        {
            Tool t = tool.GetComponent<Tool>();
            if (t.ToolName == value)
                // avoid recursion
                t.SwitchGameMode(false);
        }

        // enable/disable outlines and panel when switching to/away from anchors or anchor ball
        bool isAnchorRelated = currentEditMode.IsAnchorRelated();
        foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
        {
            Animator anim = anchor.GetComponentInChildren<Animator>();
            anim.SetBool(editingString, isAnchorRelated);
        }

        AnchorManager.AlternatePanels(!isAnchorRelated);

        // enable/disable anchor path
        if (AnchorManager.Instance.SelectedAnchor)
            AnchorManager.Instance.SelectedAnchor.SetLinesActive(isAnchorRelated);
    }

    private void SetEditRotation(int value)
    {
        editRotation = value;

        ReferenceManager.Instance.PlacementPreview.UpdateRotation();
    }

    #endregion

    public void InvokeOnPlay() => OnPlay?.Invoke();

    public void InvokeOnEdit() => OnEdit?.Invoke();

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