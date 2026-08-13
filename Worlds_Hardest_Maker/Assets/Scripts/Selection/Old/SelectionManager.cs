using UnityEngine;
using VContainer;

public partial class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    
    private IObjectResolver diContainer;
    private EventBus eventBus;
    private ISelectionStateService selectionStateService;
    private ISelectionAreaProvider selectionAreaProvider;

    [SerializeField] private SelectionOptionsPanelController optionsPanelController;
    [Space] [SerializeField] private SelectionPreviewController previewController;
    [Space] [SerializeField] private SelectionOutlineController outlineController;
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    [Inject]
    private void Construct(IObjectResolver diContainer, EventBus eventBus, ISelectionStateService selectionStateService, ISelectionAreaProvider selectionAreaProvider)
    {
        this.diContainer = diContainer;
        this.eventBus = eventBus;
        this.selectionStateService = selectionStateService;
        this.selectionAreaProvider = selectionAreaProvider;
        
        optionsPanelController.SetEventBus(eventBus);
        previewController.Initialize(eventBus, diContainer, selectionAreaProvider);
        outlineController.SetEventBus(eventBus);
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Subscribe<SelectionCancelledEvent>(OnSelectionCancelled);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => selectionStateService.ClearSelection();
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => selectionStateService.ClearSelection();
    private void OnSelectionCancelled(SelectionCancelledEvent evt) => selectionStateService.ClearSelection();
    
    private void OnDestroy()
    {
        optionsPanelController.Dispose();
        previewController.Dispose();
        outlineController.Dispose();
        
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Unsubscribe<SelectionCancelledEvent>(OnSelectionCancelled);
    }
}