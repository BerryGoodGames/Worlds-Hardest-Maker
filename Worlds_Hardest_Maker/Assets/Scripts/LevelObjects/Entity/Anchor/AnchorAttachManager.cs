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
    private IPanelService panelService;
    
    [Inject]
    private void Construct(EventBus eventBus,
        IPanelService panelService)
    {
        this.eventBus = eventBus;
        this.panelService = panelService;
        
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
        
        panelService.SetPanelHidden(anchorAttachButtonController, true);
        panelService.SetPanelHidden(anchorAttachExitButtonController, false, false);
        
        panelService.SetPanelOpen(anchorPanelController, false, false);
        
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
            panelService.SetPanelHidden(anchorAttachButtonController, false, false);
        }
        
        panelService.SetPanelHidden(anchorAttachExitButtonController, true);
        if (!isModeAnchorRelated)
        {
            panelService.SetPanelHidden(levelSettingsPanelController, false, false);
            panelService.SetPanelHidden(testingOptionsPanelController, false, false);
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