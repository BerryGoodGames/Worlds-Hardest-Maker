using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    [FormerlySerializedAs("editing")] public bool Editing = true;

    public bool Playing
    {
        get => !Editing;
        set => Editing = !value;
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

    public event Action OnPlay;
    public event Action OnEdit;
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
        GameObject[] tools = ToolbarManager.Tools;
        foreach (GameObject tool in tools)
        {
            Tool t = tool.GetComponent<Tool>();
            if (t.ToolName == value)
            {
                // avoid recursion
                t.SwitchGameMode(false);
            }
        }

        // enable/disable outlines and window when switching to/away from anchors/balls
        if (currentEditMode is EditMode.ANCHOR or EditMode.BALL)
        {
            // enable stuff
            // ReferenceManager.Instance.ballWindows.SetActive(true);

            if (AnchorManagerOld.Instance.SelectedAnchor == null) return;

            // enable lines
            AnchorManagerOld.Instance.SelectedPathControllerOld.DoDrawLines = true;
            AnchorManagerOld.Instance.SelectedPathControllerOld.DrawLines();

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
            // ReferenceManager.Instance.BallWindows.SetActive(false);

            if (AnchorManagerOld.Instance.SelectedAnchor == null) return;

            // disable lines
            AnchorManagerOld.Instance.SelectedPathControllerOld.DoDrawLines = false;
            AnchorManagerOld.Instance.SelectedPathControllerOld.ClearLines();

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

        ReferenceManager.Instance.PlacementPreview.GetComponent<PreviewController>().UpdateRotation();
    }

    #endregion

    public void InvokeOnPlay()
    {
        OnPlay?.Invoke();
    }

    public void InvokeOnEdit()
    {
        OnEdit?.Invoke();
    }

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