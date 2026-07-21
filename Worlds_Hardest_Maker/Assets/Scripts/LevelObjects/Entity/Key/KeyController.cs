using System.Collections;
using DG.Tweening;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class KeyController : EntityController, IResettable, ICollectible
{
    private EventBus eventBus;
    
    [Separator] [SerializeField] [PositiveValueOnly] private float fadeDuration = 0.5f;
    [Separator] [MyBox.ReadOnly] public KeyColor Color;
    [MyBox.ReadOnly] public Vector2 InitialPosition;
    [MyBox.ReadOnly] public bool Collected;
    
    [Separator] [InitializationField] [Required] public SpriteRenderer SpriteRenderer;
    
    [InitializationField] [Required] public Animator Animator;
    [InitializationField] [Required] public IntervalRandomAnimation KonamiAnimation;
    
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
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
    }
    
    private void Awake()
    {
        InitialPosition = transform.position;
        
        // cache key controller
        KeyManager.Instance.Keys.Add(this);
        
        SetOrderInLayer();
    }
    
    protected override void Start()
    {
        base.Start();
        
        ((IResettable)this).Subscribe();
        PlayManager.Instance.OnSwitchToPlay += ActivateAnimation;
    }
    
    private void OnDestroy()
    {
        KeyManager.Instance.Keys.Remove(this);
        
        ((IResettable)this).Unsubscribe();
        PlayManager.Instance.OnSwitchToPlay -= ActivateAnimation;
        
        eventBus.Unsubscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        
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
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            int order = key.SpriteRenderer.sortingOrder;
            if (order > highestOrder) highestOrder = order;
        }
        
        SpriteRenderer.sortingOrder = highestOrder + 1;
    }
    
    public void Collect()
    {
        KeyManager.Instance.CollectedKeys.Add(this);
        
        // pickup animation and sound
        Animator.SetBool(pickedUpString, true);
        audioService.Play("PlaceKey");
        
        Collected = true;
        
        UnlockKeyDoors();
    }
    
    public void UnlockKeyDoors()
    {
        if (!KeyManager.Instance.AllKeysCollected(Color)) return;
        
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
        PlayerController player = PlayerManager.Instance.Player;
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