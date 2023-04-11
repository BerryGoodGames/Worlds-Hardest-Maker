using System;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : Controller
{
    #region Variables

    #region Editor variables

    [SerializeField] private Text speedText;

    [FormerlySerializedAs("speed")] [Space]
    public float Speed;

    [Space]
    // water settings
    [SerializeField]
    private Transform waterLevel;

    [SerializeField] [Range(0, 1)] private float waterDamping;
    [SerializeField] private float drownDuration;
    private float currentDrownDuration;

    [Space]
    // ice settings
    [SerializeField]
    private float iceFriction;

    [SerializeField] private float maxIceSpeed;

    [Space]
    // void setting(s)
    [SerializeField]
    private float voidSuckDuration;

    #endregion

    #region Components

    [FormerlySerializedAs("rb")] [HideInInspector]
    public Rigidbody2D Rb;

    [FormerlySerializedAs("animator")] [HideInInspector]
    public Animator Animator;

    [FormerlySerializedAs("edgeCollider")] [HideInInspector]
    public EdgeCollider2D EdgeCollider;

    private AppendSlider sliderController;
    private AppendNameTag nameTagController;

    [FormerlySerializedAs("photonView")] [HideInInspector]
    public PhotonView PhotonView;

    private SpriteRenderer spriteRenderer;

    #endregion

    #region Fields

    [FormerlySerializedAs("id")] [HideInInspector]
    public int ID;

    [FormerlySerializedAs("deaths")] [HideInInspector]
    public int Deaths;

    [FormerlySerializedAs("coinsCollected")] [HideInInspector]
    public List<GameObject> CoinsCollected;

    [FormerlySerializedAs("keysCollected")] [HideInInspector]
    public List<GameObject> KeysCollected;

    [FormerlySerializedAs("currentFields")] [HideInInspector]
    public List<GameObject> CurrentFields;

    public GameState CurrentGameState;

    [FormerlySerializedAs("startPos")] [HideInInspector]
    public Vector2 StartPos;

    private Vector2 movementInput;
    private Vector2 extraMovementInput;

    [FormerlySerializedAs("inDeathAnim")] [HideInInspector]
    public bool InDeathAnim;

    private bool onWater;

    [FormerlySerializedAs("won")] public bool Won;

    #endregion

    private static readonly int death = Animator.StringToHash("Death");
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");

    #endregion

    private void Awake()
    {
        InitComponents();
        InitSlider();

        StartPos = transform.position;
    }

    private void Start()
    {
        EditModeManager.Instance.OnEdit += () =>
        {
            if (Won && Animator != null)
                Animator.SetTrigger(death);
        };

        if (transform.parent != ReferenceManager.Instance.PlayerContainer)
            transform.SetParent(ReferenceManager.Instance.PlayerContainer);

        ApplyCurrentState();

        UpdateSpeedText();

        if (MultiplayerManager.Instance.Multiplayer)
            PhotonView.RPC("SetNameTagActive", RpcTarget.All, EditModeManager.Instance.Playing);

        SyncToLevelSettings();
    }

    private void Update()
    {
        movementInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (MultiplayerManager.Instance.Multiplayer)
            PhotonView.RPC("SetNameTagActive", RpcTarget.All, EditModeManager.Instance.Playing);
    }

    private void OnCollisionStay2D(Collision2D collider)
    {
        CornerPush(collider);
    }

    private void FixedUpdate()
    {
        UpdateWaterState();

        Move();
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

            if (ice)
                IcePhysics();
            else
                UpdateMovement(ref totalMovement);
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
        else if (!InDeathAnim && !onWater)
        {
            currentDrownDuration = 0;
        }

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
            totalMovement += GetCurrentSpeed() * Time.fixedDeltaTime * new Vector2(
                Mathf.Clamp(movementInput.x + extraMovementInput.x, -1, 1),
                Mathf.Clamp(movementInput.y + extraMovementInput.y, -1, 1));
        extraMovementInput = Vector2.zero;
    }

    private void UpdateConveyorMovement(ref Vector2 totalMovement)
    {
        ConveyorController conveyor = GetCurrentConveyor();
        if (conveyor == null) return;

        Vector2 conveyorVector = conveyor.Strength * Time.fixedDeltaTime * (conveyor.transform.rotation * Vector2.up);

        conveyorVector = Quaternion.Euler(0, 0, conveyor.Rotation) * conveyorVector;
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
        // do vertical
        if (movementInput.y == 0 || roundedPos.x.EqualsFloat(Mathf.Round(collider.transform.position.x)) ||
            !(Mathf.Abs(Rb.position.x) % 1 > (1 - transform.lossyScale.x) * 0.5f + err) ||
            !(Mathf.Abs(Rb.position.x) % 1 < 1 - ((1 - transform.lossyScale.x) * 0.5f + err))) return;

        Vector2 posCheck = new(roundedPos.x, Mathf.Round(Rb.position.y + movementInput.y));
        if (FieldManager.GetFieldType(FieldManager.GetField(posCheck)) != FieldType.WALL_FIELD)
            extraMovementInput = new Vector2(Rb.position.x % 1 > 0.5f ? 1 : -1, movementInput.y);
    }

    private void CornerPushHorizontal(Collision2D collider, Vector2 roundedPos, float err)
    {
        // do horizontal
        if (movementInput.x == 0 || roundedPos.y.EqualsFloat(Mathf.Round(collider.transform.position.y)) ||
            !(Mathf.Abs(Rb.position.y) % 1 > (1 - transform.lossyScale.y) * 0.5f + err) ||
            !(Mathf.Abs(Rb.position.y) % 1 < 1 - ((1 - transform.lossyScale.y) * 0.5f + err))) return;

        Vector2 posCheck = new(Mathf.Round(Rb.position.x + movementInput.x), roundedPos.y);
        if (FieldManager.GetFieldType(FieldManager.GetField(posCheck)) != FieldType.WALL_FIELD)
            extraMovementInput = new Vector2(movementInput.x, Rb.position.y % 1 > 0.5f ? 1 : -1);
    }

    #endregion

    /// <summary>
    ///     Always use SetSpeed instead of setting
    /// </summary>
    [PunRPC]
    public void SetSpeed(float speed)
    {
        Speed = speed;

        // sync slider
        float currentSliderValue = sliderController.GetValue() / sliderController.Step;
        if (!currentSliderValue.EqualsFloat(speed))
            sliderController.GetSlider().SetValueWithoutNotify(speed / sliderController.Step);
    }

    [PunRPC]
    public void SetNameTagActive(bool active)
    {
        if (!PhotonView.IsMine) print($"Set name tag {active}");

        if (!MultiplayerManager.Instance.Multiplayer)
            throw new Exception("Trying to enable/disable name tag while in singleplayer");
        nameTagController.NameTag.SetActive(active);
    }

    /// <returns>rounded position of player</returns>
    public Vector2 GetMatrixPos() => transform.position.Floor();

    #region Field detection

    public bool IsOnSafeField()
    {
        foreach (GameObject field in CurrentFields)
        {
            // check if current field is safe
            FieldType? currentFieldType = FieldManager.GetFieldType(field);
            if (PlayerManager.SafeFields.Contains((FieldType)currentFieldType)) return true;
        }

        return false;
    }

    public bool IsOnField(FieldType type)
    {
        foreach (GameObject field in CurrentFields)
        {
            // check if current field is type
            FieldType? currentFieldType = FieldManager.GetFieldType(field);
            if (currentFieldType == type) return true;
        }

        return false;
    }

    public List<GameObject> GetFullyOnFields()
    {
        // finds every field the player is at least half way on
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.011f);
        List<GameObject> res = new();
        foreach (Collider2D hit in hits) res.Add(hit.gameObject);

        return res;
    }

    public bool IsFullyOnField(FieldType type)
    {
        List<GameObject> fullyOnFields = GetFullyOnFields();
        foreach (GameObject field in fullyOnFields)
        {
            FieldType? currentFieldType = FieldManager.GetFieldType(field);
            if (currentFieldType == type) return true;
        }

        return false;
    }

    public bool IsOnWater() => IsFullyOnField(FieldType.WATER);

    public bool IsOnIce() => IsFullyOnField(FieldType.ICE);

    public ConveyorController GetCurrentConveyor()
    {
        if (!IsFullyOnField(FieldType.CONVEYOR)) return null;

        List<GameObject> fullyOnFields = GetFullyOnFields();
        foreach (GameObject field in fullyOnFields)
        {
            FieldType? currentFieldType = FieldManager.GetFieldType(field);
            if (currentFieldType == FieldType.CONVEYOR) return field.GetComponent<ConveyorController>();
        }

        return null;
    }

    public GameObject CurrentVoid()
    {
        // returns the void the player gets sucked to (null if none)
        List<GameObject> fullyOnFields = GetFullyOnFields();
        foreach (GameObject field in fullyOnFields)
        {
            FieldType? currentFieldType = FieldManager.GetFieldType(field);
            if (currentFieldType == FieldType.VOID) return field;
        }

        return null;
    }

    public bool IsOnVoid() =>
        // we don't need that, its just there lol
        IsFullyOnField(FieldType.VOID);

    public GameObject GetCurrentField() => FieldManager.GetField(transform.position.Round());

    #endregion

    public void Win()
    {
        if (InDeathAnim || Won) return;
        // animation and play mode and that's it really
        AudioManager.Instance.Play("Win");
        Won = true;

        PlayerManager.Instance.InvokeOnWin();
    }

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
        Vector2 suckPosition = (Vector2)transform.position + movementInput * 0.5f;

        spriteRenderer.material.DOFade(0, voidSuckDuration)
            .SetEase(Ease.Linear);
        transform.DOMove(suckPosition, voidSuckDuration)
            .SetEase(Ease.OutQuint);
        transform.DOScale(Vector2.zero, voidSuckDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(DeathAnimFinish);

        AudioManager.Instance.Play("Void");

        Die();
    }

    private void Die()
    {
        // general method when dying in any way
        Rb.simulated = false;
        InDeathAnim = true;

        // avoid doing more if not own view in multiplayer
        if (MultiplayerManager.Instance.Multiplayer && !PhotonView.IsMine) return;

        if (EditModeManager.Instance.Playing) Deaths++;

        // update coin counter
        CoinsCollected.Clear();
        if (CurrentGameState == null) return;

        foreach (Vector2 coinPos in CurrentGameState.CollectedCoins)
        {
            GameObject coin = CoinManager.GetCoin(coinPos);
            if (coin != null) CoinsCollected.Add(coin);
        }
    }

    [PunRPC]
    public void DeathAnim()
    {
        Animator.SetTrigger(death);
    }

    public bool AllCoinsCollected() => CoinsCollected.Count >= ReferenceManager.Instance.CoinContainer.childCount;

    public void UncollectCoinAtPos(Vector2 pos)
    {
        for (int i = CoinsCollected.Count - 1; i >= 0; i--)
        {
            GameObject c = CoinsCollected[i];
            if (c.GetComponent<CoinController>().CoinPosition == pos) CoinsCollected.Remove(c);
        }
    }

    public bool AllKeysCollected(KeyManager.KeyColor color)
    {
        foreach (Transform k in ReferenceManager.Instance.KeyContainer)
        {
            KeyController controller = k.GetChild(0).GetComponent<KeyController>();
            if (!controller.PickedUp && controller.Color == color) return false;
        }

        return true;
    }

    public void UpdateSpeedText()
    {
        speedText.text = $"Speed: {Speed:0.0}";
    }

    public void ResetGame()
    {
        Rb.MovePosition(StartPos);
    }

    public void DestroyPlayer(bool removeTargetFromCamera = true)
    {
        if (Camera.main != null)
        {
            JumpToEntity camera = Camera.main.GetComponent<JumpToEntity>();
            if (removeTargetFromCamera && camera.Target == gameObject) camera.Target = null;
        }

        Destroy(sliderController.GetSliderObject());
        if (nameTagController != null) Destroy(nameTagController.NameTag);
        Destroy(gameObject);
    }

    public void ActivateCheckpoint(float mx, float my)
    {
        // mx my coords of checkpoint field
        Vector2 statePlayerStartingPos = new(mx, my);

        GameState newState = GetGameStateNow();
        newState.PlayerStartPos = statePlayerStartingPos;

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

        foreach (GameObject c in CoinsCollected) coinPositions.Add(c.GetComponent<CoinController>().CoinPosition);

        List<Vector2> keyPositions = new();
        foreach (GameObject k in KeysCollected)
        {
            KeyController keyController = k.GetComponent<KeyController>();
            keyPositions.Add(keyController.KeyPosition);
        }

        GameState res = new(statePlayerStartingPos, coinPositions, keyPositions);

        return res;
    }

    public void DeathAnimFinish()
    {
        DestroyPlayer(false);

        if (MultiplayerManager.Instance.Multiplayer && !PhotonView.IsMine) return;

        Won = false;
        InDeathAnim = false;

        // create new player at start position
        CreateRespawnPlayer();

        ResetCoinsToCurrentGameState();
        ResetKeysToCurrentGameState();

        string[] tags =
            { "KeyDoorField", "RedKeyDoorField", "GreenKeyDoorField", "BlueKeyDoorField", "YellowKeyDoorField" };
        foreach (string tag in tags)
        {
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tag))
            {
                KeyDoorField comp = door.GetComponent<KeyDoorField>();
                if (!AllKeysCollected(comp.Color)) comp.Lock(true);
            }
        }
    }


    private void ResetCoinsToCurrentGameState()
    {
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer)
        {
            CoinController coinController = coin.GetChild(0).GetComponent<CoinController>();

            bool respawns = true;
            if (CurrentGameState != null)
                foreach (Vector2 collected in CurrentGameState.CollectedCoins)
                {
                    if (!collected.x.EqualsFloat(coinController.CoinPosition.x) ||
                        !collected.y.EqualsFloat(coinController.CoinPosition.y)) continue;

                    // if coin is collected or no state exists it doesn't respawn
                    respawns = false;
                    break;
                }

            if (!respawns) continue;

            CoinsCollected.Remove(coin.gameObject);

            coinController.PickedUp = false;

            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool(pickedUp, false);
        }
    }

    private void ResetKeysToCurrentGameState()
    {
        foreach (Transform key in ReferenceManager.Instance.KeyContainer)
        {
            KeyController keyController = key.GetChild(0).GetComponent<KeyController>();

            bool respawns = true;
            if (CurrentGameState != null)
                foreach (Vector2 collected in CurrentGameState.CollectedKeys)
                {
                    if (!collected.x.EqualsFloat(keyController.KeyPosition.x) ||
                        !collected.y.EqualsFloat(keyController.KeyPosition.y)) continue;

                    // if key is collected or no state exists it doesn't respawn
                    respawns = false;
                    break;
                }

            if (!respawns) continue;
            KeysCollected.Remove(key.gameObject);

            keyController.PickedUp = false;

            Animator anim = key.GetComponent<Animator>();
            anim.SetBool(pickedUp, false);
        }
    }

    private void CreateRespawnPlayer()
    {
        float applySpeed = Speed;
        int deaths = Deaths;

        Vector2 spawnPos = !EditModeManager.Instance.Playing || CurrentGameState == null
            ? StartPos
            : CurrentGameState.PlayerStartPos;

        GameObject player =
            PlayerManager.InstantiatePlayer(spawnPos, applySpeed, MultiplayerManager.Instance.Multiplayer);
        PlayerController newController = player.GetComponent<PlayerController>();
        newController.Deaths = deaths;
        newController.StartPos = StartPos;
        newController.CurrentGameState = CurrentGameState;

        if (Camera.main == null) return;
        JumpToEntity jumpToPlayer = Camera.main.GetComponent<JumpToEntity>();
        if (jumpToPlayer.Target == gameObject) jumpToPlayer.Target = player;
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
        CoinsCollected = new();

        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        sliderController = GetComponent<AppendSlider>();

        EdgeCollider = GetComponent<EdgeCollider2D>();

        PhotonView = GetComponent<PhotonView>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        speedText = sliderController.GetSliderObject().transform.GetChild(0).GetComponent<Text>();

        if (!MultiplayerManager.Instance.Multiplayer) return;

        nameTagController = GetComponent<AppendNameTag>();
        nameTagController.SetNameTag(PhotonView.Controller.NickName);
    }

    private void InitSlider()
    {
        // make slider follow player
        GameObject sliderObject = sliderController.GetSliderObject();
        sliderObject.GetComponent<UIFollowEntity>().Entity = gameObject;

        // update speed every time changed
        Slider slider = sliderController.GetSlider();
        slider.onValueChanged.AddListener(_ =>
        {
            float newSpeed = sliderController.GetValue();

            Speed = newSpeed;

            UpdateSpeedText();

            if (MultiplayerManager.Instance.Multiplayer) PhotonView.RPC("SetSpeed", RpcTarget.Others, newSpeed);
        });

        UIFollowEntity follow = sliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.Entity = gameObject;
        follow.Offset = new(0, 0.5f);
    }

    private void ApplyCurrentState()
    {
        // set progress from current state
        if (CurrentGameState == null) return;

        foreach (Vector2 coinCollectedPos in CurrentGameState.CollectedCoins)
        {
            GameObject coin = CoinManager.GetCoin(coinCollectedPos);
            if (coin == null) throw new Exception("Passed game state has null value for coin");

            CoinsCollected.Add(coin);
        }

        foreach (Vector2 keyCollectedPos in CurrentGameState.CollectedKeys)
        {
            GameObject key = KeyManager.GetKey(keyCollectedPos);
            if (key == null) throw new Exception("Passed game state has null value for key");

            KeysCollected.Add(key);
        }
    }

    public override Data GetData() => new PlayerData(this);
}