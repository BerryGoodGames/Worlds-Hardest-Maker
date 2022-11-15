using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using DG.Tweening;

public class CPlayer : MonoBehaviour
{
    [HideInInspector] public int id;

    public float speed;
    [Space]
    // water settings
    [SerializeField] private Transform waterLevel;
    [SerializeField][Range(0, 1)] private float waterDamping;
    [SerializeField] private float drownDuration;
    private float currentDrownDuration = 0;
    [Space]
    // ice settings
    [SerializeField] private float iceFriction;
    [SerializeField] private float maxIceSpeed;
    [Space]
    // void setting(s)
    [SerializeField] private float voidSuckDuration;

    [HideInInspector] public int deaths = 0;

    [HideInInspector] public List<GameObject> coinsCollected;
    [HideInInspector] public List<GameObject> keysCollected;

    [HideInInspector] public List<GameObject> currentFields;

    [HideInInspector] public GameState currentState = null;

    [HideInInspector] public Vector2 startPos;

    // components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public EdgeCollider2D edgeCollider;
    private AppendSlider sliderController;
    private AppendNameTag nameTagController = null;
    [HideInInspector] public PhotonView photonView;
    private SpriteRenderer spriteRenderer;

    private Vector2 movementInput;
    private Vector2 extraMovementInput;

    [HideInInspector] public bool inDeathAnim = false;

    private bool onWater = false;

    private void Awake()
    {
        coinsCollected = new();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sliderController = GetComponent<AppendSlider>();

        edgeCollider = GetComponent<EdgeCollider2D>();

        photonView = GetComponent<PhotonView>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (MGame.Instance.Multiplayer)
        {
            nameTagController = GetComponent<AppendNameTag>();
            nameTagController.SetNameTag(photonView.Controller.NickName);
        }

        // make slider follow player
        GameObject sliderObject = sliderController.GetSliderObject();
        sliderObject.GetComponent<UIFollowEntity>().entity = gameObject;

        // update speed every time changed
        Slider slider = sliderController.GetSlider();
        slider.onValueChanged.AddListener((value) =>
        {
            float newSpeed = sliderController.GetValue();

            speed = newSpeed;

            UpdateSpeedText();

            if (MGame.Instance.Multiplayer) photonView.RPC("SetSpeed", RpcTarget.Others, newSpeed);
        });

        UIFollowEntity follow = sliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.entity = gameObject;
        follow.offset = new(0, 0.5f);

        startPos = transform.position;
    }

    private void Start()
    {
        if(transform.parent != MGame.Instance.PlayerContainer.transform)
        {
            transform.SetParent(MGame.Instance.PlayerContainer.transform);
        }

        // set progress from current state
        if(currentState != null)
        {
            foreach(Vector2 coinCollectedPos in currentState.collectedCoins)
            {
                GameObject coin = MCoin.GetCoin(coinCollectedPos);
                if (coin == null) throw new System.Exception("Passed game state has null value for coin");

                coinsCollected.Add(coin);
            }

            foreach (Vector2 keyCollectedPos in currentState.collectedKeys)
            {
                GameObject key = MKey.GetKey(keyCollectedPos);
                if (key == null) throw new System.Exception("Passed game state has null value for key");

                keysCollected.Add(key);
            }
        }

        UpdateSpeedText();

        if (MGame.Instance.Multiplayer) photonView.RPC("SetNameTagActive", RpcTarget.All, MGame.Instance.Playing);

        SyncToLevelSettings();
    }

    private void Update()
    {
        movementInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (MGame.Instance.Multiplayer) photonView.RPC("SetNameTagActive", RpcTarget.All, MGame.Instance.Playing);
    }

