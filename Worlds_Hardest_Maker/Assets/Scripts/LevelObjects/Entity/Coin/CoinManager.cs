using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CoinManager : MonoBehaviour, 
    IManager<CoinController>, 
    IManagerPlaceRestrictable, 
    ILevelObjectManager
{
    public static CoinManager Instance { get; private set; }
    
    [UsedImplicitly] public static readonly List<FieldMode> CannotPlaceFields = new();
    
    [SerializeField] [InitializationField] [MustBeAssigned] private CoinController coinPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform coinContainer;
    
    [ReadOnly] public List<CoinController> Coins = new();
    [ReadOnly] public List<CoinController> CollectedCoins = new();
    
    private int TotalCoins => Coins.Count;
    
    public int CoinsNeededFinal =>
        Mathf.Min(LevelSettings.Instance.IsCoinsNeededLimited ? LevelSettings.Instance.CoinsNeeded : TotalCoins, TotalCoins);
    
    private static readonly int PLAYING = Animator.StringToHash("Playing");
    
    private IObjectResolver diContainer;
    private EventBus eventBus;
    private IPositionQueryService positionQueryService;
    
    [Inject]
    private void Construct(IObjectResolver diContainer, EventBus eventBus, IPositionQueryService positionQueryService)
    {
        this.diContainer = diContainer;
        this.eventBus = eventBus;
        this.positionQueryService = positionQueryService;
        
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
    }
    
    public bool CanPlace(Vector2 position) => CanPlaceInSheet(position, PlaceManager.GetCurrentSheet());
    
    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet) =>
        // conditions: no coin there, doesn't intersect with any walls etc, no player there
        !((IManager<CoinController>)this).IsThereInSheet(position, sheet)
        && !FieldManager.Instance.IntersectingAnyFieldsAtPos(position, sheet, CannotPlaceFields.ToArray())
        && !PlayerManager.Instance.IsThere(position);
    
    public CoinController SetInSheet(ManagerParameters args)
    {
        Vector2 matrixPosition = args.Position.ConvertToGrid();
        
        if (!CanPlaceInSheet(matrixPosition, args.Sheet)) return null;
        
        CoinController coin = InstantiateInSheet(args);
        
        coin.Animator.SetBool(PLAYING, LevelSessionEditManager.Instance.IsPlaying);
        
        PlaceManager.Instance.AttachToSheet(coin.gameObject, args.Sheet);
        
        return coin;
    }
    
    public CoinController GetInSheet(Vector2 position, AnchorController sheet)
    {
        return positionQueryService.QueryPosition<CoinController>(position, 
            0.1f, 
            LayerManager.Instance.Layers.Entity,
            "Coin",
            sheet);
    }
    
    public CoinController Get(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out CoinController c)) return c;
        }
        
        return null;
    }
    
    public CoinController InstantiateInSheet(ManagerParameters args)
    {
        CoinController coin = Instantiate(
            coinPrefab,
            args.Position, Quaternion.identity,
            args.Sheet == null ? coinContainer : args.Sheet.AttachmentContainer
        );
        
        diContainer.InjectGameObject(coin.gameObject);
        
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

    public bool Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        ManagerParameters args = ManagerParameters.FromCurrentSheet(new()
        {
            Position = gridPosition
        });
        
        return SetInSheet(args);
    }

    public LevelObjectController Query(Vector2 position, AnchorController sheet)
    {
        return GetInSheet(position, sheet);
    }

    public bool Remove(Vector2 position, AnchorController sheet)
    {
        CoinController coin = GetInSheet(position, sheet);

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