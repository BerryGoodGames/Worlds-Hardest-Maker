using MyBox;
using UnityEngine;
using VContainer;

public partial class AnchorAttachManager : MonoBehaviour
{
    public static AnchorAttachManager Instance { get; private set; }
    
    [ReadOnly] public bool InAttachMode;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController levelSettingsPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController testingOptionsPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorPanelController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorAttachButtonController;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelController anchorAttachExitButtonController;
    
    private static readonly int editingString = Animator.StringToHash("Editing");
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        if (InAttachMode) ExitAttachMode();
    }
    
    public void EnterAttachMode()
    {
        if (LevelSessionEditManager.Instance.IsPlaying
            || !LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated
            || AnchorManager.Instance.SelectedAnchor == null
            || AnchorPositionInputEditManager.Instance.IsEditing) return;
        
        PanelManager.Instance.SetPanelHidden(anchorAttachButtonController, true);
        PanelManager.Instance.SetPanelHidden(anchorAttachExitButtonController, false, false);
        
        PanelManager.Instance.SetPanelOpen(anchorPanelController, false, false);
        
        InAttachMode = true;
        
        LevelSessionEditManager.Instance.SetEditMode(EditModeManager.Ball);
        
        HighlightAnchor(AnchorManager.Instance.SelectedAnchor);
        
        eventBus.Fire(new EnterAnchorAttachEvent());
    }
    
    public void ExitAttachMode()
    {
        bool isModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
        
        if (LevelSessionEditManager.Instance.IsEditing)
        {
            PanelManager.Instance.SetPanelHidden(anchorAttachButtonController, false, false);
        }
        
        PanelManager.Instance.SetPanelHidden(anchorAttachExitButtonController, true);
        if (!isModeAnchorRelated)
        {
            PanelManager.Instance.SetPanelHidden(levelSettingsPanelController, false, false);
            PanelManager.Instance.SetPanelHidden(testingOptionsPanelController, false, false);
        }
        
        InAttachMode = false;
        
        if (AnchorManager.Instance.SelectedAnchor)
        {
            AnchorManager.Instance.SelectedAnchor.GetComponent<Animator>().SetBool(editingString, isModeAnchorRelated);
            AnchorManager.Instance.SelectedAnchor.SetLinesActive(isModeAnchorRelated);
        }
        
        Dehighlight(AnchorManager.Instance.SelectedAnchor);
        
        eventBus.Fire(new ExitAnchorAttachEvent());
    }
    
    public static Transform GetCurrentAnchorContainer() => Instance.InAttachMode ? AnchorManager.Instance.SelectedAnchor.AttachmentContainer : null;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
    }
}