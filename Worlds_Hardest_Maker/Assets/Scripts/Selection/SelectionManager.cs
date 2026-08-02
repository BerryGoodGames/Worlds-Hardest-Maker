using System.Collections.Generic;
using System.Linq;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

/// <summary>
///     Methods for filling: GetFillRange, FillArea, GetBounds, GetBoundsMatrix
///     <para>Attach to game manager</para>
/// </summary>
public partial class SelectionManager : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private RectTransform selectionOptions;
    [SerializeField] [InitializationField] [Required] private MouseOverUIRect fillMouseOver;
    [SerializeField] [InitializationField] [Required] private PlacementPreviewCoordinator placementPreview;
    
    private GameObject selectionOutline;
    private LineAnimator selectionOutlineAnim;
    public bool Selecting { get; private set; }
    
    public static List<Vector2> CurrentSelectionRange => SelectionStart == null || SelectionEnd == null ? null : GetCurrentFillRange();
    
    public static SelectionManager Instance { get; private set; }
    
    private Vector2 prevStart;
    private Vector2 prevEnd;
    public static Vector2? SelectionStart;
    public static Vector2? SelectionEnd;
    
    private IObjectResolver diContainer;
    
    private EventBus eventBus;

    private IMouseService mouseService;
    
    [Inject]
    private void Construct(IObjectResolver diContainer, EventBus eventBus, IMouseService mouseService)
    {
        this.diContainer = diContainer;
        this.eventBus = eventBus;
        this.mouseService = mouseService;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => OnCancelClicked();
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => OnCancelClicked();
    private void OnEditModeChange(EditModeChangeEvent evt) => UpdateFillPreviews();
    
    private void Update()
    {
        if (!LevelSessionManager.Instance.IsEdit
            || AnchorAttachManager.Instance.InAttachMode) return;
        
        if (KeyBinds.GetKeyBind("Editor_Select")
            && !LevelSessionEditManager.Instance.Playing
            && !EventSystem.current.IsPointerOverGameObject()) Selecting = true;
        
        // update selection markings
        if (!LevelSessionEditManager.Instance.Playing
            && Selecting
            && mouseService.MouseDragStart != null
            && mouseService.MouseDragCurrent != null)
        {
            (Vector2 start, Vector2 end) = mouseService.GetDragPositions();
            
            // disable normal placement preview
            placementPreview.Hide();
            
            if (KeyBinds.GetKeyBindDown("Editor_Select")) OnStartSelect(start);
            else if (KeyBinds.GetKeyBindUp("Editor_Select")) OnAreaSelected(start, end);
            
            if (!prevStart.Equals(start) || !prevEnd.Equals(end)) OnAreaSelectionChanged(start, end);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) OnCancelClicked();
    }
    
    private void LateUpdate()
    {
        if (mouseService.MouseDragStart == null || mouseService.MouseDragCurrent == null) return;
        
        prevStart = ((Vector2)mouseService.MouseDragStart).ConvertToGrid();
        prevEnd = ((Vector2)mouseService.MouseDragCurrent).ConvertToGrid();
    }
    
    private void Start()
    {
        fillMouseOver.OnHovered += SetPreviewVisible;
        fillMouseOver.OnUnhovered += SetPreviewInvisible;
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
        
        UpdateFillPreviews();
    }
    
    private void OnStartSelect(Vector2 start)
    {
        // called when mouse button was pressed and user starts selecting
        InitSelectionOutline(start);
        
        selectionOptions.gameObject.SetActive(false);
        
        MenuManager.Instance.BlockMenu = true;
    }
    
    #region Get bounds
    
    // get bounds of multiple points (in matrix)
    private static (Vector2 lowest, Vector2 highest) GetBounds(List<Vector2> points)
    {
        Vector2 lowest = points[0];
        Vector2 highest = points[0];
        
        foreach (Vector2 pos in points)
        {
            if (lowest.x > pos.x) lowest.x = pos.x;
            if (lowest.y > pos.y) lowest.y = pos.y;
            if (highest.x < pos.x) highest.x = pos.x;
            if (highest.y < pos.y) highest.y = pos.y;
        }
        
        return (lowest, highest);
    }
    
    public static (Vector2 lowest, Vector2 highest) GetBounds(params Vector2[] points) => GetBounds(points.ToList());
    
    public static (Vector2Int lowest, Vector2Int highest) GetBoundsMatrix(List<Vector2> points)
    {
        (Vector2 lowest, Vector2 highest) = GetBounds(points);
        return (Vector2Int.CeilToInt(lowest), Vector2Int.FloorToInt(highest));
    }
    
    private static (Vector2Int lowest, Vector2Int highest) GetBoundsMatrix(params Vector2[] points) => GetBoundsMatrix(points.ToList());
    
    #endregion
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
    }
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}