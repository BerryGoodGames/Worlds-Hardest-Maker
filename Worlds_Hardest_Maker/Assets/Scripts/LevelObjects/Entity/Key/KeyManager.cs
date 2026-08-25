using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class KeyManager : MonoBehaviour, 
    IManager<KeyController>, 
    ILevelObjectManager,
    IKeyQueryService
{
    public static KeyManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController grayKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController redKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController blueKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController greenKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController yellowKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform keyContainer;
    
    [ReadOnly] public List<KeyController> Keys = new();
    [ReadOnly] public List<KeyController> CollectedKeys = new();
    
    [Inject] private IObjectResolver diContainer;
    private EventBus eventBus;
    [Inject] private IKonamiService konamiService;
    [Inject] private IPositionQueryService positionQueryService;
    [Inject] private KeyPlacementRules placementRules;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
    }
    
    private void OnPlayAgain(PlayAgainEvent evt) => CollectedKeys.Clear();
    
    private void RemoveKeyInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        KeyController key = GetInSheet(position, sheet);
        
        if (key == null) return;
        
        // un-cache
        Keys.Remove(key);
        
        // destroy
        DestroyImmediate(key.transform.gameObject);
    }
    
    public KeyController SetInSheet(ManagerParameters args)
    {
        if (!CanPlaceInSheet(args.Position, args.Sheet)) return null;
        
        // remove other key (which has mby other color)
        RemoveKeyInSheet(args.Position, args.Sheet);
        
        KeyController key = InstantiateInSheet(args);
        
        key.Color = args.KeyColor;
        
        PlaceManager.Instance.AttachToSheet(key.gameObject, args.Sheet);
        
        return key;
    }
    
    public KeyController GetInSheet(Vector2 position, AnchorController sheet)
    {
        return positionQueryService.QueryPosition<KeyController>(position, 0.01f, LayerManager.Instance.Layers.Entity,
            "Key", sheet);
    }
    
    public KeyController Get(Vector2 position)
    {
        return positionQueryService.QueryPositionAny<KeyController>(position, 0.01f,
            LayerManager.Instance.Layers.Entity);
    }
    
    public KeyController InstantiateInSheet(ManagerParameters args)
    {
        KeyController key = Instantiate(
            GetPrefabKey(args.KeyColor),
            args.Position, Quaternion.identity,
            args.Sheet == null ? keyContainer : args.Sheet.AttachmentContainer
        );
        
        diContainer.InjectGameObject(key.gameObject);
        
        return key;
    }
    
    public bool IsThereInSheet(Vector2 position, AnchorController sheet) => GetInSheet(position, sheet) != null;
    
    public bool CanPlace(Vector2 position) => CanPlaceInSheet(position, PlaceManager.GetCurrentSheet());

    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet) =>
        placementRules.CanPlaceInSheet(position, sheet);
    
    public bool AllKeysCollected(KeyColor color)
    {
        // check if every key of specific color is picked up
        foreach (KeyController key in Keys)
        {
            if (!key.Collected && key.Color == color) return false;
        }
        
        return true;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
    }
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
    
    public void ActivateAnimations() => Keys.ForEach(key => key.ActivateAnimation());
    
    private KeyController GetPrefabKey(KeyColor color)
    {
        Dictionary<KeyColor, KeyController> prefabs = new()
        {
            { KeyColor.Gray, grayKeyPrefab },
            { KeyColor.Red, redKeyPrefab },
            { KeyColor.Blue, blueKeyPrefab },
            { KeyColor.Green, greenKeyPrefab },
            { KeyColor.Yellow, yellowKeyPrefab },
        };
        
        return prefabs[color];
    }

    public bool CanHandle(EditMode editMode)
    {
        return editMode is KeyMode;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        ManagerParameters args = ManagerParameters.FromCurrentSheet(new()
        {
            Position = gridPosition
        });
        
        args.KeyColor = ((KeyMode)request.EditMode).KeyColor;

        KeyController result = SetInSheet(args);

        return PlacementResult.FromController(result);
    }

    public LevelObjectController Query(Vector2 position, AnchorController sheet)
    {
        return GetInSheet(position, sheet);
    }

    public bool Remove(Vector2 position, AnchorController sheet)
    {
        KeyController key = GetInSheet(position, sheet);

        if (key == null) return false;
        
        key.Delete();
        return true;
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();
        foreach (KeyController key in Keys)
        {
            if (key.IsAttached) continue;
            
            KeyData keyData = new(key);
            levelData.Add(keyData);
        }
        
        return levelData;
    }
}