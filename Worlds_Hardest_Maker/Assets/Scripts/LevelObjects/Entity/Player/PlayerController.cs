using System;
using System.Collections.Generic;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public partial class PlayerController : EntityController
{
    [Separator] [SerializeField] [InitializationField] [Required] private BoxCollider2D centerCollider;
    [Space] [Separator("Water settings")] [SerializeField] private Transform waterLevel;
    
    [Separator("Death settings")] [SerializeField] [PositiveValueOnly] private float defaultDeathFadeDuration;
    [SerializeField] [PositiveValueOnly] private float voidFallDuration;
    
    [Separator] [SerializeField] [Required] private ParticleSystem confetti1;
    [SerializeField] [Required] private ParticleSystem confetti2;
    
    [HideInInspector] public Rigidbody2D Rb;
    
    [HideInInspector] public EdgeCollider2D EdgeCollider;
    
    private SpriteRenderer spriteRenderer;
    private SortingGroup sortingGroup;
    
    public ShotgunController Shotgun { get; private set; }
    
    [MyBox.ReadOnly] public int Deaths;
    
    [HideInInspector] public List<FieldController> CurrentFields;
    
    public GameState CurrentGameState;
    
    [HideInInspector] public Vector2 StartPos;
    [HideInInspector] public Vector2 SheetStartPosOffset;
    private CheckpointController currentRunCheckpoint;
    
    private Vector2 movementInput;
    private Vector2 extraMovementInput;
    
    [HideInInspector] public bool InDeathAnim;
    
    private bool onWater;
    private float currentDrownDuration;
    
    [HideInInspector] public bool Won;
    
    private Vector3 defaultScale;
    
    [HideInInspector] public bool HasTeleported;
    
    private EventBus eventBus;
    
    private IKonamiService konamiService;
    
    public static float Speed => LevelSettings.Instance.PlayerSpeed;
    
    [MyBox.ReadOnly] public List<FieldController> CurrentPlatforms;
    
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");
    
    public event Action OnDeathEnter = () => { };
    public event Action OnDeathEnd = () => { };
    public event Action OnCheckpointEnter = () => { };
    
    public override EditMode EditMode => EditModeManager.Player;
    
    [Inject]
    private void Construct(EventBus eventBus, IKonamiService konamiService)
    {
        this.eventBus = eventBus;
        this.konamiService = konamiService;
        
        eventBus.Subscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<ResetLevelEvent>(OnResetLevel);
    }
    
    private void Awake()
    {
        InitComponents();
        
        Transform t = transform;
        
        StartPos = t.position;
        defaultScale = t.localScale;
    }
    
    protected override void Start()
    {
        base.Start();
        
        LevelCompleteManager.Instance.OnPlayAgain += OnPlayAgain;
        
        IsAttached = Sheet != null;
        if (IsAttached) SheetStartPosOffset = transform.position - Sheet.transform.position;
        
        EdgeCollider.enabled = LevelSessionEditManager.Instance.Playing;
        
        ApplyCurrentGameState();
        
        if (!LevelSessionManager.Instance.IsEdit) OnSwitchToPlay(new SwitchToPlayEvent());
    }
    
    private void Update()
    {
        // get movement input
        movementInput = KeyBinds.GetMovementInput();
        
        VoidDetection();
    }
    
    private void LateUpdate() => transform.rotation = Quaternion.Euler(0, 0, 0);
    
    private void OnCollisionStay2D(Collision2D collider) => CornerPush(collider);
    
    private void FixedUpdate()
    {
        UpdateWaterState();
        
        Move();
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<ResetLevelEvent>(OnResetLevel);
        
        LevelCompleteManager.Instance.OnPlayAgain -= OnPlayAgain;
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        EdgeCollider.enabled = false;
        
        DefaultDeathAnim();
        
        Shotgun.gameObject.SetActive(false);
        
        sortingGroup.sortingLayerName = LayerManager.Instance.SortingLayers.Player;
        
        OnResetLevel(new ResetLevelEvent());
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        EdgeCollider.enabled = true;
        
        HasTeleported = false;
        
        if (konamiService.IsKonamiActive) Shotgun.gameObject.SetActive(true);
        
        sortingGroup.sortingLayerName = LayerManager.Instance.SortingLayers.PlayerPlayMode;
        
        Setup();
    }
    
    private void OnPlayAgain()
    {
        CurrentGameState = null;
        DefaultDeathAnim(0);
        Deaths = 0;
    }
    
    private void OnKonamiStateChanged(KonamiStateChangedEvent evt)
    {
        Shotgun.gameObject.SetActive((!LevelSessionManager.Instance.IsEdit || LevelSessionEditManager.Instance.Playing) && evt.Active);
    }
    
    public void ReSet(ManagerParameters args)
    {
        // calls when the player is placed, when there was already one existing, hence re-setting it
        transform.position = args.Position;
        StartPos = args.Position;
        
        bool willBeAttached = args.Sheet != null;
        
        if (willBeAttached) PlaceManager.Instance.AttachToSheet(gameObject, args.Sheet, false);
        else PlaceManager.Detach(gameObject, PlayerManager.Instance.DefaultContainer);
        
        Sheet = args.Sheet;
        
        IsAttached = Sheet != null;
        if (IsAttached) SheetStartPosOffset = transform.position - Sheet.transform.position;
    }
    
    public void Win()
    {
        if (InDeathAnim || Won) return;
        
        PlayWinSfx();
        
        Won = true;
        
        PlayerManager.Instance.InvokeOnWin();
    }
    
    private void PlayWinSfx()
    {
        audioService.Play("Win");
        
        const int PARTY_HORN_COUNT = 9;
        string[] partyHorns = new string[PARTY_HORN_COUNT];
        for (int i = 0; i < PARTY_HORN_COUNT; i++) partyHorns[i] = $"PartyHorn{i + 1}";
        
        string selectedPartyHorn = partyHorns.GetRandom();
        audioService.Play(selectedPartyHorn);
        
        audioService.Play("PartyPopper");
        
        confetti1.Play();
        confetti2.Play();
    }
    
    public void DestroySelf(bool removeTargetFromCamera = true)
    {
        if (removeTargetFromCamera && ReferenceManager.Instance.MainCameraJumper.GetTarget("Player") == gameObject)
            ReferenceManager.Instance.MainCameraJumper.RemoveTarget("Player");
        
        Destroy(gameObject);
    }
    
    public Vector2Int GetCurrentRoom() => transform.position.GetRoom();
    
    public Vector2Int GetStartRoom() => StartPos.GetRoom();
    
    private void InitComponents()
    {
        bool isEdit = LevelSessionManager.Instance.IsEdit;
        
        CoinManager.Instance.CollectedCoins = new();
        
        Rb = GetComponent<Rigidbody2D>();
        EdgeCollider = GetComponent<EdgeCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sortingGroup = GetComponent<SortingGroup>();
        Shotgun = GetComponentInChildren<ShotgunController>(true);
        Shotgun.gameObject.SetActive(
            isEdit ? LevelSessionEditManager.Instance.Playing && konamiService.IsKonamiActive : konamiService.IsKonamiActive
        );
    }
    
    public void Setup()
    {
        CurrentFields.Clear();
        CurrentGameState = null;
    }
    
    public override Data GetData() => new PlayerData(this);
    
    public override void OnAnchorMove(Vector2 oldPos, Vector2 newPos)
    {
        StartPos = transform.position;
        
        if (IsAttached) SheetStartPosOffset = transform.position - Sheet.transform.position;
    }
}