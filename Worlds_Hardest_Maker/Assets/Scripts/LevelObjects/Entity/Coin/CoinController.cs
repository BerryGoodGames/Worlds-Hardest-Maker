using System.Linq;
using DG.Tweening;
using MyBox;
using UnityEngine;
using VContainer;

public class CoinController : EntityController, IResettable, ICollectible
{
    [Separator] [SerializeField] [MinValue(0)] private float fadeDuration = 0.5f;
    [Separator] [InitializationField] [MustBeAssigned] public Animator Animator;
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer spriteRenderer;
    
    [HideInInspector] public Vector2 InitialPosition;
    
    [HideInInspector] public bool PickedUp;
    
    private static readonly int playingString = Animator.StringToHash("Playing");
    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");
    
    public override EditMode EditMode => EditModeManager.Coin;
    
    private EventBus eventBus;
    [Inject] private ILevelObjectRegistry<CoinController> coinRegistry;
    [Inject] private IPlayerProvider playerProvider;
    [Inject] private ICoinManager coinManager;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => ActivateAnimation();

    private void Awake()
    {
        InitialPosition = transform.position;
    }
    
    protected override void Start()
    {
        coinRegistry.Register(this);
        
        base.Start();
        
        ((IResettable)this).Subscribe(eventBus);
        
        Animator.SetBool(playingString, LevelSessionEditManager.Instance.IsPlaying);
    }
    
    private void OnDestroy()
    {
        // un-cache coin
        coinRegistry.Unregister(this);
        
        ((IResettable)this).Unsubscribe(eventBus);
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        
        DOTween.Kill(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PickedUp) return;
        
        // check if edgeCollider is player
        if (!collision.TryGetComponent(out PlayerController controller)) return;
        
        // check if that player hasn't picked coin up yet
        if (coinManager.CollectedCoins.Any(c => c == this)) return;
        
        Collect();
        
        // check if player is in goal while collecting coin
        if (!coinManager.AllCoinsCollected()) return;
        
        foreach (FieldController field in controller.CurrentFields)
        {
            FieldMode fieldMode = field.FieldMode;
            if (fieldMode != EditModeManager.Goal) continue;
            
            controller.Win();
            break;
        }
    }
    
    public void Collect()
    {
        coinManager.CollectCoin(this);
        
        // coin counter, sfx, animation
        audioService.Play("PlaceCoin");
        
        Animator.SetBool(pickedUpString, true);
        PickedUp = true;
    }
    
    public bool ShouldRespawn()
    {
        PlayerController player = playerProvider.Player;
        if (player == null)
        {
            Debug.LogWarning("Could not find player");
            return false;
        }
        
        // check if coin should respawn
        bool respawns = true;
        if (player.CurrentGameState == null) return true;
        
        foreach (Vector2 collected in player.CurrentGameState.CollectedCoins)
        {
            if (!collected.x.EqualsFloat(InitialPosition.x) ||
                !collected.y.EqualsFloat(InitialPosition.y)) continue;
            
            // if coin is collected or no state exists it doesn't respawn
            respawns = false;
            break;
        }
        
        return respawns;
    }
    
    public void FadeIn() => spriteRenderer.DOFade(1, fadeDuration).SetId(gameObject);
    public void FadeOut() => spriteRenderer.DOFade(0, fadeDuration).SetId(gameObject);
    
    public void ResetState()
    {
        PickedUp = false;
        
        Animator.SetBool(playingString, false);
        Animator.SetBool(pickedUpString, false);
    }
    
    public void ActivateAnimation()
    {
        Animator.SetBool(playingString, true);
        Animator.SetBool(pickedUpString, PickedUp);
    }
    
    public override Data GetData() => new CoinData(this);
    
    public override void OnAnchorMove(Vector2 oldPos, Vector2 newPos) => InitialPosition = transform.position;
}