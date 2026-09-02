using System.Collections;
using DG.Tweening;
using MyBox;
using UnityEngine;
using VContainer;

public class KeyController : EntityController, IResettable, ICollectible
{
    [Separator] [SerializeField] [PositiveValueOnly] private float fadeDuration = 0.5f;
    [Separator] [ReadOnly] public KeyColor Color;
    [ReadOnly] public Vector2 InitialPosition;
    [ReadOnly] public bool Collected;
    
    [Separator] [InitializationField] [MustBeAssigned] public SpriteRenderer SpriteRenderer;
    
    [InitializationField] [MustBeAssigned] public Animator Animator;
    [InitializationField] [MustBeAssigned] public IntervalRandomAnimation KonamiAnimation;
    
    private static readonly int playingString = Animator.StringToHash("Playing");
    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");
    
    public override EditMode EditMode =>
        Color switch
        {
            KeyColor.Gray => EditModeManager.GrayKey,
            KeyColor.Red => EditModeManager.RedKey,
            KeyColor.Green => EditModeManager.GreenKey,
            KeyColor.Blue => EditModeManager.BlueKey,
            KeyColor.Yellow => EditModeManager.YellowKey,
            _ => throw new("There is no edit mode assigned for color " + Color),
        };
    
    private EventBus eventBus;
    [Inject] private IKonamiService konamiService;
    [Inject] private ILevelObjectRegistry<KeyController> keyRegistry;
    [Inject] private IPlayerProvider playerProvider;
    [Inject] private IKeyManager keyManager;

    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => ActivateAnimation();
    
    private void Awake()
    {
        InitialPosition = transform.position;
    }
    
    protected override void Start()
    {
        keyRegistry.Register(this);
        
        SetOrderInLayer();
        
        base.Start();
        
        ((IResettable)this).Subscribe(eventBus);
        
        Animator.SetBool(playingString, LevelSessionEditManager.Instance.IsPlaying);
        KonamiAnimation.enabled = konamiService.IsKonamiActive;
    }
    
    private void OnDestroy()
    {
        keyRegistry.Unregister(this);
        
        ((IResettable)this).Unsubscribe(eventBus);
        
        eventBus.Unsubscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        
        DOTween.Kill(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check key collection: collider is player, key is not yet collected
        if (collision.CompareTag("Player") && !Collected) Collect();
    }
    
    /// <summary>
    ///     Set order in layer to be on top of every other
    /// </summary>
    private void SetOrderInLayer()
    {
        int highestOrder = 0;
        foreach (KeyController key in keyRegistry.All)
        {
            int order = key.SpriteRenderer.sortingOrder;
            if (order > highestOrder) highestOrder = order;
        }
        
        SpriteRenderer.sortingOrder = highestOrder + 1;
    }
    
    public void Collect()
    {
        keyManager.CollectKey(this);
        
        // pickup animation and sound
        Animator.SetBool(pickedUpString, true);
        audioService.Play("PlaceKey");
        
        Collected = true;
        
        UnlockKeyDoors();
    }
    
    private void UnlockKeyDoors()
    {
        if (!keyManager.AllKeysCollected(Color)) return;
        
        string tagColor = Color switch
        {
            KeyColor.Red => "Red",
            KeyColor.Green => "Green",
            KeyColor.Blue => "Blue",
            KeyColor.Yellow => "Yellow",
            _ => "Gray",
        };
        
        foreach (GameObject door in GameObject.FindGameObjectsWithTag(tagColor + "KeyDoor"))
        {
            KeyDoorFieldController controller = door.GetComponent<KeyDoorFieldController>();
            controller.SetLocked(false);
        }
        
        audioService.Play("KeyDoorUnlock");
    }
    
    public bool ShouldRespawn()
    {
        PlayerController player = playerProvider.Player;
        if (player == null)
        {
            Debug.LogWarning("Could not find player");
            return false;
        }
        
        if (player.CurrentGameState == null) return true;
        
        bool isRespawning = true;
        foreach (Vector2 collected in player.CurrentGameState.CollectedKeys)
        {
            if (!collected.x.EqualsFloat(InitialPosition.x) ||
                !collected.y.EqualsFloat(InitialPosition.y)) continue;
            
            // if key is collected or no state exists it doesn't respawn
            isRespawning = false;
            break;
        }
        
        return isRespawning;
    }
    
    private void OnKonamiStateChanged(KonamiStateChangedEvent evt)
    {
        KonamiAnimation.enabled = evt.Active;
        KonamiAnimation.Randomize();
    }
    
    public void ResetState()
    {
        Collected = false;
        
        Animator.SetBool(playingString, false);
        Animator.SetBool(pickedUpString, false);
    }
    
    public void ActivateAnimation()
    {
        StartCoroutine(Delay());
        
        return;
        
        IEnumerator Delay()
        {
            yield return new WaitForEndOfFrame();
            
            Animator.enabled = true;
            
            Animator.SetBool(playingString, true);
            Animator.SetBool(pickedUpString, Collected);
        }
    }
    
    public void FadeIn() => SpriteRenderer.DOFade(1, fadeDuration).SetId(gameObject);
    public void FadeOut() => SpriteRenderer.DOFade(0, fadeDuration).SetId(gameObject);
    
    public override Data GetData() => new KeyData(this);
    
    public override void OnAnchorMove(Vector2 oldPos, Vector2 newPos) => InitialPosition = transform.position;
}