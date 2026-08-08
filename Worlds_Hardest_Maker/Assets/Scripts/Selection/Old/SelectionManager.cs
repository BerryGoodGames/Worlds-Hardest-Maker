using UnityEngine;
using VContainer;

/// <summary>
///     Methods for filling: GetFillRange, FillArea, GetBounds, GetBoundsMatrix
///     <para>Attach to game manager</para>
/// </summary>
public partial class SelectionManager : MonoBehaviour, IFillRangeProvider
{
    public bool Selecting { get; private set; }
    
    public static SelectionManager Instance { get; private set; }
    
    private IObjectResolver diContainer;
    private EventBus eventBus;
    private ISelectionState selectionStateService;

    [SerializeField] private SelectionOptionsPanelController optionsPanelController;
    [Space] [SerializeField] private SelectionPreviewController previewController;
    [Space] [SerializeField] private SelectionOutlineController outlineController;
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    [Inject]
    private void Construct(IObjectResolver diContainer, EventBus eventBus, ISelectionState selectionStateService)
    {
        this.diContainer = diContainer;
        this.eventBus = eventBus;
        this.selectionStateService = selectionStateService;
        
        optionsPanelController.SetEventBus(eventBus);
        previewController.Initialize(eventBus, diContainer, this);
        outlineController.SetEventBus(eventBus);
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Subscribe<SelectionCancelledEvent>(OnSelectionCancelled);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => ClearSelection();
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => ClearSelection();
    private void OnSelectionCancelled(SelectionCancelledEvent evt) => ClearSelection();
    
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