using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class KeyManager : MonoBehaviour, IKeyManager, ILevelObjectPlacer, ILevelObjectSerializer
{
    public static KeyManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController grayKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController redKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController blueKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController greenKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private KeyController yellowKeyPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform keyContainer;
    
    [ReadOnly] public List<KeyController> collectedKeys = new();
    public IReadOnlyList<KeyController> CollectedKeys => collectedKeys;
    
    private EventBus eventBus;
    [Inject] private KeyQueryService keyQueryService;
    [Inject] private KeyPlacementRules placementRules;
    private KeyFactory keyFactory;
    [Inject] private IAttachmentService attachmentService;
    [Inject] private ILevelObjectRegistry<KeyController> keyRegistry;
    
    [Inject]
    private void Construct(EventBus eventBus, KeyFactory keyFactory)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
        eventBus.Subscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Subscribe<SetupPlaySceneEvent>(OnSetupPlayScene);

        this.keyFactory = keyFactory;
        keyFactory.Initialize(grayKeyPrefab, redKeyPrefab, blueKeyPrefab, greenKeyPrefab, yellowKeyPrefab, keyContainer);
    }
    
    private void OnPlayAgain(PlayAgainEvent evt) => collectedKeys.Clear();
    private void OnResetLevel(ResetLevelEvent evt) => ActivateAnimations();
    private void OnSetupPlayScene(SetupPlaySceneEvent evt) => ActivateAnimations();
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
        eventBus.Unsubscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Unsubscribe<SetupPlaySceneEvent>(OnSetupPlayScene);
    }
    
    private void RemoveKeyInSheet(Vector2 position, ISheet sheet)
    {
        KeyController key = keyQueryService.Find(position, sheet);
        
        if (key == null) return;
        
        // destroy
        DestroyImmediate(key.transform.gameObject);
    }
    
    public KeyController CreateNew(Vector2 position, ISheet sheet, KeyColor keyColor)
    {
        if (!placementRules.CanPlaceInSheet(position, sheet)) return null;
        
        // remove other key (which has mby other color)
        RemoveKeyInSheet(position, sheet);
        
        KeyController key = keyFactory.Create(position, sheet, keyColor);
        
        key.Color = keyColor;
        
        if(sheet is AnchorSheet anchorSheet) attachmentService.Attach(key, anchorSheet.Anchor);
        
        return key;
    }

    public bool AllKeysCollected(KeyColor color)
    {
        // check if every key of specific color is picked up
        foreach (KeyController key in keyRegistry.All)
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
    
    public void ActivateAnimations() => keyRegistry.All.ForEach(key => key.ActivateAnimation());
    
    public void CollectKey(KeyController key)
    {
        collectedKeys.Add(key);
    }
    
    public void UncollectKey(KeyController key)
    {
        collectedKeys.Remove(key);
    }
    
    public void RemoveCollectedKeyNulls()
    {
        collectedKeys.RemoveAll(key => key == null);
    }

    public void ClearCollectedKeys()
    {
        collectedKeys.Clear();
    }

    public bool CanHandle(EditMode editMode)
    {
        return editMode is KeyMode;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        ISheet sheet = PlaceManager.GetCurrentSheet();
        KeyColor keyColor = ((KeyMode)request.EditMode).KeyColor;

        KeyController result = CreateNew(gridPosition, sheet, keyColor);

        return PlacementResult.FromController(result);
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();
        foreach (KeyController key in keyRegistry.All)
        {
            if (key.IsAttached) continue;
            
            KeyData keyData = new(key);
            levelData.Add(keyData);
        }
        
        return levelData;
    }
}