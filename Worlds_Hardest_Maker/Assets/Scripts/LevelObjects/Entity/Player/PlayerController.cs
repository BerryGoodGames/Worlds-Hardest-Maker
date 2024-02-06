using System;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;

public class PlayerController : EntityController
{
    #region Editor variables

    [Space]
    [Separator("Water settings")] [SerializeField] private Transform waterLevel;
    
    [Separator("Death settings")] [SerializeField] [PositiveValueOnly] private float defaultDeathFadeDuration;
    [SerializeField] [PositiveValueOnly] private float voidFallDuration;

    #endregion

    #region Components

    [HideInInspector] public Rigidbody2D Rb;

    [HideInInspector] public EdgeCollider2D EdgeCollider;

    private SpriteRenderer spriteRenderer;

    public ShotgunController Shotgun { get; private set; }

    #endregion

    #region Fields

    [ReadOnly] public int Deaths;

    [HideInInspector] public List<FieldController> CurrentFields;

    public GameState CurrentGameState;

    [HideInInspector] public Vector2 StartPos;

    private Vector2 movementInput;
    private Vector2 extraMovementInput;

    [HideInInspector] public bool InDeathAnim;

    private bool onWater;
    private float currentDrownDuration;

    [HideInInspector] public bool Won;

    private Vector3 defaultScale;

    [HideInInspector] public bool HasTeleported;

    public float Speed => LevelSettings.Instance.PlayerSpeed;

    #endregion

    private static readonly int pickedUp = Animator.StringToHash("PickedUp");

    public event Action OnDeathEnter;
    public event Action OnDeathEnd;
    public event Action OnCheckpointEnter;

    public override EditMode EditMode => EditModeManager.Player;

    private void Awake()
    {
        InitComponents();

        Transform t = transform;

        StartPos = t.position;
        defaultScale = t.localScale;
    }

    private void Start()
    {
        PlayManager.Instance.OnSwitchToEdit += OnEdit;
        PlayManager.Instance.OnSwitchToPlay += OnPlay;

        EdgeCollider.enabled = LevelSessionEditManager.Instance.Playing;

        if (transform.parent != ReferenceManager.Instance.PlayerContainer) transform.SetParent(ReferenceManager.Instance.PlayerContainer);

        ApplyCurrentGameState();

        PlayManager.Instance.OnLevelReset += ResetState;
    }

    private void Update() =>
        // get movement input
        movementInput = KeyBinds.GetMovementInput();

    private void OnCollisionStay2D(Collision2D collider) => CornerPush(collider);

    private void FixedUpdate()
    {
        UpdateWaterState();

        Move();
    }

    private void OnDestroy()
    {
        PlayManager.Instance.OnSwitchToEdit -= OnEdit;
        PlayManager.Instance.OnSwitchToPlay -= OnPlay;
    }

    private void OnEdit()
    {
        EdgeCollider.enabled = false;

        DefaultDeathAnim();

        Shotgun.gameObject.SetActive(false);
    }

    private void OnPlay()
    {
        EdgeCollider.enabled = true;

        HasTeleported = false;

        if (KonamiManager.Instance.KonamiActive) Shotgun.gameObject.SetActive(true);

        Setup();
    }

    #region Physics, Movement

    private void Move()
    {
        if (Won) return;
        bool ice = IsOnIce();

        Vector2 totalMovement = Vector2.zero;
        // movement (if player is yours in multiplayer mode)
        if (LevelSessionEditManager.Instance.Playing)
        {
            if (ice) IcePhysics();
            else UpdateMovement(ref totalMovement);
        }

        UpdateConveyorMovement(ref totalMovement);

        if (totalMovement != Vector2.zero) Rb.MovePosition(Rb.position + totalMovement);
    }

