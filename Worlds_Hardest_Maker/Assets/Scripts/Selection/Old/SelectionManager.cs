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
    
    private GameObject selectionOutline;
    private LineAnimator selectionOutlineAnim;
    public bool Selecting { get; private set; }
    
    public static SelectionManager Instance { get; private set; }
    
    // private Vector2 prevStart;
    // private Vector2 prevEnd;
    
    private IObjectResolver diContainer;
    private EventBus eventBus;
    private ISelectionState selectionStateService;

    private SelectionPreviewController previewController;
    
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
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Subscribe<SelectionStartedEvent>(OnSelectionStarted);
        eventBus.Subscribe<SelectionUpdatedEvent>(OnSelectionUpdated);
        eventBus.Subscribe<SelectionEndedEvent>(OnSelectionEnded);
        eventBus.Subscribe<SelectionCancelledEvent>(OnSelectionCancelled);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => OnCancelClicked();
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => OnCancelClicked();
    private void OnSelectionStarted(SelectionStartedEvent evt) => OnStartSelect(evt.Start);
    private void OnSelectionUpdated(SelectionUpdatedEvent evt) => OnAreaSelectionChanged(evt.Start, evt.End);
    private void OnSelectionEnded(SelectionEndedEvent evt) => OnAreaSelected(evt.Start, evt.End);
    private void OnSelectionCancelled(SelectionCancelledEvent evt) => OnCancelClicked();

    private void Start()
    {
        previewController = new(eventBus, diContainer, this);
        
        fillMouseOver.OnHovered += previewController.SetPreviewVisible;
        fillMouseOver.OnUnhovered += previewController.SetPreviewInvisible;
    }
    
    private void OnAreaSelectionChanged(Vector2 start, Vector2 end)
    {
        // called when area selection changed (lol)
        // set selection outline (if u didn't already see)
        AnimSelectionOutline(start, end);
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
    
    private void OnStartSelect(Vector2 start)
    {
        // called when mouse button was pressed and user starts selecting
        InitSelectionOutline(start);
        
        selectionOptions.gameObject.SetActive(false);
        
        MenuManager.Instance.BlockMenu = true;
    }
    
    private void OnDestroy()
    {
        previewController.Dispose();
        
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Unsubscribe<SelectionStartedEvent>(OnSelectionStarted);
        eventBus.Unsubscribe<SelectionUpdatedEvent>(OnSelectionUpdated);
        eventBus.Unsubscribe<SelectionEndedEvent>(OnSelectionEnded);
        eventBus.Unsubscribe<SelectionCancelledEvent>(OnSelectionCancelled);
        
        fillMouseOver.OnHovered -= previewController.SetPreviewVisible;
        fillMouseOver.OnUnhovered -= previewController.SetPreviewInvisible;
    }
}