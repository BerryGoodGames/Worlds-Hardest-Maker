using JetBrains.Annotations;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using VContainer;

public class LevelSessionEditManager : MonoBehaviour
{
    public static LevelSessionEditManager Instance { get; private set; }
    
    #region Variables & properties
    
    [SerializeField] [Required] [InitializationField] private EditMode startEditMode;
    private EditMode currentEditMode;
    [CanBeNull] private EditMode prevEditMode;
    
    public EditMode CurrentEditMode
    {
        get => currentEditMode;
        set
        {
            if (!LevelSessionManager.Instance.IsEdit) return;
            
            currentEditMode = value;
            
            // invoke OnEditModeChanged
            if (prevEditMode != null && prevEditMode != currentEditMode)
            {
                eventBus.Fire(new EditModeChangeEvent(currentEditMode));
            }
            prevEditMode = currentEditMode;
            
            // select edit mode in toolbar
            ToolbarManager.SelectEditMode(value);
            
            // enable/disable outlines and panel when switching to/away from anchors or ball
            bool isAnchorRelated = currentEditMode.Attributes.IsAnchorRelated;
            bool inAttachMode = AnchorAttachManager.Instance.InAttachMode;
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                Animator anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool(editingString, isAnchorRelated);
            }
            
            if (AnchorManager.Instance.SelectedAnchor)
                AnchorManager.Instance.SelectedAnchor.GetComponent<Animator>().SetBool(editingString, isAnchorRelated || inAttachMode);
            
            // open corresponding panel
            PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
            PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
            PanelController anchorAttachButton = ReferenceManager.Instance.AnchorAttachButtonController;
            PanelController anchorAttachExitButton = ReferenceManager.Instance.AnchorAttachExitButtonController;
            
            if (!AnchorAttachManager.Instance.InAttachMode)
            {
                if (isAnchorRelated)
                {
                    PanelManager.Instance.SetPanelHidden(anchorPanel, false);
                    
                    if (AnchorManager.Instance.SelectedAnchor)
                    {
                        PanelManager.Instance.SetPanelHidden(
                            AnchorAttachManager.Instance.InAttachMode ? anchorAttachExitButton : anchorAttachButton, false, false
                        );
                    }
                }
                else PanelManager.Instance.SetPanelHidden(levelSettingsPanel, false);
            }
            
            // enable/disable anchor path
            if (AnchorManager.Instance.SelectedAnchor && !AnchorAttachManager.Instance.InAttachMode)
                AnchorManager.Instance.SelectedAnchor.SetLinesActive(isAnchorRelated);
        }
    }
    
    [field: SerializeField] [field: MyBox.ReadOnly] public bool Editing { get; set; }
    
    public bool Playing
    {
        get => !Editing;
        set => Editing = !value;
    }
    
    [field: SerializeField] [field: MyBox.ReadOnly] public bool InPlaytest { get; set; }
    
    
    [SerializeField] [MyBox.ReadOnly] private int editRotation = 270;
    
    public int EditRotation
    {
        get => editRotation;
        set
        {
            editRotation = value;
            eventBus.Fire(new EditRotationChangeEvent(value));
        }
    }
    
    #endregion
    
    private static readonly int editingString = Animator.StringToHash("Editing");
    
    [Inject] private EventBus eventBus;
    
    private void Start()
    {
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        CurrentEditMode = startEditMode;
        
        eventBus.Fire(new EditModeInitializedEvent());
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}