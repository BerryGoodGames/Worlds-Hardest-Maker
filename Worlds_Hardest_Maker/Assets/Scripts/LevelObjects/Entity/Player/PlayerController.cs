using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

public partial class PlayerController : EntityController
{
    [Separator] [SerializeField] [InitializationField] [MustBeAssigned] private BoxCollider2D centerCollider;
    [Space] [Separator("Water settings")] [SerializeField] private Transform waterLevel;
    
    [Separator("Death settings")] [SerializeField] [PositiveValueOnly] private float defaultDeathFadeDuration;
    [SerializeField] [PositiveValueOnly] private float voidFallDuration;
    
    [Separator] [SerializeField] [MustBeAssigned] private ParticleSystem confetti1;
    [SerializeField] [MustBeAssigned] private ParticleSystem confetti2;
    
    [HideInInspector] public Rigidbody2D Rb;
    
    [HideInInspector] public EdgeCollider2D EdgeCollider;
    
    private SpriteRenderer spriteRenderer;
    private SortingGroup sortingGroup;
    
    public ShotgunController Shotgun { get; private set; }
    
    [ReadOnly] public int Deaths;
    
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
    
    private JumpToEntity mainCameraJumper;

    private TimerController timerController;

    private Transform playerContainer;
    
    private EventBus eventBus;
    [Inject] private IKonamiService konamiService;
    [Inject] private CoinQueryService coinQueryService;
    [Inject] private KeyQueryService keyQueryService;
    
    public static float Speed => LevelSettings.Instance.PlayerSpeed;
    
    [ReadOnly] public List<FieldController> CurrentPlatforms;
    
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");
    
    public event Action OnDeathEnter = () => { };
    public event Action OnDeathEnd = () => { };
    public event Action OnCheckpointEnter = () => { };
    
    public override EditMode EditMode => EditModeManager.Player;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<ResetLevelEvent>(OnResetLevel);
        
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
    }
    
    public void Initialize(JumpToEntity mainCameraJumper, TimerController timerController, Transform playerContainer)
    {
        this.mainCameraJumper = mainCameraJumper;
        this.timerController = timerController;
        this.playerContainer = playerContainer;
    }
    
    protected override void Start()
    {
        InitComponents();
        
        Transform t = transform;
        
        StartPos = t.position;
        defaultScale = t.localScale;
        
        base.Start();
        
        IsAttached = Sheet != null;
        if (IsAttached) SheetStartPosOffset = transform.position - Sheet.transform.position;
        
        EdgeCollider.enabled = LevelSessionEditManager.Instance.IsPlaying;
        
        ApplyCurrentGameState();
        
        if (!LevelSessionManager.Instance.IsEdit) OnSwitchToPlay(new());
    }
    
    private void Update()
    {
        // get movement input
        movementInput = KeyBinds.GetMovementInput();
        
        if (KeyBinds.GetKeyBindDown("Level_RestartFromStart")) PlayManager.Instance.RestartLevel();
        else if (KeyBinds.GetKeyBindDown("Level_RestartFromLastCheckpoint")) DieNormal();
        
        VoidDetection();
    }
    
    private void LateUpdate() => transform.rotation = Quaternion.Euler(0, 0, 0);
    
    private void OnCollisionStay2D(Collision2D collider) => CornerPush(collider);
    
    private void FixedUpdate()
    {
        UpdateWaterState();
        
        Move();
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
    
    private void OnPlayAgain(PlayAgainEvent evt)
    {
        CurrentGameState = null;
        DefaultDeathAnim(0);
        Deaths = 0;
    }
    
    private void OnKonamiStateChanged(KonamiStateChangedEvent evt)
    {
        Shotgun.gameObject.SetActive((!LevelSessionManager.Instance.IsEdit || LevelSessionEditManager.Instance.IsPlaying) && evt.Active);
    }
    
    public void ReSet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        // calls when the player is placed, when there was already one existing, hence re-setting it
        transform.position = position;
        StartPos = position;
        
        bool willBeAttached = sheet != null;
        
        if (willBeAttached) PlaceManager.Instance.AttachToSheet(gameObject, sheet, false);
        else PlaceManager.Detach(gameObject, playerContainer);
        
        Sheet = sheet;
        
        IsAttached = Sheet != null;
        if (IsAttached) SheetStartPosOffset = transform.position - Sheet!.transform.position;
    }
    
    public void Win()
    {
        if (InDeathAnim || Won) return;
        
        PlayWinSfx();
        
        Won = true;
        
        eventBus.Fire(new WinLevelEvent());
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
        if (removeTargetFromCamera && mainCameraJumper.GetTarget("Player") == gameObject)
        {
            mainCameraJumper.RemoveTarget("Player");
        }
        
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
            isEdit ? LevelSessionEditManager.Instance.IsPlaying && konamiService.IsKonamiActive : konamiService.IsKonamiActive
        );
    }
    
    public void Setup()
    {
        CurrentFields.Clear();
        CurrentGameState = null;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<ResetLevelEvent>(OnResetLevel);
        
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
    }
    
    public override Data GetData() => new PlayerData(this);
    
    public override void OnAnchorMove(Vector2 oldPos, Vector2 newPos)
    {
        StartPos = transform.position;
        
        if (IsAttached) SheetStartPosOffset = transform.position - Sheet.transform.position;
    }
}