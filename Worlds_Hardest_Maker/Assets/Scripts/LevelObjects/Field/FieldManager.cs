using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

public partial class FieldManager : MonoBehaviour, IFieldManager, ILevelObjectPlacer, ILevelObjectSerializer
{
    public static FieldManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform playerContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform fieldContainer;   
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform coinContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform keyContainer;

    [Inject] private IObjectResolver diContainer;
    private EventBus eventBus;
    [Inject] private FieldQueryService fieldQueryService;
    private FieldFactory fieldFactory;
    [Inject] private IPlayerProvider playerProvider;

    [Inject]
    private void Construct(EventBus eventBus, FieldFactory fieldFactory)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        
        this.fieldFactory = fieldFactory;
        this.fieldFactory.Initialize(fieldContainer, playerContainer);
    }

    private void OnSwitchToEdit(SwitchToEditEvent evt) => ApplySafeFieldsColor(false);
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        if (SettingsManager.Instance.OneColorSafeFields)
        {
            ApplySafeFieldsColor(true);
        }
    }

    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
    }

    public FieldController CreateNew(Vector2 position, int rotation, ISheet sheet, FieldMode fieldMode)
    {
        FieldController fieldAtPosition = fieldQueryService.Find(position, sheet);
        if (fieldAtPosition is not null && fieldAtPosition.FieldMode == fieldMode) return null;
        
        // remove any field at pos
        Remove(position, sheet, true);
        
        // place field according to edit mode
        FieldController field = fieldFactory.Create(position, rotation, sheet, fieldMode);
        
        if (field.TryGetComponent(out ColorCalibration calibration))
        {
            calibration.Apply(LevelSessionEditManager.Instance.IsPlaying &&
                              SettingsManager.Instance.OneColorSafeFields);
        }
        
        diContainer.Resolve<PlayerStartFieldResolver>().OnFieldPlaced(position, sheet, fieldMode);

        return field;
    }
    
    public bool Remove(Vector2 position, ISheet sheet, bool updateOutlines = false)
    {
        FieldController field = fieldQueryService.Find(position, sheet);
        
        PlayerController player = playerProvider.Player;
        if (player != null && player.CurrentPlatforms.Contains(field))
        {
            field.OnPlayerExited();
        }
        
        bool fieldDestroyed = false;
        
        if (field != null)
        {
            DestroyImmediate(field.gameObject);
            fieldDestroyed = true;
        }
        
        if (!updateOutlines) return fieldDestroyed;
        
        // Update outlines beside removed field
        foreach (FieldController neighbor in GetNeighborsInSheet(position.ConvertToMatrix(), PlaceManager.GetCurrentSheet()))
        {
            if (neighbor.TryGetComponent(out FieldOutline comp)) comp.UpdateOutline();
        }
        
        return fieldDestroyed;
    }
    
    public void ApplySafeFieldsColor(bool oneColor)
    {
        ColorCalibration[] colorCalibrations = fieldContainer.GetComponentsInChildren<ColorCalibration>();
        
        foreach (ColorCalibration field in colorCalibrations) field.Apply(oneColor);
    }
    
    public void UpdateOutlinesInArea(bool hasOutline, SelectionArea area)
    {
        Vector2 lowest = area.Lowest;
        Vector2 highest = area.Highest;
        
        int width = (int)highest.x - (int)lowest.x;
        int height = (int)highest.y - (int)lowest.y;
        
        // update outlines
        if (hasOutline)
        {
            // update lowest and highest field separately cause ray casting
            UpdateOutlinesSingle(lowest, Vector2.left, Vector2.down);
            UpdateOutlinesSingle(highest, Vector2.right, Vector2.up);
            
            // // horizontal
            // cast rays to get all bottom and all top fields of the filling, then update their and their neighbors' outlines
            UpdateOutlinesRayLineWithNeighbors(lowest, Vector2.right, Vector2.down, width);
            UpdateOutlinesRayLineWithNeighbors(highest, Vector2.left, Vector2.up, width);
            
            // // vertical
            // cast rays to get all left and all right fields of the filling, then update their and their neighbors' outlines
            UpdateOutlinesRayLineWithNeighbors(lowest, Vector2.up, Vector2.left, height);
            UpdateOutlinesRayLineWithNeighbors(highest, Vector2.down, Vector2.right, height);
            
            return;
        }
        
        // update fields around fill area
        (Vector2, Vector2, int)[] rays =
        {
            (new(lowest.x - 1, lowest.y - 1), Vector2.right, width + 2),
            (new(lowest.x - 1, lowest.y - 1), Vector2.up, height + 2),
            (new(highest.x + 1, highest.y + 1), Vector2.left, width + 2),
            (new(highest.x + 1, highest.y + 1), Vector2.down, height + 2),
        };
        
        foreach ((Vector2 origin, Vector2 direction, int length) in rays) UpdateOutlinesRayLine(origin, direction, length);
    }
    
    private static void UpdateOutlinesRayLineWithNeighbors(Vector2 origin, Vector2 rayDirection, Vector2 neighborDirection, int length)
    {
        RaycastHit2D[] hits = new RaycastHit2D[length];
        Physics2D.RaycastNonAlloc(origin, rayDirection, hits, length);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;
            
            if (hit.transform.TryGetComponent(out FieldOutline foComp)) foComp.UpdateOutline(neighborDirection, true);
        }
    }
    
    private void UpdateOutlinesSingle(Vector2 origin, params Vector2[] directions)
    {
        FieldController lowestField = fieldQueryService.FindAny(Vector2Int.RoundToInt(origin));
        
        if (!lowestField.TryGetComponent(out FieldOutline foComp)) return;
        
        foreach (Vector2 direction in directions) foComp.UpdateOutline(direction, true);
    }
    
    private static void UpdateOutlinesRayLine(Vector2 origin, Vector2 direction, int length)
    {
        RaycastHit2D[] currentHits = new RaycastHit2D[length];
        _ = Physics2D.RaycastNonAlloc(origin, direction, currentHits, length);
        
        foreach (RaycastHit2D r in currentHits)
        {
            if (r.collider == null) continue;
            
            GameObject collider = r.collider.gameObject;
            
            if (collider.TryGetComponent(out FieldOutline outline)) outline.UpdateOutline();
        }
    }
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
    
    public bool CanHandle(EditMode editMode)
    {
        return editMode is FieldMode;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        FieldMode mode = (FieldMode)request.EditMode;
        int rotation = request.Rotation;
        Vector2 position = request.Position.ConvertToMatrix();
        
        if (!mode.IsRotatable) rotation = 0;

        FieldController placedField = CreateNew(position, rotation, PlaceManager.GetCurrentSheet(), mode);
        
        return PlacementResult.FromController(placedField);
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();
        foreach (Transform field in fieldContainer)
        {
            FieldController controller = field.GetComponent<FieldController>();
            
            if (controller.IsAttached) continue;
            
            FieldData fieldData = new(controller);
            levelData.Add(fieldData);
        }
        
        return levelData;
    }
}