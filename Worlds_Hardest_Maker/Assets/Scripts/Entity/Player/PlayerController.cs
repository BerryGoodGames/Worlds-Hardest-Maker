using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback
{
    [HideInInspector] public int id;

    public float speed;

    [HideInInspector] public int deaths = 0;

    [HideInInspector] public List<GameObject> coinsCollected;
    [HideInInspector] public List<GameObject> keysCollected;

    [HideInInspector] public GameState currentState = null;

    [HideInInspector] public Vector2 startPos;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    private AppendSlider sliderController;
    private AppendNameTag nameTagController = null;

    [HideInInspector] public List<GameObject> currentFields;

    private Vector2 movementInput;

    [HideInInspector] public bool inDeathAnim = false;

    [HideInInspector] public PhotonView photonView;

    private void Awake()
    {
        coinsCollected = new();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sliderController = GetComponent<AppendSlider>();

        photonView = GetComponent<PhotonView>();

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
        // movement (if player is yours in multiplayer mode)
        if (GameManager.Instance.Playing)
        {
            if (GameManager.Instance.Multiplayer && !photonView.IsMine) return;

            rb.MovePosition((Vector2) rb.transform.position + speed * Time.fixedDeltaTime * movementInput);
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
        // sum up collected keys with color
        int collected = 0;
        foreach(GameObject k in keysCollected)
        {
            if (k.GetComponent<KeyController>().color == color) collected++;
        }

        // sum up total keys with color
        int total = 0;
        foreach (Transform k in GameManager.Instance.KeyContainer.transform)
        {
            if (k.GetChild(0).GetComponent<KeyController>().color == color) total++;
        }

        return total <= collected;
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
            bool respawns = true;
            if (currentState != null)
            {
                foreach (Vector2 collected in currentState.collectedKeys)
                {
                    if (collected.x == key.position.x && collected.y == key.position.y)
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

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if(transform.parent != GameManager.Instance.PlayerContainer.transform)
        {
            transform.SetParent(GameManager.Instance.PlayerContainer.transform);
        }
    }
}
