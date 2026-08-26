using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;

public class KeyManager : MonoBehaviour, ILevelObjectPlacer, ILevelObjectSerializer
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
    [Inject] private KeyQueryService keyQueryService;
    [Inject] private KeyPlacementRules placementRules;
    private KeyFactory keyFactory;
    
    [Inject]
    private void Construct(EventBus eventBus, KeyFactory keyFactory)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);

        this.keyFactory = keyFactory;
        keyFactory.Initialize(grayKeyPrefab, redKeyPrefab, blueKeyPrefab, greenKeyPrefab, yellowKeyPrefab, keyContainer);
    }
    
    private void OnPlayAgain(PlayAgainEvent evt) => CollectedKeys.Clear();
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
    }
    
    private void RemoveKeyInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        KeyController key = keyQueryService.Find(position, sheet);
        
        if (key == null) return;
        
        // un-cache
        Keys.Remove(key);
        
        // destroy
        DestroyImmediate(key.transform.gameObject);
    }
    
    public KeyController CreateNew(Vector2 position, [CanBeNull] AnchorController sheet, KeyColor keyColor)
    {
        if (!placementRules.CanPlaceInSheet(position, sheet)) return null;
        
        // remove other key (which has mby other color)
        RemoveKeyInSheet(position, sheet);
        
        KeyController key = keyFactory.Create(position, sheet, keyColor);
        
        key.Color = keyColor;
        
        PlaceManager.Instance.AttachToSheet(key.gameObject, sheet);
        
        return key;
    }
    
    public bool AllKeysCollected(KeyColor color)
    {
        // check if every key of specific color is picked up
        foreach (KeyController key in Keys)
        {
            if (!key.Collected && key.Color == color) return false;
        }
        
        return true;
    }
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
    
    public void ActivateAnimations() => Keys.ForEach(key => key.ActivateAnimation());

    public bool CanHandle(EditMode editMode)
    {
        return editMode is KeyMode;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        AnchorController sheet = PlaceManager.GetCurrentSheet();
        KeyColor keyColor = ((KeyMode)request.EditMode).KeyColor;

        KeyController result = CreateNew(gridPosition, sheet, keyColor);

        return PlacementResult.FromController(result);
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