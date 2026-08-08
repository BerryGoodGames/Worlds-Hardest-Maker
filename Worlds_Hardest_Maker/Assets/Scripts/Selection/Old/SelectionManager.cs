using MyBox;
using NaughtyAttributes;
using UnityEngine;
using VContainer;

/// <summary>
///     Methods for filling: GetFillRange, FillArea, GetBounds, GetBoundsMatrix
///     <para>Attach to game manager</para>
/// </summary>
public partial class SelectionManager : MonoBehaviour, IFillRangeProvider
{
    [SerializeField] [InitializationField] [Required] private RectTransform selectionOptions;
    [SerializeField] [InitializationField] [Required] private MouseOverUIRect fillMouseOver;
    [SerializeField] [InitializationField] [Required] private PlacementPreviewCoordinator placementPreview;
    
    public bool Selecting { get; private set; }
    
    public static SelectionManager Instance { get; private set; }
    
    private IObjectResolver diContainer;
    private EventBus eventBus;
    private ISelectionState selectionStateService;

    private SelectionPreviewController previewController;
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
        
        previewController = new(eventBus, diContainer, this);
        outlineController.SetEventBus(eventBus);
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Subscribe<SelectionStartedEvent>(OnSelectionStarted);
        eventBus.Subscribe<SelectionEndedEvent>(OnSelectionEnded);
        eventBus.Subscribe<SelectionCancelledEvent>(OnSelectionCancelled);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => ClearSelection();
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => ClearSelection();
    private void OnSelectionStarted(SelectionStartedEvent evt) => OnSelectionStarted();
    private void OnSelectionEnded(SelectionEndedEvent evt) => OnAreaSelected(evt.Start, evt.End);
    private void OnSelectionCancelled(SelectionCancelledEvent evt) => ClearSelection();

    private void Start()
    {
        fillMouseOver.OnHovered += previewController.SetPreviewVisible;
        fillMouseOver.OnUnhovered += previewController.SetPreviewInvisible;
    }
    
    private void OnAreaSelected(Vector2 start, Vector2 end)
    {
        // called when mouse button was released and area was selected
        selectionOptions.gameObject.SetActive(true);
        float width = end.x - start.x;
        float height = end.y - start.y;
        
        Vector2 position = new(width < 0 ? end.x - 0.5f : end.x + 0.5f, height < 0 ? end.y - 0.5f : end.y + 0.5f);
        UIAttachToPoint posController = selectionOptions.GetComponent<UIAttachToPoint>();
        
        posController.Point = position;
        
        selectionOptions.pivot = new(width > 0 ? 0 : 1, height > 0 ? 0 : 1);
        
        previewController.UpdateFillPreviews();
    }
    
    private void OnSelectionStarted()
    {
        selectionOptions.gameObject.SetActive(false);
        
        MenuManager.Instance.BlockMenu = true;
    }
    
    private void OnDestroy()
    {
        previewController.Dispose();
        outlineController.Dispose();
        
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Unsubscribe<SelectionStartedEvent>(OnSelectionStarted);
        eventBus.Unsubscribe<SelectionEndedEvent>(OnSelectionEnded);
        eventBus.Unsubscribe<SelectionCancelledEvent>(OnSelectionCancelled);
        
        fillMouseOver.OnHovered -= previewController.SetPreviewVisible;
        fillMouseOver.OnUnhovered -= previewController.SetPreviewInvisible;
    }
}