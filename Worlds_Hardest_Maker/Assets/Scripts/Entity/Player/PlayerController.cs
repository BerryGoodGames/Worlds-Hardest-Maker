using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using DG.Tweening;

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

    [HideInInspector] public bool inDeathAnim = false;

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
    }

    private void Update()
    {
        movementInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (GameManager.Instance.Multiplayer) photonView.RPC("SetNameTagActive", RpcTarget.All, GameManager.Instance.Playing);
    }

    private void FixedUpdate()
    {
        // check water and update drown level
        bool water = IsOnWater();

        if (water && !inDeathAnim)
        {
            currentDrownDuration += Time.fixedDeltaTime;

            if (currentDrownDuration >= drownDuration)
            {
                Die();
            }
        }
        else if(!inDeathAnim && !water) currentDrownDuration = 0;

        float drown = currentDrownDuration / drownDuration;
        waterLevel.localScale = new(waterLevel.localScale.x, drown);


        bool ice = IsOnIce();

        // movement (if player is yours in multiplayer mode)
        if (GameManager.Instance.Playing)
        {
            if (GameManager.Instance.Multiplayer && !photonView.IsMine) return;

            if (ice) 
            { 
                // acceleration on ice
                rb.drag = iceFriction;
                rb.AddForce(iceFriction * speed * 1.2f * movementInput, ForceMode2D.Force);

                rb.velocity = new(Mathf.Min(maxIceSpeed, rb.velocity.x), Mathf.Min(maxIceSpeed, rb.velocity.y));
            }
            else
            {
                // snappy movement (when not on ice)
                rb.MovePosition((Vector2)rb.transform.position + (water ? waterDamping * speed : speed) * Time.fixedDeltaTime * movementInput);
            }

            // check void death
            GameObject currentVoid = CurrentVoid();
            if(!inDeathAnim && currentVoid != null)
            {
                // get sucked to void
                Vector2 suckPosition = currentVoid.transform.position;

                spriteRenderer.material.DOFade(0, voidSuckDuration)
                    .SetEase(Ease.Linear);
                transform.DOMove(suckPosition, voidSuckDuration)
                    .SetEase(Ease.OutQuint);
                transform.DOScale(Vector2.zero, voidSuckDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(DeathAnimFinish);

                inDeathAnim = true;
            }
        }
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.001f);
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

    public void Win()
    {
        // animation and play mode and that's it really
        animator.SetTrigger("Death");
        GameManager.Instance.TogglePlay();
    }

    public void Die()
    {
        // avoid dying while in animation
        if (!inDeathAnim)
        {
            // animation trigger and no movement
            DeathAnim();
            if (GameManager.Instance.Multiplayer) photonView.RPC("DeathAnim", RpcTarget.Others);
            rb.simulated = false;
            inDeathAnim = true;
        }

        // avoid doing more if not own view in multiplayer
        if (GameManager.Instance.Multiplayer && !photonView.IsMine) return;

        if (GameManager.Instance.Playing)
        {
            // sfx and death counter
            AudioManager.Instance.Play("Smack");
            deaths++;
        }

        // update coin counter
        coinsCollected.Clear();
        if (currentState != null)
        {
            foreach(Vector2 coinPos in currentState.collectedCoins)
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
        // mx my coords of checkpointfield
        Vector2 statePlayerStartingPos = new(mx, my);

        // serialize game state
        // convert collectedCoins and collectedKeys to List<Vector2>
        List<Vector2> coinPositions = new();
        foreach(GameObject c in coinsCollected)
        {
            CoinController coinController = c.GetComponent<CoinController>();
            coinPositions.Add(coinController.coinPosition);
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
}