    private void UpdateWaterState()
    {
        // check water and update drown level
        bool onWaterNow = IsOnWater();
        if (!onWater && onWaterNow)
            // frame player enters water
            AudioManager.Instance.Play("WaterEnter");

        onWater = onWaterNow;

        if (onWater && !InDeathAnim && !Won)
        {
            currentDrownDuration += Time.fixedDeltaTime;

            if (currentDrownDuration >= LevelSettings.Instance.DrownDuration) DieNormal("DeathDrown");
        }
        else if (!InDeathAnim && !onWater) currentDrownDuration = 0;

        if (LevelSettings.Instance.DrownDuration == 0) return;

        float drown = currentDrownDuration / LevelSettings.Instance.DrownDuration;
        waterLevel.localScale = new(waterLevel.localScale.x, drown);
    }

    private void IcePhysics()
    {
        // transfer velocity to ice when entering
        if (Rb.velocity == Vector2.zero) Rb.velocity = GetPhysicsSpeed() * movementInput;

        Rb.drag = LevelSettings.Instance.IceFriction;

        // acceleration on ice
        // convert to units / second
        float force = LevelSettings.Instance.IceMaxSpeed;
        Rb.AddForce(force * LevelSettings.Instance.IceFriction * movementInput, ForceMode2D.Force);
    }

    private void UpdateMovement(ref Vector2 totalMovement)
    {
        Rb.velocity = Vector2.zero;

        // snappy movement (when not on ice)
        if (!movementInput.Equals(Vector2.zero))
        {
            totalMovement += GetPhysicsSpeed() * Time.fixedDeltaTime * new Vector2(
                Mathf.Clamp(movementInput.x + extraMovementInput.x, -1, 1),
                Mathf.Clamp(movementInput.y + extraMovementInput.y, -1, 1)
            );
        }

        extraMovementInput = Vector2.zero;
    }

    private void UpdateConveyorMovement(ref Vector2 totalMovement)
    {
        ConveyorController conveyor = GetCurrentConveyor();
        if (conveyor == null) return;

        Quaternion forceRotation = Quaternion.Euler(0, 0, conveyor.Rotation);

        Vector2 conveyorVector = conveyor.Speed * Time.fixedDeltaTime * (forceRotation * Vector2.up);

        totalMovement += conveyorVector;
    }

    private float GetPhysicsSpeed() => onWater ? LevelSettings.Instance.WaterDampingFactor * Speed : Speed;

    private void CornerPush(Collision2D collider)
    {
        Vector2 roundedPos = new(Mathf.Round(Rb.position.x), Mathf.Round(Rb.position.y));

        const float err = 0.00001f;

        // do wall corner pushy thingy
        if (!collider.transform.tag.IsSolidFieldTag() ||
            (!collider.transform.position.x.EqualsFloat(roundedPos.x + movementInput.x) &&
             !collider.transform.position.y.EqualsFloat(roundedPos.y + movementInput.y))) return;

        CornerPushHorizontal(collider, roundedPos, err);
        CornerPushVertical(collider, roundedPos, err);
    }

    private void CornerPushVertical(Collision2D collider, Vector2 roundedPos, float err)
    {
        // early-out if it should not push 
        if (movementInput.y == 0 || roundedPos.x.EqualsFloat(Mathf.Round(collider.transform.position.x)) ||
            !(Mathf.Abs(Rb.position.x) % 1 > (1 - transform.lossyScale.x) * 0.5f + err) ||
            !(Mathf.Abs(Rb.position.x) % 1 < 1 - ((1 - transform.lossyScale.x) * 0.5f + err))) return;

        // check if player counteracts
        if (movementInput.x != 0) return;

        // calculate new position 
        extraMovementInput = new(Mathf.Round(Rb.position.x) > Rb.position.x ? 1 : -1, movementInput.y);
    }

