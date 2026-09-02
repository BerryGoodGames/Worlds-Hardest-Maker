using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class CoinManager : MonoBehaviour, ICoinManager, ILevelObjectPlacer, ILevelObjectSerializer
{
    public static CoinManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [MustBeAssigned] private CoinController coinPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform coinContainer;
    
    private readonly List<CoinController> collectedCoins = new();
    public IReadOnlyList<CoinController> CollectedCoins => collectedCoins;
    
    private int TotalCoins => coinRegistry.All.Count;
    
    public int CoinsNeededFinal =>
        Mathf.Min(LevelSettings.Instance.IsCoinsNeededLimited ? LevelSettings.Instance.CoinsNeeded : TotalCoins, TotalCoins);

    private EventBus eventBus;
    [Inject] private CoinPlacementRules placementRules;
    private CoinFactory coinFactory;
    [Inject] private IAttachmentService attachmentService;
    [Inject] private ILevelObjectRegistry<CoinController> coinRegistry;
    
    [Inject]
    private void Construct(EventBus eventBus, CoinFactory coinFactory)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
        eventBus.Subscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Subscribe<SetupPlaySceneEvent>(OnSetupPlayScene);

        this.coinFactory = coinFactory;
        coinFactory.Initialize(coinPrefab, coinContainer);
    }
    
    private void OnPlayAgain(PlayAgainEvent evt) => collectedCoins.Clear();
    private void OnResetLevel(ResetLevelEvent evt) => ActivateAnimations();
    private void OnSetupPlayScene(SetupPlaySceneEvent evt) => ActivateAnimations();
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
        eventBus.Unsubscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Unsubscribe<SetupPlaySceneEvent>(OnSetupPlayScene);
    }
    
    public CoinController CreateNew(Vector2 position, ISheet sheet)
    {
        Vector2 gridPosition = position.ConvertToGrid();
        
        if (!placementRules.CanPlaceInSheet(gridPosition, sheet)) return null;

        CoinController coin = coinFactory.Create(gridPosition, sheet);

        if (sheet is AnchorSheet anchorSheet) attachmentService.Attach(coin, anchorSheet.Anchor);
        
        return coin;
    }
    
    public void CollectCoin(CoinController coin)
    {
        collectedCoins.Add(coin);
    }

    public void UncollectCoin(CoinController coin)
    {
        collectedCoins.Remove(coin);
    }
    
    public bool AllCoinsCollected() => collectedCoins.Count >= CoinsNeededFinal;
    
    public void RemoveCollectedCoinNulls()
    {
        collectedCoins.RemoveAll(coin => coin == null);
    }

    public void ClearCollectedCoins()
    {
        collectedCoins.Clear();
    }

    public void ActivateAnimations() => coinRegistry.All.ForEach(coin => coin.ActivateAnimation());
    
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
        foreach (CoinController coin in coinRegistry.All)
        {
            if (coin.IsAttached) continue;
            
            CoinData coinData = new(coin);
            levelData.Add(coinData);
        }
        
        return levelData;
    }
}