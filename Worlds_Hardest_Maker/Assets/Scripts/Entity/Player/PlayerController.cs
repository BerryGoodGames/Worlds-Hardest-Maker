using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using DG.Tweening;
using System;

public class PlayerController : MonoBehaviour
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
    [Space]
    [Range(0, 0.5f)][SerializeField] private float cornerCuttingBias;

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

    private Vector2 movementInput = new(0, 0);
    private Vector2 cornerCuttingMovement = new(0, 0);

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

        if (GameManager.Instance.Multiplayer)
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

            if (GameManager.Instance.Multiplayer) photonView.RPC("SetSpeed", RpcTarget.Others, newSpeed);
        });

        UIFollowEntity follow = sliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.entity = gameObject;
        follow.offset = new(0, 0.5f);

        startPos = transform.position;
    }

    private void Start()
    {
        if(transform.parent != GameManager.Instance.PlayerContainer.transform)
        {
            transform.SetParent(GameManager.Instance.PlayerContainer.transform);
        }

        // set progress from current state
        if(currentState != null)
        {
            foreach(Vector2 coinCollectedPos in currentState.collectedCoins)
            {
                GameObject coin = CoinManager.GetCoin(coinCollectedPos);
                if (coin == null) throw new System.Exception("Passed game state has null value for coin");

                coinsCollected.Add(coin);
            }

            foreach (Vector2 keyCollectedPos in currentState.collectedKeys)
            {
                GameObject key = KeyManager.GetKey(keyCollectedPos);
                if (key == null) throw new System.Exception("Passed game state has null value for key");

                keysCollected.Add(key);
            }
        }

        UpdateSpeedText();

        if (GameManager.Instance.Multiplayer) photonView.RPC("SetNameTagActive", RpcTarget.All, GameManager.Instance.Playing);

        SyncToLevelSettings();
    }

    private void Update()
    {
        movementInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (GameManager.Instance.Multiplayer) photonView.RPC("SetNameTagActive", RpcTarget.All, GameManager.Instance.Playing);
    }

    private void FixedUpdate()
    {
        // check water and update drown level
        bool onWaterNow = IsOnWater();
        if (!onWater && onWaterNow)
        {
            // frame player enters water
            AudioManager.Instance.Play("WaterEnter");
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

        // movement (if player is yours in multiplayer mode)
        if (GameManager.Instance.Playing)
        {
            if (GameManager.Instance.Multiplayer && !photonView.IsMine) return;

            // corner cutting
            float playerWidth = transform.lossyScale.x;
            float playerHeight = transform.lossyScale.y;

            if (ice) 
            {
                // transfer velocity to ice when entering
                if (rb.velocity == Vector2.zero) rb.velocity = (onWater ? waterDamping * speed : speed) * (movementInput + cornerCuttingMovement);

                // acceleration on ice
                rb.drag = iceFriction;
                rb.AddForce(iceFriction * speed * 1.2f * (movementInput + cornerCuttingMovement), ForceMode2D.Force);

                rb.velocity = new(Mathf.Min(maxIceSpeed, rb.velocity.x), Mathf.Min(maxIceSpeed, rb.velocity.y));
            }
            else
            {
                rb.velocity = Vector2.zero;

                // snappy movement (when not on ice)
                rb.MovePosition((Vector2)rb.transform.position + (onWater ? waterDamping * speed : speed) * Time.fixedDeltaTime * (movementInput + cornerCuttingMovement));
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        float playerWidth = transform.lossyScale.x;
        float playerHeight = transform.lossyScale.y;

        GameObject collider = collision.gameObject;
        if (collider.tag.IsSolidFieldTag())
        {
            // horizontal
            if(movementInput.x != 0 && movementInput.y == 0)
            {
                float sign = Mathf.Sign(movementInput.x);

                float rayX1 = transform.position.x + (playerWidth * 0.5f + 0.05f) * sign;
                float rayY1 = transform.position.y + playerHeight * 0.5f - cornerCuttingBias * playerHeight;
                RaycastHit2D topHit = Physics2D.Raycast(new(rayX1, rayY1), new(sign, 0));

                float rayX2 = transform.position.x + (playerWidth * 0.5f + 0.05f) * sign;
                float rayY2 = transform.position.y - playerHeight * 0.5f + cornerCuttingBias * playerHeight;
                RaycastHit2D bottomHit = Physics2D.Raycast(new(rayX2, rayY2), new(sign, 0));

                Debug.DrawRay(new(rayX1, rayY1), new(sign, 0));
                Debug.DrawRay(new(rayX2, rayY2), new(sign, 0));
                if (topHit ^ bottomHit)
                {
                    RaycastHit2D hit = topHit ? topHit : bottomHit;
                    if (hit.collider.tag.IsSolidFieldTag())
                    {
                        cornerCuttingMovement = new(0, topHit ? -1 : 1);
                        return;
                    }
                }
            }
        }
        //cornerCuttingMovement = new(0, 0);
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

        if (!GameManager.Instance.Multiplayer) throw new System.Exception("Trying to enable/disable name tag while in singleplayer");
        nameTagController.nameTag.SetActive(active);
    }

    /// <returns>rounded position of player</returns>
    public Vector2 GetMatrixPos()
    {
        return new(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y));
    }

    #region FIELD DETECTION
    public bool IsOnSafeField()
    {
        foreach (GameObject field in currentFields)
        {
            // check if current field is safe
            FieldManager.FieldType? currentFieldType = FieldManager.GetFieldType(field);
            if (PlayerManager.SafeFields.Contains((FieldManager.FieldType)currentFieldType))
            {
                return true;
            }
        }
        return false;
    }
    public bool IsOnField(FieldManager.FieldType type)
    {
        foreach (GameObject field in currentFields)
        {
            // check if current field is type
            FieldManager.FieldType? currentFieldType = FieldManager.GetFieldType(field);
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
    public bool IsFullyOnField(FieldManager.FieldType type)
    {
        List<GameObject> fullyOnFields = GetFullyOnFields();
        foreach(GameObject field in fullyOnFields)
        {
            FieldManager.FieldType? currentFieldType = FieldManager.GetFieldType(field);
            if (currentFieldType != null && currentFieldType == type)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsOnWater()
    {
        return IsFullyOnField(FieldManager.FieldType.WATER);
    }
    public bool IsOnIce()
    {
        return IsFullyOnField(FieldManager.FieldType.ICE);
    }
    public GameObject CurrentVoid()
    {
        // returns the void the player gets sucked to (null if none)
        List<GameObject> fullyOnFields = GetFullyOnFields();
        foreach (GameObject field in fullyOnFields)
        {
            FieldManager.FieldType? currentFieldType = FieldManager.GetFieldType(field);
            if (currentFieldType != null && currentFieldType == FieldManager.FieldType.VOID)
            {
                return field;
            }
        }
        return null;
    }
    public bool IsOnVoid()
    {
        // we dont need that, its just there lol
        return IsFullyOnField(FieldManager.FieldType.VOID);
    }

    public GameObject GetCurrentField()
    {
        return FieldManager.GetField((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));
    }
    #endregion

    public void Win()
    {
        // animation and play mode and that's it really
        animator.SetTrigger("Death");
        AudioManager.Instance.Play("Win");
        GameManager.Instance.TogglePlay(false);
    }

    public void DieNormal(string soundEffect = "Smack")
    {
        // default dying
        // avoid dying while in animation
        if (!inDeathAnim)
        {
            // animation trigger and no movement
            DeathAnim();
            if (GameManager.Instance.Multiplayer) photonView.RPC("DeathAnim", RpcTarget.Others);
        }

        // avoid doing more if not own view in multiplayer
        if (GameManager.Instance.Multiplayer && !photonView.IsMine) return;

        if (GameManager.Instance.Playing)
        {
            // sfx and death counter
            AudioManager.Instance.Play(soundEffect);
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

        AudioManager.Instance.Play("Void");

        Die();
    }
    private void Die()
    {
        // general method when dying in any way

        rb.simulated = false;
        inDeathAnim = true;

        // avoid doing more if not own view in multiplayer
        if (GameManager.Instance.Multiplayer && !photonView.IsMine) return;

        if(GameManager.Instance.Playing) deaths++;

        // update coin counter
        coinsCollected.Clear();
        if (currentState != null)
        {
            foreach (Vector2 coinPos in currentState.collectedCoins)
            {
                GameObject coin = CoinManager.GetCoin(coinPos);
                if (coin != null) coinsCollected.Add(coin);
            }
        }
    }

    [PunRPC]
    public void DeathAnim()
    {
        animator.SetTrigger("Death");
    }

    public void ActivateCheckpoint(float mx, float my)
    {
        coinsCollected.RemoveAll(e => e == null);
        keysCollected.RemoveAll(e => e == null);

        // mx my coords of checkpointfield
        Vector2 statePlayerStartingPos = new(mx, my);

        // serialize game state
        // convert collectedCoins and collectedKeys to List<Vector2>
        List<Vector2> coinPositions = new();
        
        foreach(GameObject c in coinsCollected)
        {
            coinPositions.Add(c.GetComponent<CoinController>().coinPosition);
        }

        List<Vector2> keyPositions = new();
        foreach (GameObject k in keysCollected)
        {
            KeyController keyController = k.GetComponent<KeyController>();
            keyPositions.Add(keyController.keyPosition);
        }

        currentState = new(statePlayerStartingPos, coinPositions, keyPositions);
        
        print("Saved game state");
    }

    public bool CoinsCollected()
    {
        return coinsCollected.Count >= GameManager.Instance.CoinContainer.transform.childCount;
    }
    public void UncollectCoinAtPos(Vector2 pos)
    {
        for(int i = coinsCollected.Count - 1; i >= 0; i--)
        {
            GameObject c = coinsCollected[i];
            if (c.GetComponent<CoinController>().coinPosition == pos)
            {
                print("removedCoin");
                coinsCollected.Remove(c);
            }
        }
    }
    
    public bool KeysCollected(KeyManager.KeyColor color)
    {
        foreach(Transform k in GameManager.Instance.KeyContainer.transform)
        {
            KeyController controller = k.GetChild(0).GetComponent<KeyController>();
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
        rb.transform.position = startPos;
    }

    public void DestroyPlayer(bool removeTargetFromCamera = true)
    {
        JumpToEntity camera = Camera.main.GetComponent<JumpToEntity>();
        if (removeTargetFromCamera && camera.target == gameObject) camera.target = null;

        Destroy(sliderController.GetSliderObject());
        if (nameTagController != null) Destroy(nameTagController.nameTag);
        Destroy(gameObject);
    }

    public void DeathAnimFinish()
    {
        DestroyPlayer(false);

        if (GameManager.Instance.Multiplayer && !photonView.IsMine) return;

        inDeathAnim = false;

        float applySpeed = speed;
        int deaths = this.deaths;

        // create new player at start position
        Vector2 spawnPos = !GameManager.Instance.Playing || currentState == null ? startPos : currentState.playerStartPos;

        GameObject player = PlayerManager.InstantiatePlayer(spawnPos, applySpeed, GameManager.Instance.Multiplayer);
        PlayerController newController = player.GetComponent<PlayerController>();
        newController.deaths = deaths;
        newController.startPos = startPos;
        newController.currentState = currentState;

        JumpToEntity jumpToPlayer = Camera.main.GetComponent<JumpToEntity>();
        if (jumpToPlayer.target == gameObject) jumpToPlayer.target = player;

        // reset coins
        foreach (Transform coin in GameManager.Instance.CoinContainer.transform)
        {
            CoinController coinController = coin.GetChild(0).GetComponent<CoinController>();

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
        foreach (Transform key in GameManager.Instance.KeyContainer.transform)
        {
            KeyController keyController = key.GetChild(0).GetComponent<KeyController>();

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
