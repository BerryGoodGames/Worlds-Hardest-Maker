using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class CoinManager : MonoBehaviour, 
    IManager<CoinController>, 
    ILevelObjectManager
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
    
    [Inject]
    private void Construct(EventBus eventBus, CoinFactory coinFactory)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);

        this.coinFactory = coinFactory;
        coinFactory.Initialize(coinPrefab, coinContainer);
    }
    
    public bool CanPlace(Vector2 position) => CanPlaceInSheet(position, PlaceManager.GetCurrentSheet());

    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet) => placementRules.CanPlaceInSheet(position, sheet);
    
    public CoinController SetInSheet(ManagerParameters args)
    {
        Vector2 matrixPosition = args.Position.ConvertToGrid();
        
        if (!CanPlaceInSheet(matrixPosition, args.Sheet)) return null;
        
        CoinController coin = coinFactory.Create(args);
        
        PlaceManager.Instance.AttachToSheet(coin.gameObject, args.Sheet);
        
        return coin;
    }
    
    public void UncollectCoinAtPos(Vector2 position)
    {
        for (int i = CollectedCoins.Count - 1; i >= 0; i--)
        {
            CoinController c = CollectedCoins[i];
            if (c.InitialPosition == position) CollectedCoins.Remove(c);
        }
    }
    
    public bool AllCoinsCollected() => CollectedCoins.Count >= CoinsNeededFinal;
    
    public void ActivateAnimations() => Coins.ForEach(coin => coin.ActivateAnimation());
    
    private void OnPlayAgain(PlayAgainEvent evt) => CollectedCoins.Clear();
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
    }
    
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
        ManagerParameters args = ManagerParameters.FromCurrentSheet(new()
        {
            Position = gridPosition
        });
        
        CoinController result = SetInSheet(args);

        return PlacementResult.FromController(result);
    }

    public bool Remove(Vector2 position, AnchorController sheet)
    {
        CoinController coin = coinQueryService.Find(position, sheet);

        if (coin == null) return false;

        coin.Delete();
        return true;
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