using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public partial class PlayerController : EntityController
{
    [Separator] [SerializeField] [InitializationField] [MustBeAssigned] private BoxCollider2D centerCollider;
    [Space] [Separator("Water settings")] [SerializeField] private Transform waterLevel;
    
    [Separator("Death settings")] [SerializeField] [PositiveValueOnly] private float defaultDeathFadeDuration;
    [SerializeField] [PositiveValueOnly] private float voidFallDuration;
    
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
    
    [ReadOnly] public bool IsAttached;
    
    private Vector2 movementInput;
    private Vector2 extraMovementInput;
    
    [HideInInspector] public bool InDeathAnim;
    
    private bool onWater;
    private float currentDrownDuration;
    
    [HideInInspector] public bool Won;
    
    private Vector3 defaultScale;
    
    [HideInInspector] public bool HasTeleported;
    
    public static float Speed => LevelSettings.Instance.PlayerSpeed;
    
    [ReadOnly] [CanBeNull] public AnchorController Sheet;
    
    [ReadOnly] public List<FieldController> CurrentPlatforms;
    
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");
    
    public event Action OnDeathEnter = () => { };
    public event Action OnDeathEnd = () => { };
    public event Action OnCheckpointEnter = () => { };
    
    public override EditMode EditMode => EditModeManager.Player;
    
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
        
        PlayManager.Instance.OnSwitchToEdit += OnEdit;
        PlayManager.Instance.OnSwitchToPlay += OnPlay;
        PlayManager.Instance.OnLevelReset += ResetState;
        
        IsAttached = Sheet != null;
        if (IsAttached) SheetStartPosOffset = transform.position - Sheet.transform.position;
        
        EdgeCollider.enabled = LevelSessionEditManager.Instance.Playing;
        
        ApplyCurrentGameState();
    }
    
    private void Update() =>
        // get movement input
        movementInput = KeyBinds.GetMovementInput();
    
    private void OnCollisionStay2D(Collision2D collider) => CornerPush(collider);
    
    private void FixedUpdate()
    {
        UpdateWaterState();
        
        Move();
        
        VoidDetection();
    }
    
    private void OnDestroy()
    {
        PlayManager.Instance.OnSwitchToEdit -= OnEdit;
        PlayManager.Instance.OnSwitchToPlay -= OnPlay;
        
        PlayManager.Instance.OnLevelReset -= ResetState;
    }
    
    private void OnEdit()
    {
        EdgeCollider.enabled = false;
        
        DefaultDeathAnim();
        
        Shotgun.gameObject.SetActive(false);
        
        sortingGroup.sortingLayerName = LayerManager.Instance.SortingLayers.Player;
        
        ResetState();
    }
    
    private void OnPlay()
    {
        EdgeCollider.enabled = true;
        
        HasTeleported = false;
        
        if (KonamiManager.Instance.KonamiActive) Shotgun.gameObject.SetActive(true);
        
        sortingGroup.sortingLayerName = LayerManager.Instance.SortingLayers.PlayerPlayMode;
        
        Setup();
    }
    
    public void ReSet(ManagerParameters args)
    {
        // calls when the player is placed, when there was already one existing, hence re-setting it
        transform.position = args.Position;
        StartPos = args.Position;
        
        bool willBeAttached = args.Sheet != null;
        
        if (willBeAttached)
        {
            PlaceManager.AttachToSheet(gameObject, args.Sheet, false);
        }
        else
        {
            PlaceManager.Detach(gameObject, PlayerManager.Instance.DefaultContainer);
        }
        Sheet = args.Sheet;
        
        IsAttached = Sheet != null;
        if (IsAttached)
        {
            SheetStartPosOffset = transform.position - Sheet.transform.position;
            
        }
    }
    
    public void Win()
    {
        if (InDeathAnim || Won) return;
        // animation and play mode and that's it really
        AudioManager.Instance.Play("Win");
        Won = true;
        
        PlayerManager.Instance.InvokeOnWin();
    }
    
    public void DestroySelf(bool removeTargetFromCamera = true)
    {
        if (removeTargetFromCamera && ReferenceManager.Instance.MainCameraJumper.GetTarget("Player") == gameObject)
            ReferenceManager.Instance.MainCameraJumper.RemoveTarget("Player");
        
        Destroy(gameObject);
    }
    
    public Vector2Int GetCurrentRoom()
    {
        Vector2 position = transform.position;
        return new(
            Mathf.RoundToInt(position.x / LevelSettings.Instance.RoomWidth),
            Mathf.RoundToInt(position.y / LevelSettings.Instance.RoomHeight)
        );
    }
    
    public Vector2Int GetStartRoom() =>
        new(
            Mathf.RoundToInt(StartPos.x / LevelSettings.Instance.RoomWidth),
            Mathf.RoundToInt(StartPos.y / LevelSettings.Instance.RoomHeight)
        );
    
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
            isEdit ? LevelSessionEditManager.Instance.Playing && KonamiManager.Instance.KonamiActive : KonamiManager.Instance.KonamiActive
        );
    }
    
    public void Setup()
    {
        CurrentFields.Clear();
        CurrentGameState = null;
        Deaths = 0;
    }
    
    public override Data GetData() => new PlayerData(this);
}