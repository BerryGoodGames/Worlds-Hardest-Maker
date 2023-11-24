using System;
using MyBox;
using UnityEngine;

public class EditModeManager : MonoBehaviour
{
    public static EditModeManager Instance { get; private set; }

    #region Variables & properties

    [SerializeField] [SearchableEnum] [InitializationField] private EditMode startEditMode;
    private EditMode currentEditMode;
    private EditMode? prevEditMode;

    public EditMode CurrentEditMode
    {
        get => currentEditMode;
        set
        {
            if (!LevelSessionManager.Instance.IsEdit) return;

            currentEditMode = value;

            // invoke OnEditModeChanged
            if (prevEditMode != null && prevEditMode != currentEditMode) OnEditModeChange?.Invoke();
            prevEditMode = currentEditMode;

            // select edit mode in toolbar
            ToolbarManager.SelectEditMode(value);

            // enable/disable outlines and panel when switching to/away from anchors or anchor ball
            bool isAnchorRelated = currentEditMode.IsAnchorRelated();
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                Animator anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool(editingString, isAnchorRelated);
            }

            if (isAnchorRelated && AnchorManager.Instance.SelectedAnchor) ReferenceManager.Instance.AnchorBallContainer.BallFadeOut();
            else ReferenceManager.Instance.AnchorBallContainer.BallFadeIn();

            // open corresponding panel
            PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
            PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
            PanelManager.Instance.SetPanelHidden(isAnchorRelated ? anchorPanel : levelSettingsPanel, false);

            // enable/disable anchor path
            if (AnchorManager.Instance.SelectedAnchor) AnchorManager.Instance.SelectedAnchor.SetLinesActive(isAnchorRelated);
        }
    }

    [field: SerializeField] [field: ReadOnly] public bool Editing { get; set; }
    public bool Playing
    {
        get => !Editing;
        set => Editing = !value;
    }

    [SerializeField] [ReadOnly] private int editRotation = 270;

    public int EditRotation
    {
        get => editRotation;
        set 
        {
            editRotation = value;
            ReferenceManager.Instance.PlacementPreview.UpdateRotation();
        }
    }

    #endregion

    #region Events

    public event Action OnPlay;
    public event Action OnEdit;
    private static readonly int editingString = Animator.StringToHash("Editing");
    public event Action OnEditModeChange;

    #endregion

    public void InvokeOnPlay() => OnPlay?.Invoke();

    public void InvokeOnEdit() => OnEdit?.Invoke();

    private void Start()
    {
        if (!LevelSessionManager.Instance.IsEdit) return;

        CurrentEditMode = startEditMode;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}