    private void OnCollisionStay2D(Collision2D collider)
    {
        Vector2 roundedPos = new(Mathf.Round(rb.position.x), Mathf.Round(rb.position.y));

        float err = 0.00001f;

        // do wall corner pushy thingy
        if (collider.transform.tag.IsSolidFieldTag() && (collider.transform.position.x == roundedPos.x + movementInput.x || collider.transform.position.y == roundedPos.y + movementInput.y))
        {

            // do horizontal
            if (movementInput.x != 0 && roundedPos.y != Mathf.Round(collider.transform.position.y) && 
                Mathf.Abs(rb.position.y) % 1 > ((1 - transform.lossyScale.y) * 0.5f + err) && 
                Mathf.Abs(rb.position.y) % 1 < (1 - ((1 - transform.lossyScale.y) * 0.5f + err)))
            {
                Vector2 posCheck = new(Mathf.Round(rb.position.x + movementInput.x), roundedPos.y);
                if (MField.GetFieldType(MField.GetField(posCheck)) != MField.FieldType.WALL_FIELD)
                {
                    //transform.position = new Vector2(transform.position.x, roundedPos.y + (transform.position.y % 1 > 0.5f ? -1 : 1) * (1 - transform.lossyScale.y) * 0.5f);
                    extraMovementInput = new Vector2(movementInput.x, (rb.position.y % 1 > 0.5f ? 1 : -1));
                    return;
                }
            }
            // do vertical
            if (movementInput.y != 0 && roundedPos.x != Mathf.Round(collider.transform.position.x) && 
                Mathf.Abs(rb.position.x) % 1 > ((1 - transform.lossyScale.x) * 0.5f + err) && 
                Mathf.Abs(rb.position.x) % 1 < (1 - ((1 - transform.lossyScale.x) * 0.5f + err)))
            {
                Vector2 posCheck = new(Mathf.Round(rb.position.y + movementInput.y), roundedPos.x);
                if (MField.GetFieldType(MField.GetField(posCheck)) != MField.FieldType.WALL_FIELD)
                {
                    //transform.position = new Vector2(roundedPos.x + (transform.position.x % 1 > 0.5f ? -1 : 1) * (1 - transform.lossyScale.x) * 0.5f, transform.position.y);
                    extraMovementInput = new Vector2((rb.position.x % 1 > 0.5f ? 1 : -1), movementInput.y);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        // check water and update drown level
        bool onWaterNow = IsOnWater();
        if (!onWater && onWaterNow)
        {
            // frame player enters water
            MAudio.Instance.Play("WaterEnter");
        }

        onWater = onWaterNow;

        if (onWater && !inDeathAnim)
        {
            currentDrownDuration += Time.fixedDeltaTime;

            if (currentDrownDuration >= drownDuration)
            {
                DieNormal("Drown");
            }
        }
        else if(!inDeathAnim && !onWater) currentDrownDuration = 0;

        if (drownDuration != 0)
        {
            float drown = currentDrownDuration / drownDuration;
            waterLevel.localScale = new(waterLevel.localScale.x, drown);
        }

        bool ice = IsOnIce();


        Vector2 totalMovement = Vector2.zero;
        // movement (if player is yours in multiplayer mode)
        if (MGame.Instance.Playing)
        {
            if (MGame.Instance.Multiplayer && !photonView.IsMine) return;

            if (ice) 
            {
                // transfer velocity to ice when entering
                if (rb.velocity == Vector2.zero) rb.velocity = (onWater ? waterDamping * speed : speed) * movementInput;

                // acceleration on ice
                rb.drag = iceFriction;
                rb.AddForce(iceFriction * speed * 1.2f * movementInput, ForceMode2D.Force);

                rb.velocity = new(Mathf.Min(maxIceSpeed, rb.velocity.x), Mathf.Min(maxIceSpeed, rb.velocity.y));
            }
            else
            {
                rb.velocity = Vector2.zero;

                // snappy movement (when not on ice)
                if (!movementInput.Equals(Vector2.zero))
                    totalMovement += (onWater ? waterDamping * speed : speed) * Time.fixedDeltaTime * new Vector2(Mathf.Clamp(movementInput.x + extraMovementInput.x, -1, 1), Mathf.Clamp(movementInput.y + extraMovementInput.y, -1, 1));
                extraMovementInput = Vector2.zero;
            }
        }

        // get conveyor speed
        CConveyor conveyor = GetCurrentConveyor();
        if (conveyor != null)
        {
            Vector2 conveyorVector = conveyor.Strength * Time.fixedDeltaTime * (conveyor.transform.rotation * Vector2.up);

            conveyorVector = Quaternion.Euler(0, 0, conveyor.Rotation) * conveyorVector;
            totalMovement += conveyorVector;
        }
        rb.MovePosition(rb.position + totalMovement);
    }

    /// <summary>
    /// always use SetSpeed instead of setting
    /// </summary>
    [PunRPC]
    public void SetSpeed(float speed)
    {
        // TODO: code duplication from IBallController
        this.speed = speed;

        // sync slider
        float currentSliderValue = sliderController.GetValue() / sliderController.Step;
        if (currentSliderValue != speed)
        {
            sliderController.GetSlider().SetValueWithoutNotify(speed / sliderController.Step);
        }
    }

    [PunRPC]
    public void SetNameTagActive(bool active)
    {
        if (!photonView.IsMine) print($"Setnametag {active}");

        if (!MGame.Instance.Multiplayer) throw new System.Exception("Trying to enable/disable name tag while in singleplayer");
        nameTagController.nameTag.SetActive(active);
    }

    /// <returns>rounded position of player</returns>
    public Vector2 GetMatrixPos()
    {
        return new(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y));
    }

    #region Field detection
    public bool IsOnSafeField()
    {
        foreach (GameObject field in currentFields)
        {
            // check if current field is safe
            MField.FieldType? currentFieldType = MField.GetFieldType(field);
            if (MPlayer.SafeFields.Contains((MField.FieldType)currentFieldType))
            {
                return true;
            }
        }
        return false;
    }
    public bool IsOnField(MField.FieldType type)
    {
        foreach (GameObject field in currentFields)
        {
            // check if current field is type
            MField.FieldType? currentFieldType = MField.GetFieldType(field);
            if (currentFieldType != null && currentFieldType == type)
            {
                return true;
            }
        }
        return false;
    }
    public List<GameObject> GetFullyOnFields()
    {
        // finds every field the player is at least half way on
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.011f);
        List<GameObject> res = new();
        foreach (Collider2D hit in hits)
        {
            res.Add(hit.gameObject);
        }
        return res;
    }
    public bool IsFullyOnField(MField.FieldType type)
    {
        List<GameObject> fullyOnFields = GetFullyOnFields();
        foreach(GameObject field in fullyOnFields)
        {
            MField.FieldType? currentFieldType = MField.GetFieldType(field);
            if (currentFieldType != null && currentFieldType == type)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsOnWater()
    {
        return IsFullyOnField(MField.FieldType.WATER);
    }
    public bool IsOnIce()
    {
        return IsFullyOnField(MField.FieldType.ICE);
    }
    public CConveyor GetCurrentConveyor()
    {
        if (IsFullyOnField(MField.FieldType.CONVEYOR))
        {
            List<GameObject> fullyOnFields = GetFullyOnFields();
            foreach (GameObject field in fullyOnFields)
            {
                MField.FieldType? currentFieldType = MField.GetFieldType(field);
                if (currentFieldType != null && currentFieldType == MField.FieldType.CONVEYOR)
                {
                    return field.GetComponent<CConveyor>();
                }
            }
        }
        return null;
    }
    public GameObject CurrentVoid()
    {
        // returns the void the player gets sucked to (null if none)
        List<GameObject> fullyOnFields = GetFullyOnFields();
        foreach (GameObject field in fullyOnFields)
        {
            MField.FieldType? currentFieldType = MField.GetFieldType(field);
            if (currentFieldType != null && currentFieldType == MField.FieldType.VOID)
            {
                return field;
            }
        }
        return null;
    }
    public bool IsOnVoid()
    {
        // we dont need that, its just there lol
        return IsFullyOnField(MField.FieldType.VOID);
    }

    public GameObject GetCurrentField()
    {
        return MField.GetField((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));
    }
    #endregion

    public void Win()
    {
        // animation and play mode and that's it really
        animator.SetTrigger("Death");
        MAudio.Instance.Play("Win");
        MGame.Instance.TogglePlay(false);
    }

    public void DieNormal(string soundEffect = "Smack")
    {
        // default dying
        // avoid dying while in animation
        if (!inDeathAnim)
        {
            // animation trigger and no movement
            DeathAnim();
            if (MGame.Instance.Multiplayer) photonView.RPC("DeathAnim", RpcTarget.Others);
        }

        // avoid doing more if not own view in multiplayer
        if (MGame.Instance.Multiplayer && !photonView.IsMine) return;

        if (MGame.Instance.Playing)
        {
            // sfx and death counter
            MAudio.Instance.Play(soundEffect);
        }

        Die();
    }
    public void DieVoid()
    {

        // dying through void
        Vector2 suckPosition = (Vector2)transform.position + movementInput * 0.5f;

        spriteRenderer.material.DOFade(0, voidSuckDuration)
            .SetEase(Ease.Linear);
        transform.DOMove(suckPosition, voidSuckDuration)
            .SetEase(Ease.OutQuint);
        transform.DOScale(Vector2.zero, voidSuckDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(DeathAnimFinish);

        MAudio.Instance.Play("Void");

        Die();
    }
    private void Die()
    {
        // general method when dying in any way

        rb.simulated = false;
        inDeathAnim = true;

        // avoid doing more if not own view in multiplayer
        if (MGame.Instance.Multiplayer && !photonView.IsMine) return;

        if(MGame.Instance.Playing) deaths++;

        // update coin counter
        coinsCollected.Clear();
        if (currentState != null)
        {
            foreach (Vector2 coinPos in currentState.collectedCoins)
            {
                GameObject coin = MCoin.GetCoin(coinPos);
                if (coin != null) coinsCollected.Add(coin);
            }
        }
    }

    [PunRPC]
    public void DeathAnim()
    {
        animator.SetTrigger("Death");
    }

    public bool CoinsCollected()
    {
        return coinsCollected.Count >= MGame.Instance.CoinContainer.transform.childCount;
    }
    public void UncollectCoinAtPos(Vector2 pos)
    {
        for(int i = coinsCollected.Count - 1; i >= 0; i--)
        {
            GameObject c = coinsCollected[i];
            if (c.GetComponent<CCoin>().coinPosition == pos)
            {
                print("removedCoin");
                coinsCollected.Remove(c);
            }
        }
    }
    
    public bool KeysCollected(MKey.KeyColor color)
    {
        foreach(Transform k in MGame.Instance.KeyContainer.transform)
        {
            CKey controller = k.GetChild(0).GetComponent<CKey>();
            if (!controller.pickedUp && controller.color == color) return false;
        }

        return true;
    }

    public void UpdateSpeedText()
    {
        Text speedText = sliderController.GetSliderObject().transform.GetChild(0).GetComponent<Text>();
        speedText.text = $"Speed: {speed:0.0}";
    }

    public void ResetGame()
    {
        rb.MovePosition(startPos);
    }

    public void DestroyPlayer(bool removeTargetFromCamera = true)
    {
        JumpToEntity camera = Camera.main.GetComponent<JumpToEntity>();
        if (removeTargetFromCamera && camera.target == gameObject) camera.target = null;

        Destroy(sliderController.GetSliderObject());
        if (nameTagController != null) Destroy(nameTagController.nameTag);
        Destroy(gameObject);
    }

    public void ActivateCheckpoint(float mx, float my)
    {
        // mx my coords of checkpointfield
        Vector2 statePlayerStartingPos = new(mx, my);

        GameState newState = GetGameStateNow();
        newState.playerStartPos = statePlayerStartingPos;

        currentState = newState;

        print("Saved game state");
    }

    public GameState GetGameStateNow()
    {
        coinsCollected.RemoveAll(e => e == null);
        keysCollected.RemoveAll(e => e == null);

        // mx my coords of checkpointfield
        Vector2 statePlayerStartingPos = currentState != null ? currentState.playerStartPos : startPos;

        // serialize game state
        // convert collectedCoins and collectedKeys to List<Vector2>
        List<Vector2> coinPositions = new();

        foreach (GameObject c in coinsCollected)
        {
            coinPositions.Add(c.GetComponent<CCoin>().coinPosition);
        }

        List<Vector2> keyPositions = new();
        foreach (GameObject k in keysCollected)
        {
            CKey keyController = k.GetComponent<CKey>();
            keyPositions.Add(keyController.keyPosition);
        }

        GameState res = new(statePlayerStartingPos, coinPositions, keyPositions);

        return res;
    }

    public void DeathAnimFinish()
    {
        DestroyPlayer(false);

        if (MGame.Instance.Multiplayer && !photonView.IsMine) return;

        inDeathAnim = false;

        float applySpeed = speed;
        int deaths = this.deaths;

        // create new player at start position
        Vector2 spawnPos = !MGame.Instance.Playing || currentState == null ? startPos : currentState.playerStartPos;

        GameObject player = MPlayer.InstantiatePlayer(spawnPos, applySpeed, MGame.Instance.Multiplayer);
        CPlayer newController = player.GetComponent<CPlayer>();
        newController.deaths = deaths;
        newController.startPos = startPos;
        newController.currentState = currentState;

        JumpToEntity jumpToPlayer = Camera.main.GetComponent<JumpToEntity>();
        if (jumpToPlayer.target == gameObject) jumpToPlayer.target = player;

        // reset coins
        foreach (Transform coin in MGame.Instance.CoinContainer.transform)
        {
            CCoin coinController = coin.GetChild(0).GetComponent<CCoin>();

            bool respawns = true;
            if(currentState != null) { 
                foreach (Vector2 collected in currentState.collectedCoins)
                {
                    if (collected.x == coinController.coinPosition.x && collected.y == coinController.coinPosition.y)
                    {
                        // if coin is collected or no state exists it doesnt respawn
                        respawns = false;
                        break;
                    }
                }
            }
            
            if(respawns)
            {
                coinsCollected.Remove(coin.gameObject);

                coinController.pickedUp = false;

                Animator anim = coin.GetComponent<Animator>();
                anim.SetBool("PickedUp", false);
            }
        }

        // reset keys
        foreach (Transform key in MGame.Instance.KeyContainer.transform)
        {
            CKey keyController = key.GetChild(0).GetComponent<CKey>();

            bool respawns = true;
            if (currentState != null)
            {
                foreach (Vector2 collected in currentState.collectedKeys)
                {
                    if (collected.x == keyController.keyPosition.x && collected.y == keyController.keyPosition.y)
                    {
                        // if key is collected or no state exists it doesnt respawn
                        respawns = false;
                        break;
                    }
                }
            }

            if (respawns)
            {
                keysCollected.Remove(key.gameObject);

                keyController.pickedUp = false;

                Animator anim = key.GetComponent<Animator>();
                anim.SetBool("PickedUp", false);
            }
        }

        // reset key doors
        string[] tags = { "KeyDoorField", "RedKeyDoorField", "GreenKeyDoorField", "BlueKeyDoorField", "YellowKeyDoorField" };
        foreach (string tag in tags)
        {
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tag))
            {
                KeyDoorField comp = door.GetComponent<KeyDoorField>();
                if(!KeysCollected(comp.color)) comp.Lock(true);
            }
        }
    }

    public void SyncToLevelSettings()
    {
        drownDuration = LevelSettings.Instance.drownDuration;
        waterDamping = LevelSettings.Instance.waterDamping;
        iceFriction = LevelSettings.Instance.iceFriction;
        maxIceSpeed = LevelSettings.Instance.iceMaxSpeed;
    }
}