    private void CornerPushHorizontal(Collision2D collider, Vector2 roundedPos, float err)
    {
        // early-out if it should not push 
        if (movementInput.x == 0 || roundedPos.y.EqualsFloat(Mathf.Round(collider.transform.position.y)) ||
            !(Mathf.Abs(Rb.position.y) % 1 > (1 - transform.lossyScale.y) * 0.5f + err) ||
            !(Mathf.Abs(Rb.position.y) % 1 < 1 - ((1 - transform.lossyScale.y) * 0.5f + err))) return;


        // check if player counteracts
        if (movementInput.y != 0) return;

        // calculate new position
        extraMovementInput = new(movementInput.x, Mathf.Round(Rb.position.y) > Rb.position.y ? 1 : -1);
    }

    #endregion

    #region Field detection

    public bool IsOnSafeField()
    {
        foreach (FieldController field in CurrentFields)
        {
            // check if current field is safe
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType.IsSafeForPlayer) return true;
        }

        return false;
    }

    public bool IsOnField(FieldMode mode)
    {
        foreach (FieldController field in CurrentFields)
        {
            // check if current field is type
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType == mode) return true;
        }

        return false;
    }

    public List<FieldController> GetFullyOnFields()
    {
        // finds every field the player is at least half way on
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.011f);
        List<FieldController> res = new();
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out FieldController f)) res.Add(f);
        }

        return res;
    }

    public bool IsFullyOnField(FieldMode mode)
    {
        List<FieldController> fullyOnFields = GetFullyOnFields();
        foreach (FieldController field in fullyOnFields)
        {
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType == mode) return true;
        }

        return false;
    }

    public bool IsOnWater() => IsFullyOnField(EditModeManager.Water);

    public bool IsOnIce() => IsFullyOnField(EditModeManager.Ice);

    public ConveyorController GetCurrentConveyor()
    {
        if (!IsFullyOnField(EditModeManager.Conveyor)) return null;

        List<FieldController> fullyOnFields = GetFullyOnFields();
        foreach (FieldController field in fullyOnFields)
        {
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType == EditModeManager.Conveyor) return field.GetComponent<ConveyorController>();
        }

        return null;
    }

    public FieldController GetCurrentVoid()
    {
        // returns the void the player falls into (null if none)
        List<FieldController> fullyOnFields = GetFullyOnFields();
        foreach (FieldController field in fullyOnFields)
        {
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType == EditModeManager.Void) return field;
        }

        return null;
    }

    public bool IsOnVoid() =>
        // we don't need that, its just there lol
        IsFullyOnField(EditModeManager.Void);

    public FieldController GetCurrentField() => FieldManager.GetField(Vector2Int.RoundToInt(transform.position));

    #endregion

    public void Win()
    {
        if (InDeathAnim || Won) return;
        // animation and play mode and that's it really
        AudioManager.Instance.Play("Win");
        Won = true;

        PlayerManager.Instance.InvokeOnWin();
    }

    #region Dying

    public void DieNormal(string soundEffect = "Death")
    {
        if (Won) return;

        // default dying
        // avoid dying while in animation
        if (!InDeathAnim) DefaultDeathAnim();

        if (LevelSessionEditManager.Instance.Playing)
        {
            // sfx and death counter
            AudioManager.Instance.Play(soundEffect);
        }

        Death();
    }

    public void DieVoid()
    {
        if (Won) return;

        // dying through void
        FieldController currentVoid = GetCurrentVoid();

        Vector2 fallPosition = currentVoid.transform.position;

        spriteRenderer.DOFade(0, voidFallDuration)
            .SetEase(Ease.Linear);

        transform.DOMove(fallPosition, voidFallDuration)
            .SetEase(Ease.OutQuint);

        transform.DOScale(Vector2.zero, voidFallDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(DeathAnimFinish);

        AudioManager.Instance.Play("DeathFall");

        Death();
    }

    private void DefaultDeathAnim() =>
        spriteRenderer.DOFade(0, defaultDeathFadeDuration)
            .SetEase(Ease.Linear)
            .OnComplete(DeathAnimFinish);


    /// <summary>
    ///     general method when dying in any way
    /// </summary>
    private void Death()
    {
        Rb.velocity = Vector2.zero;
        Rb.simulated = false;
        InDeathAnim = true;
        
        if (LevelSessionEditManager.Instance.Playing)
        {
            Deaths++;
            if (!LevelSessionManager.Instance.IsEdit) LevelSessionManager.Instance.Deaths++;
        }

        UpdateCoinCounterDeath();
        
        OnDeathEnter?.Invoke();

        if (KonamiManager.Instance.KonamiActive) return;

        // set timer color to "not cheated", unless when hit a checkpoint
        if (!HasTeleported || CurrentGameState == null) PlayManager.Instance.Cheated = false;

        // reset balls to start position (if player launched them e.g. with shotgun)
        foreach (AnchorBallController ball in AnchorBallManager.Instance.AnchorBallList) ball.ResetPosition();
    }

    private void RevertDeathAnimation()
    {
        Transform t = transform;

        // cancel potential animations
        spriteRenderer.DOKill();
        t.DOKill();

        // reset color + scale
        Color color = spriteRenderer.color;
        color = new(color.r, color.g, color.b, 1);
        spriteRenderer.color = color;

        t.localScale = defaultScale;
    }

    private void UpdateCoinCounterDeath()
    {
        // update coin counter
        bool hasCheckpointActivated = CurrentGameState != null;
        CoinManager.Instance.CollectedCoins.Clear();

        if (!hasCheckpointActivated) return;

        foreach (Vector2 coinPos in CurrentGameState.CollectedCoins)
        {
            CoinController coin = CoinManager.GetCoin(coinPos);
            if (coin != null) CoinManager.Instance.CollectedCoins.Add(coin);
        }
    }

    public void DeathAnimFinish()
    {
        // reset timer if no checkpoint activated
        bool hasCheckpointActivated = CurrentGameState != null;
        if (!hasCheckpointActivated) ReferenceManager.Instance.TimerController.ResetTimer();

        Won = false;
        InDeathAnim = false;
        Rb.simulated = true;

        Vector2 spawnPos = !LevelSessionEditManager.Instance.Playing || CurrentGameState == null
            ? StartPos
            : CurrentGameState.PlayerStartPos;

        OnDeathEnd?.Invoke();
        
        transform.position = spawnPos;

        RevertDeathAnimation();

        Camera main = Camera.main;
        if (main != null)
        {
            JumpToEntity jumpToPlayer = main.GetComponent<JumpToEntity>();
            if (jumpToPlayer.GetTarget("Player") == gameObject) jumpToPlayer.SetTarget("Player", gameObject);
        }

        ResetCoinsToCurrentGameState();
        ResetKeysToCurrentGameState();

        string[] tags =
            { "GrayKeyDoor", "RedKeyDoor", "GreenKeyDoor", "BlueKeyDoor", "YellowKeyDoor", };

        foreach (string tag in tags)
        {
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tag))
            {
                KeyDoorFieldController comp = door.GetComponent<KeyDoorFieldController>();
                if (!KeyManager.Instance.AllKeysCollected(comp.Color)) comp.SetLocked(true);
            }
        }
        
    }

    #endregion

    public void DestroySelf(bool removeTargetFromCamera = true)
    {
        if (removeTargetFromCamera && ReferenceManager.Instance.MainCameraJumper.GetTarget("Player") == gameObject)
            ReferenceManager.Instance.MainCameraJumper.RemoveTarget("Player");

        Destroy(gameObject);
    }

    public void ActivateCheckpoint(Vector2 position)
    {
        GameState newState = GetGameStateNow();
        newState.PlayerStartPos = position;

        CurrentGameState = newState;

        OnCheckpointEnter?.Invoke();
        
        print("Saved game state");
    }

    public GameState GetGameStateNow()
    {
        CoinManager.Instance.CollectedCoins.RemoveAll(e => e == null);
        KeyManager.Instance.CollectedKeys.RemoveAll(e => e == null);

        // mx my coords of checkpoint field
        Vector2 statePlayerStartingPos = CurrentGameState?.PlayerStartPos ?? StartPos;

        // serialize game state
        // convert collectedCoins and collectedKeys to List<Vector2>
        List<Vector2> coinPositions = new();

        foreach (CoinController c in CoinManager.Instance.CollectedCoins) coinPositions.Add(c.CoinPosition);

        List<Vector2> keyPositions = new();
        foreach (KeyController key in KeyManager.Instance.CollectedKeys) keyPositions.Add(key.KeyPosition);

        GameState res = new(statePlayerStartingPos, coinPositions, keyPositions);

        return res;
    }

    public void ResetState()
    {
        DieNormal();
        CoinManager.Instance.CollectedCoins.Clear();
        KeyManager.Instance.CollectedKeys.Clear();
        CurrentGameState = null;
    }

    private void ResetCoinsToCurrentGameState()
    {
        foreach (CoinController coin in CoinManager.Instance.Coins)
        {
            if (!ShouldCoinRespawn(coin)) continue;

            CoinManager.Instance.CollectedCoins.Remove(coin);

            coin.PickedUp = false;

            coin.Animator.SetBool(pickedUp, false);
        }
    }

    private bool ShouldCoinRespawn(CoinController coin)
    {
        // check if coin should respawn
        bool respawns = true;
        if (CurrentGameState == null) return true;

        foreach (Vector2 collected in CurrentGameState.CollectedCoins)
        {
            if (!collected.x.EqualsFloat(coin.CoinPosition.x) ||
                !collected.y.EqualsFloat(coin.CoinPosition.y)) continue;

            // if coin is collected or no state exists it doesn't respawn
            respawns = false;
            break;
        }

        return respawns;
    }

    private void ResetKeysToCurrentGameState()
    {
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            bool isRespawning = true;
            if (CurrentGameState != null)
            {
                foreach (Vector2 collected in CurrentGameState.CollectedKeys)
                {
                    if (!collected.x.EqualsFloat(key.KeyPosition.x) ||
                        !collected.y.EqualsFloat(key.KeyPosition.y)) continue;

                    // if key is collected or no state exists it doesn't respawn
                    isRespawning = false;
                    break;
                }
            }

            if (!isRespawning) continue;

            KeyManager.Instance.CollectedKeys.Remove(key);

            key.Collected = false;
            key.Animator.SetBool(pickedUp, false);
        }
    }

    private void InitComponents()
    {
        bool isEdit = LevelSessionManager.Instance.IsEdit;

        CoinManager.Instance.CollectedCoins = new();

        Rb = GetComponent<Rigidbody2D>();

        EdgeCollider = GetComponent<EdgeCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        Shotgun = GetComponentInChildren<ShotgunController>(true);
        Shotgun.gameObject.SetActive(
            isEdit ? LevelSessionEditManager.Instance.Playing && KonamiManager.Instance.KonamiActive : KonamiManager.Instance.KonamiActive
        );
    }

    private void ApplyCurrentGameState()
    {
        // set progress from current state
        if (CurrentGameState == null) return;

        foreach (Vector2 coinCollectedPos in CurrentGameState.CollectedCoins)
        {
            CoinController coin = CoinManager.GetCoin(coinCollectedPos);
            if (coin == null) throw new Exception("Passed game state has null value for coin");

            CoinManager.Instance.CollectedCoins.Add(coin);
        }

        foreach (Vector2 keyCollectedPos in CurrentGameState.CollectedKeys)
        {
            KeyController key = KeyManager.GetKey(keyCollectedPos);
            if (key == null) throw new Exception("Passed game state has null value for key");

            KeyManager.Instance.CollectedKeys.Add(key);
        }
    }

    public void Setup()
    {
        CurrentFields.Clear();
        CurrentGameState = null;
        Deaths = 0;
    }

    public override Data GetData() => new PlayerData(this);
}