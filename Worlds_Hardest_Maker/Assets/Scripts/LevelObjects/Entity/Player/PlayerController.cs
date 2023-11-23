using System;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : EntityController
{
    #region Variables

    #region Editor variables

    [Space] public float Speed;

    [Separator("Water settings")] [SerializeField] private Transform waterLevel;

    [SerializeField] [Range(0, 1)] private float waterDamping;
    [SerializeField] private float drownDuration;
    private float currentDrownDuration;

    [Separator("Ice settings")] [SerializeField] private float iceFriction;

    [SerializeField] private float maxIceSpeed;

    [Separator("Void settings")] [SerializeField] private float voidFallDuration;

    #endregion

    #region Components

    [HideInInspector] public Rigidbody2D Rb;

    [HideInInspector] public Animator Animator;

    [HideInInspector] public EdgeCollider2D EdgeCollider;

    private AppendSlider sliderController;
    private TMP_Text speedText;

    private AppendNameTag nameTagController;

    [HideInInspector] public PhotonView PhotonView;

    private SpriteRenderer spriteRenderer;

    public ShotgunController Shotgun { get; private set; }

    #endregion

    #region Fields

    // [Separator] [ReadOnly] public int ID;

    [ReadOnly] public int Deaths;

    [HideInInspector] public List<CoinController> CoinsCollected;
    [HideInInspector] public List<KeyController> KeysCollected;

    [HideInInspector] public List<FieldController> CurrentFields;

    public GameState CurrentGameState;

    [HideInInspector] public Vector2 StartPos;

    private Vector2 movementInput;
    private Vector2 extraMovementInput;

    [HideInInspector] public bool InDeathAnim;

    private bool onWater;

    [HideInInspector] public bool Won;

    #endregion

    private static readonly int death = Animator.StringToHash("Death");
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");

    #endregion

    public override EditMode EditMode => EditMode.Player;

    private void Awake()
    {
        InitComponents();
        if (LevelSessionManager.Instance.IsEdit) InitSlider();

        StartPos = transform.position;
    }

    private void Start()
    {
        EditModeManager.Instance.OnEdit += OnEdit;
        EditModeManager.Instance.OnPlay += OnPlay;

        EdgeCollider.enabled = EditModeManager.Instance.Playing;

        if (transform.parent != ReferenceManager.Instance.PlayerContainer) transform.SetParent(ReferenceManager.Instance.PlayerContainer);

        ApplyCurrentGameState();

        if (LevelSessionManager.Instance.IsEdit) UpdateSpeedText();

        SyncToLevelSettings();
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
        EditModeManager.Instance.OnEdit -= OnEdit;
        EditModeManager.Instance.OnPlay -= OnPlay;
    }

    private void OnEdit()
    {
        EdgeCollider.enabled = false;

        if (Won && Animator != null) Animator.SetTrigger(death);

        Shotgun.gameObject.SetActive(false);
    }

    private void OnPlay()
    {
        EdgeCollider.enabled = true;

        if (KonamiManager.Instance.KonamiActive) Shotgun.gameObject.SetActive(true);
    }

    #region Physics, Movement

    private void Move()
    {
        if (Won) return;
        bool ice = IsOnIce();

        Vector2 totalMovement = Vector2.zero;
        // movement (if player is yours in multiplayer mode)
        if (EditModeManager.Instance.Playing)
        {
            if (MultiplayerManager.Instance.Multiplayer && !PhotonView.IsMine) return;

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

            if (currentDrownDuration >= drownDuration) DieNormal("Drown");
        }
        else if (!InDeathAnim && !onWater) currentDrownDuration = 0;

        if (drownDuration == 0) return;

        float drown = currentDrownDuration / drownDuration;
        waterLevel.localScale = new(waterLevel.localScale.x, drown);
    }

    private void IcePhysics()
    {
        // transfer velocity to ice when entering
        if (Rb.velocity == Vector2.zero) Rb.velocity = GetCurrentSpeed() * movementInput;

        Rb.drag = iceFriction;

        // acceleration on ice
        // convert to units / second
        float force = maxIceSpeed;
        Rb.AddForce(force * iceFriction * movementInput, ForceMode2D.Force);
    }

    private void UpdateMovement(ref Vector2 totalMovement)
    {
        Rb.velocity = Vector2.zero;

        // snappy movement (when not on ice)
        if (!movementInput.Equals(Vector2.zero))
        {
            totalMovement += GetCurrentSpeed() * Time.fixedDeltaTime * new Vector2(
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

        Vector2 conveyorVector = conveyor.Strength * Time.fixedDeltaTime * (forceRotation * Vector2.up);

        totalMovement += conveyorVector;
    }

    private float GetCurrentSpeed() => onWater ? waterDamping * Speed : Speed;

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
        Vector2 posCheck = new(roundedPos.x, Mathf.Round(Rb.position.y + movementInput.y));
        FieldController fieldAtPosition = FieldManager.GetField(Vector2Int.RoundToInt(posCheck));
        if (fieldAtPosition.ScriptableObject.FieldType != FieldType.Wall)
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
        Vector2 posCheck = new(Mathf.Round(Rb.position.x + movementInput.x), roundedPos.y);
        FieldController fieldAtPosition = FieldManager.GetField(Vector2Int.RoundToInt(posCheck));
        if (fieldAtPosition.ScriptableObject.FieldType != FieldType.Wall)
            extraMovementInput = new(movementInput.x, Mathf.Round(Rb.position.y) > Rb.position.y ? 1 : -1);
    }

    #endregion

    /// <summary>
    ///     Always use SetSpeed instead of setting
    /// </summary>
    [PunRPC]
    public void SetSpeed(float speed)
    {
        Speed = speed;

        if (LevelSessionManager.Instance.IsEdit)
        {
            // sync slider
            float currentSliderValue = sliderController.GetValue() / sliderController.Step;
            if (!currentSliderValue.EqualsFloat(speed)) sliderController.GetSlider().SetValueWithoutNotify(speed / sliderController.Step);
        }
    }

    [PunRPC]
    public void SetNameTagActive(bool active)
    {
        if (!PhotonView.IsMine) print($"Set name tag {active}");

        if (!MultiplayerManager.Instance.Multiplayer) throw new Exception("Trying to enable/disable name tag while in singleplayer");
        nameTagController.NameTag.SetActive(active);
    }

    /// <returns>rounded position of player</returns>
    public Vector2 GetMatrixPos() => transform.position.Floor();

    #region Field detection

    public bool IsOnSafeField()
    {
        foreach (FieldController field in CurrentFields)
        {
            // check if current field is safe
            FieldType currentFieldType = field.ScriptableObject.FieldType;
            if (PlayerManager.SafeFields.Contains(currentFieldType)) return true;
        }

        return false;
    }

    public bool IsOnField(FieldType type)
    {
        foreach (FieldController field in CurrentFields)
        {
            // check if current field is type
            FieldType currentFieldType = field.ScriptableObject.FieldType;
            if (currentFieldType == type) return true;
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
            if(hit.TryGetComponent(out FieldController f)) res.Add(f);
        }

        return res;
    }

    public bool IsFullyOnField(FieldType type)
    {
        List<FieldController> fullyOnFields = GetFullyOnFields();
        foreach (FieldController field in fullyOnFields)
        {
            FieldType currentFieldType = field.ScriptableObject.FieldType;
            if (currentFieldType == type) return true;
        }

        return false;
    }

    public bool IsOnWater() => IsFullyOnField(FieldType.Water);

    public bool IsOnIce() => IsFullyOnField(FieldType.Ice);

    public ConveyorController GetCurrentConveyor()
    {
        if (!IsFullyOnField(FieldType.Conveyor)) return null;

        List<FieldController> fullyOnFields = GetFullyOnFields();
        foreach (FieldController field in fullyOnFields)
        {
            FieldType currentFieldType = field.ScriptableObject.FieldType;
            if (currentFieldType == FieldType.Conveyor) return field.GetComponent<ConveyorController>();
        }

        return null;
    }

    public FieldController GetCurrentVoid()
    {
        // returns the void the player falls into (null if none)
        List<FieldController> fullyOnFields = GetFullyOnFields();
        foreach (FieldController field in fullyOnFields)
        {
            FieldType currentFieldType = field.ScriptableObject.FieldType;
            if (currentFieldType == FieldType.Void) return field;
        }

        return null;
    }

    public bool IsOnVoid() =>
        // we don't need that, its just there lol
        IsFullyOnField(FieldType.Void);

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

    public void DieNormal(string soundEffect = "Smack")
    {
        if (Won) return;

        // default dying
        // avoid dying while in animation
        if (!InDeathAnim)
        {
            // animation trigger and no movement
            DeathAnim();
            if (MultiplayerManager.Instance.Multiplayer) PhotonView.RPC("DeathAnim", RpcTarget.Others);
        }

        // avoid doing more if not own view in multiplayer
        if (MultiplayerManager.Instance.Multiplayer && !PhotonView.IsMine) return;

        if (EditModeManager.Instance.Playing)
            // sfx and death counter
            AudioManager.Instance.Play(soundEffect);

        Die();
    }

    public void DieVoid()
    {
        if (Won) return;

        // dying through void
        FieldController currentVoid = GetCurrentVoid();

        Vector2 fallPosition = currentVoid.transform.position;

        spriteRenderer.material.DOFade(0, voidFallDuration)
            .SetEase(Ease.Linear);

        transform.DOMove(fallPosition, voidFallDuration)
            .SetEase(Ease.OutQuint);

        transform.DOScale(Vector2.zero, voidFallDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(DeathAnimFinish);

        AudioManager.Instance.Play("Void");

        Die();
    }

    private void Die()
    {
        // // general method when dying in any way

        Rb.simulated = false;
        InDeathAnim = true;

        // avoid doing more if not own view in multiplayer
        if (MultiplayerManager.Instance.Multiplayer && !PhotonView.IsMine) return;

        if (EditModeManager.Instance.Playing)
        {
            Deaths++;
            if (!LevelSessionManager.Instance.IsEdit) LevelSessionManager.Instance.Deaths++;
        }
        
        UpdateCoinCounterDeath();

        if (KonamiManager.Instance.KonamiActive) return;
        
        PlayManager.Instance.Cheated = false;

        // reset balls to start position (if player launched them e.g. with shotgun)
        foreach (AnchorBallController ball in AnchorBallManager.Instance.AnchorBallList) ball.ResetPosition();
    }

    private void UpdateCoinCounterDeath()
    {
        // update coin counter
        bool hasCheckpointActivated = CurrentGameState != null;
        CoinsCollected.Clear();
        
        if (!hasCheckpointActivated) return;
        
        foreach (Vector2 coinPos in CurrentGameState.CollectedCoins)
        {
            CoinController coin = CoinManager.GetCoin(coinPos);
            if (coin != null) CoinsCollected.Add(coin);
        }
    }

    [PunRPC]
    public void DeathAnim() => Animator.SetTrigger(death);


    public void DeathAnimFinish()
    {
        DestroySelf(false);

        if (MultiplayerManager.Instance.Multiplayer && !PhotonView.IsMine) return;

        // reset timer if no checkpoint activated
        bool hasCheckpointActivated = CurrentGameState != null;
        if (!hasCheckpointActivated) ReferenceManager.Instance.TimerController.ResetTimer();

        Won = false;
        InDeathAnim = false;

        // create new player at start position
        CreateRespawnPlayer();

        ResetCoinsToCurrentGameState();
        ResetKeysToCurrentGameState();

        string[] tags =
            { "GrayKeyDoor", "RedKeyDoor", "GreenKeyDoor", "BlueKeyDoor", "YellowKeyDoor", };

        foreach (string tag in tags)
        {
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tag))
            {
                KeyDoorFieldController comp = door.GetComponent<KeyDoorFieldController>();
                if (!AllKeysCollected(comp.Color)) comp.SetLocked(true);
            }
        }
    }

    #endregion

    public bool AllCoinsCollected() => CoinsCollected.Count >= ReferenceManager.Instance.CoinContainer.childCount;

    public void UncollectCoinAtPos(Vector2 position)
    {
        for (int i = CoinsCollected.Count - 1; i >= 0; i--)
        {
            CoinController c = CoinsCollected[i];
            if (c.CoinPosition == position) CoinsCollected.Remove(c);
        }
    }

    public bool AllKeysCollected(KeyManager.KeyColor color)
    {
        // check if every key of specific color is picked up
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            if (!key.PickedUp && key.Color == color) return false;
        }

        return true;
    }

    private void UpdateSpeedText() => speedText.text = Speed.ToString("0.0");

    public void ResetGame() => Rb.MovePosition(StartPos);

    public void DestroySelf(bool removeTargetFromCamera = true)
    {
        if (removeTargetFromCamera && ReferenceManager.Instance.MainCameraJumper.GetTarget("Player") == gameObject)
            ReferenceManager.Instance.MainCameraJumper.RemoveTarget("Player");

        if (LevelSessionManager.Instance.IsEdit) Destroy(sliderController.GetSliderObject());

        if (nameTagController != null) Destroy(nameTagController.NameTag);

        Destroy(gameObject);
    }

    public void ActivateCheckpoint(Vector2 position)
    {
        GameState newState = GetGameStateNow();
        newState.PlayerStartPos = position;

        CurrentGameState = newState;

        print("Saved game state");
    }

    public GameState GetGameStateNow()
    {
        CoinsCollected.RemoveAll(e => e == null);
        KeysCollected.RemoveAll(e => e == null);

        // mx my coords of checkpoint field
        Vector2 statePlayerStartingPos = CurrentGameState?.PlayerStartPos ?? StartPos;

        // serialize game state
        // convert collectedCoins and collectedKeys to List<Vector2>
        List<Vector2> coinPositions = new();

        foreach (CoinController c in CoinsCollected) coinPositions.Add(c.CoinPosition);

        List<Vector2> keyPositions = new();
        foreach (KeyController key in KeysCollected) keyPositions.Add(key.KeyPosition);

        GameState res = new(statePlayerStartingPos, coinPositions, keyPositions);

        return res;
    }


    private void ResetCoinsToCurrentGameState()
    {
        foreach (CoinController coin in CoinManager.Instance.Coins)
        {
            if (!ShouldCoinRespawn(coin)) continue;

            CoinsCollected.Remove(coin);

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

            KeysCollected.Remove(key);

            key.PickedUp = false;
            key.Animator.SetBool(pickedUp, false);
        }
    }

    private void CreateRespawnPlayer()
    {
        float applySpeed = Speed;
        int deaths = Deaths;

        Vector2 spawnPos = !EditModeManager.Instance.Playing || CurrentGameState == null
            ? StartPos
            : CurrentGameState.PlayerStartPos;

        PlayerController player = PlayerManager.InstantiatePlayer(spawnPos, applySpeed, MultiplayerManager.Instance.Multiplayer);

        player.Deaths = deaths;
        player.StartPos = StartPos;
        player.CurrentGameState = CurrentGameState;

        if (Camera.main == null) return;
        JumpToEntity jumpToPlayer = Camera.main.GetComponent<JumpToEntity>();
        if (jumpToPlayer.GetTarget("Player") == gameObject) jumpToPlayer.AddTarget("Player", player.gameObject);
    }

    public void SyncToLevelSettings()
    {
        drownDuration = LevelSettings.Instance.DrownDuration;
        waterDamping = LevelSettings.Instance.WaterDamping;
        iceFriction = LevelSettings.Instance.IceFriction;
        maxIceSpeed = LevelSettings.Instance.IceMaxSpeed;
    }

    private void InitComponents()
    {
        bool isEdit = LevelSessionManager.Instance.IsEdit;

        CoinsCollected = new();

        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        EdgeCollider = GetComponent<EdgeCollider2D>();

        PhotonView = GetComponent<PhotonView>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (isEdit)
        {
            sliderController = GetComponent<AppendSlider>();
            speedText = sliderController.GetSliderObject().transform.GetChild(1).GetComponent<TMP_Text>();
        }

        if (MultiplayerManager.Instance.Multiplayer)
        {
            nameTagController = GetComponent<AppendNameTag>();
            nameTagController.SetNameTag(PhotonView.Controller.NickName);
        }

        Shotgun = GetComponentInChildren<ShotgunController>(true);
        Shotgun.gameObject.SetActive(
            isEdit ? EditModeManager.Instance.Playing && KonamiManager.Instance.KonamiActive : KonamiManager.Instance.KonamiActive
        );
    }

    private void InitSlider()
    {
        // make slider follow player
        GameObject sliderObject = sliderController.GetSliderObject();
        sliderObject.GetComponent<UIFollowEntity>().Entity = gameObject;

        // update speed every time changed
        Slider slider = sliderController.GetSlider();
        slider.onValueChanged.AddListener(
            _ =>
            {
                float newSpeed = sliderController.GetValue();

                Speed = newSpeed;

                UpdateSpeedText();

                if (MultiplayerManager.Instance.Multiplayer) PhotonView.RPC("SetSpeed", RpcTarget.Others, newSpeed);
            }
        );

        UIFollowEntity follow = sliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.Entity = gameObject;
        follow.Offset = new(0, 0.5f);
    }

    private void ApplyCurrentGameState()
    {
        // set progress from current state
        if (CurrentGameState == null) return;

        foreach (Vector2 coinCollectedPos in CurrentGameState.CollectedCoins)
        {
            CoinController coin = CoinManager.GetCoin(coinCollectedPos);
            if (coin == null) throw new Exception("Passed game state has null value for coin");

            CoinsCollected.Add(coin);
        }

        foreach (Vector2 keyCollectedPos in CurrentGameState.CollectedKeys)
        {
            KeyController key = KeyManager.GetKey(keyCollectedPos);
            if (key == null) throw new Exception("Passed game state has null value for key");

            KeysCollected.Add(key);
        }
    }

    public override Data GetData() => new PlayerData(this);
}