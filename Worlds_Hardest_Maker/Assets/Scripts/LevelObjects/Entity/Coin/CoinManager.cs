using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CoinManager : MonoBehaviour, IManager<CoinController>, IManagerPlaceRestrictable
{
    public static CoinManager Instance { get; private set; }
    
    [UsedImplicitly] public static readonly List<FieldMode> CannotPlaceFields = new();
    
    [ReadOnly] public List<CoinController> Coins = new();
    [ReadOnly] public List<CoinController> CollectedCoins = new();
    
    public Transform DefaultContainer => ReferenceManager.Instance.CoinContainer;
    
    private int TotalCoins => Coins.Count;
    
    public int CoinsNeededFinal =>
        Mathf.Min(LevelSettings.Instance.IsCoinsNeededLimited ? LevelSettings.Instance.CoinsNeeded : TotalCoins, TotalCoins);
    
    private static readonly int PLAYING = Animator.StringToHash("Playing");
    
    private IObjectResolver diContainer;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(IObjectResolver diContainer, EventBus eventBus)
    {
        this.diContainer = diContainer;
        this.eventBus = eventBus;
        
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
        
        coin.Animator.SetBool(PLAYING, LevelSessionEditManager.Instance.Playing);
        
        PlaceManager.Instance.AttachToSheet(coin.gameObject, args.Sheet);
        
        return coin;
    }
    
    public CoinController GetInSheet(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Coin")) continue;
            if (!hit.TryGetComponent(out CoinController coin)) continue;
            if (IManager.IsInSheet(coin, sheet)) return coin;
        }
        
        return null;
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
            PrefabManager.Instance.Coin,
            args.Position, Quaternion.identity,
            args.Sheet == null ? DefaultContainer : args.Sheet.AttachmentContainer
        );
        
        diContainer.InjectGameObject(coin.gameObject);
        
        return coin;
    }
    
    public List<Data> Serialize(List<Data> levelData)
    {
        foreach (CoinController coin in Coins)
        {
            if (coin.IsAttached) continue;
            
            CoinData coinData = new(coin);
            levelData.Add(coinData);
        }
        
        return levelData;
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
    
    public bool CorrespondsToEditMode(EditMode compare) => compare == EditModeManager.Coin;
}