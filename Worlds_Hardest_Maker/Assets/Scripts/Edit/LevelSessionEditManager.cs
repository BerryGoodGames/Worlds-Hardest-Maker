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
        
        CurrentEditMode = editMode;

        // invoke OnEditModeChanged
        if (prevEditMode != null && prevEditMode != CurrentEditMode)
        {
            eventBus.Fire(new EditModeChangeEvent(CurrentEditMode));
        }

        prevEditMode = CurrentEditMode;
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