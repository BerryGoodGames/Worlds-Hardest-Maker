using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class CoinManager : MonoBehaviour, ILevelObjectPlacer, ILevelObjectSerializer
{
    public static CoinManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [MustBeAssigned] private CoinController coinPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform coinContainer;
    
    [ReadOnly] public List<CoinController> Coins = new();
    [ReadOnly] public List<CoinController> CollectedCoins = new();
    
    private int TotalCoins => Coins.Count;
    
    public int CoinsNeededFinal =>
        Mathf.Min(LevelSettings.Instance.IsCoinsNeededLimited ? LevelSettings.Instance.CoinsNeeded : TotalCoins, TotalCoins);
    
    [Inject] private IObjectResolver diContainer;
    private EventBus eventBus;
    [Inject] private IPositionQueryService positionQueryService;
    [Inject] private CoinQueryService coinQueryService;
    [Inject] private CoinPlacementRules placementRules;
    private CoinFactory coinFactory;
    [Inject] private IAttachmentService attachmentService;
    
    [Inject]
    private void Construct(EventBus eventBus, CoinFactory coinFactory)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);

        this.coinFactory = coinFactory;
        coinFactory.Initialize(coinPrefab, coinContainer);
    }
    
    private void OnPlayAgain(PlayAgainEvent evt) => CollectedCoins.Clear();
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
    }
    
    public CoinController CreateNew(Vector2 position, ISheet sheet)
    {
        Vector2 gridPosition = position.ConvertToGrid();
        
        if (!placementRules.CanPlaceInSheet(gridPosition, sheet)) return null;

        CoinController coin = coinFactory.Create(gridPosition, sheet);

        if (sheet is AnchorSheet anchorSheet) attachmentService.Attach(coin, anchorSheet.Anchor);
        
        return coin;
    }
    
    public bool AllCoinsCollected() => CollectedCoins.Count >= CoinsNeededFinal;
    
    public void ActivateAnimations() => Coins.ForEach(coin => coin.ActivateAnimation());
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
    
    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Coin;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        
        CoinController result = CreateNew(gridPosition, PlaceManager.GetCurrentSheet());

        return PlacementResult.FromController(result);
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();
        foreach (CoinController coin in Coins)
        {
            if (coin.IsAttached) continue;
            
            CoinData coinData = new(coin);
            levelData.Add(coinData);
        }
        
        return levelData;
    }
}