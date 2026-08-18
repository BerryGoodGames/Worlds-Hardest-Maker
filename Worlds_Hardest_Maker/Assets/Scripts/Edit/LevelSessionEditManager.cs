using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;

public class LevelSessionEditManager : MonoBehaviour
{
    public static LevelSessionEditManager Instance { get; private set; }
    
    [SerializeField] [MustBeAssigned] [InitializationField] private EditMode startEditMode;

    [field: SerializeField]
    [field: ReadOnly]
    public bool IsEditing { get; set; } = true;
    
    public bool IsPlaying
    {
        get => !IsEditing;
        set => IsEditing = !value;
    }
    
    [field: SerializeField] [field: ReadOnly] public bool IsPlaytesting { get; set; }
    
    [SerializeField] [ReadOnly] private int editRotation = 270;
    
    public int EditRotation
    {
        get => editRotation;
        set
        {
            editRotation = value;
            eventBus.Fire(new EditRotationChangeEvent(value));
        }
    }
    
    [Inject] private EventBus eventBus;
    
    [CanBeNull] private EditMode prevEditMode;
    public EditMode CurrentEditMode { get; private set; }
    
    public void SetEditMode(EditMode editMode)
    {
        if (!LevelSessionManager.Instance.IsEdit) return;

        int editingString = Animator.StringToHash("Editing");

        CurrentEditMode = editMode;

        // invoke OnEditModeChanged
        if (prevEditMode != null && prevEditMode != CurrentEditMode)
        {
            eventBus.Fire(new EditModeChangeEvent(CurrentEditMode));
        }

        prevEditMode = CurrentEditMode;

        // select edit mode in toolbar
        ToolbarManager.SelectEditMode(editMode);

        // enable/disable outlines and panel when switching to/away from anchors or ball
        bool isAnchorRelated = CurrentEditMode.Attributes.IsAnchorRelated;
        bool inAttachMode = AnchorAttachManager.Instance.InAttachMode;
        foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
        {
            Animator anim = anchor.GetComponentInChildren<Animator>();
            anim.SetBool(editingString, isAnchorRelated);
        }

        if (AnchorManager.Instance.SelectedAnchor)
        {
            AnchorManager.Instance.SelectedAnchor.GetComponent<Animator>()
                .SetBool(editingString, isAnchorRelated || inAttachMode);
        }

        // enable/disable anchor path
        if (AnchorManager.Instance.SelectedAnchor && !AnchorAttachManager.Instance.InAttachMode)
        {
            AnchorManager.Instance.SelectedAnchor.SetLinesActive(isAnchorRelated);
        }
    }

    private void Start()
    {
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        SetEditMode(startEditMode);
        
        eventBus.Fire(new EditModeInitializedEvent());
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}