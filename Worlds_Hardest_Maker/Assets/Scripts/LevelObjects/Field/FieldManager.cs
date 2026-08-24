using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using WorldsHardestMaker.Selection;

public partial class FieldManager : MonoBehaviour, 
    IManager<FieldController>, 
    ILevelObjectManager
{
    public static FieldManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform playerContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform fieldContainer;   
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform coinContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform keyContainer;
    
    [Inject] private IObjectResolver diContainer;
    
    public FieldController SetInSheet(ManagerParameters args)
    {
        FieldController fieldAtPosition = GetInSheet(args.Position, args.Sheet);
        if (fieldAtPosition is not null && fieldAtPosition.FieldMode == args.FieldMode) return null;
        
        // remove any field at pos
        Remove(args.Position, true, args.Sheet);
        
        // place field according to edit mode
        FieldController field = ((IManager<FieldController>)this).InstantiateInSheet(args);
        
        if (field.TryGetComponent(out ColorCalibration calibration))
            calibration.Apply(LevelSessionEditManager.Instance.IsPlaying && SettingsManager.Instance.OneColorSafeFields);
        
        // remove player if at changed pos
        if (!args.FieldMode.IsStartFieldForPlayer) PlayerManager.Instance.RemoveAtPosIntersectInSheet(args.Position, args.Sheet);
        
        if (CoinManager.CannotPlaceFields.Contains(args.FieldMode))
            // remove coin if wall is placed
            GameManager.Instance.RemoveObjectInContainerIntersect(args.Position, coinContainer);
        
        if (KeyManager.CannotPlaceFields.Contains(args.FieldMode))
            // remove key if wall is placed
            GameManager.Instance.RemoveObjectInContainerIntersect(args.Position, keyContainer);
        
        return field;
    }
    
    public FieldController GetInSheet(Vector2 position, AnchorController sheet)
    {
        // get all collisions from layers Field and Void
        Collider2D[] collidedGameObjects = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Field)
            .Concat(Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Void)).ToArray();
        
        foreach (Collider2D c in collidedGameObjects)
        {
            // check if field
            if (!c.TryGetComponent(out FieldController f)) continue;
            
            if (IManager.IsInSheet(f, sheet)) return f;
        }
        
        return null;
    }
    
    public FieldController Get(Vector2 position)
    {
        // get all collisions from layers Field and Void
        Collider2D[] collidedGameObjects = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Field)
            .Concat(Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Void)).ToArray();
        
        foreach (Collider2D c in collidedGameObjects)
        {
            if (c.TryGetComponent(out FieldController f)) return f;
        }
        
        return null;
    }
    
    public FieldController InstantiateInSheet(ManagerParameters args)
    {
        GameObject prefab = args.FieldMode.Prefab;
        GameObject res = Instantiate(
            prefab, args.Position, Quaternion.Euler(0, 0, args.Rotation),
            args.Sheet == null ? fieldContainer : args.Sheet.AttachmentContainer
        );
        
        diContainer.InjectGameObject(res);
        
        FieldController fieldController = res.GetComponent<FieldController>();
        
        fieldController.Initialize(playerContainer);
        
        fieldController.FieldMode = args.FieldMode;
        
        PlaceManager.Instance.AttachToSheet(res, args.Sheet);
        
        return fieldController;
    }
    
    public bool Remove(Vector2 position, bool updateOutlines = false, [CanBeNull] AnchorController sheet = null)
    {
        FieldController field = GetInSheet(position, sheet);
        
        PlayerController player = PlayerManager.Instance.Player;
        if (player != null && player.CurrentPlatforms.Contains(field))
        {
            print("Calling from remove");
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
        foreach (FieldController neighbor in GetNeighbors(position))
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
    
    public List<FieldController> GetFieldsAtGridPosInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
        };
        
        checkPoses = checkPoses.Distinct().ToArray();
        
        List<FieldController> res = new();
        foreach (Vector2Int checkPosition in checkPoses)
        {
            FieldController field = GetInSheet(checkPosition, sheet);
            if (field != null) res.Add(field);
        }
        
        return res;
    }
    
    public static void UpdateOutlinesInArea(bool hasOutline, SelectionArea area)
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
    
    private static void UpdateOutlinesSingle(Vector2 origin, params Vector2[] directions)
    {
        FieldController lowestField = Instance.Get(Vector2Int.RoundToInt(origin));
        
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

    public bool Place(PlacementRequest request)
    {
        FieldMode mode = (FieldMode)request.EditMode;
        int rotation = request.Rotation;
        Vector2 position = request.Position.ConvertToMatrix();
        
        if (!mode.IsRotatable) rotation = 0;
        
        ManagerParameters args = new()
        {
            Position = position,
            FieldMode = mode,
            Rotation = rotation,
        };

        FieldController placedField = ((IManager<FieldController>)this).Set(args);

        return placedField;
    }

    public LevelObjectController Query(Vector2 position, AnchorController sheet)
    {
        return GetInSheet(position, sheet);
    }

    public bool Remove(Vector2 position, AnchorController sheet)
    {
        return Remove(position, true, sheet);
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