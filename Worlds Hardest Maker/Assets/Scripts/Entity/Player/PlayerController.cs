using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public Animator animator;
    public List<GameObject> currentFields;
    private Vector2 movementInput;
    private bool inDeathAnim = false;
    private AppendSlider sliderController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sliderController = GetComponent<AppendSlider>();

        // make slider follow player
        GameObject sliderObject = sliderController.GetSliderObject();
        sliderObject.GetComponent<UIFollowEntity>().entity = gameObject;

        // update speed every time changed
        Slider slider = sliderController.GetSlider();
        slider.onValueChanged.AddListener((value) =>
        {
            speed = sliderController.GetValue();

            UpdateSpeedText();
        });

        UIFollowEntity follow = sliderController.GetSliderObject().GetComponent<UIFollowEntity>();
        follow.entity = gameObject;
        follow.offset = new(0, 0.5f);

        UpdateSpeedText();
    }

    private void Update()
    {
        movementInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        // movement
        if (GameManager.Instance.Playing)
        {
            rb.MovePosition((Vector2) rb.transform.position + speed * Time.fixedDeltaTime * movementInput.normalized);
        }
    }

    /// <summary>
    /// always use SetSpeed instead of setting
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;

        Slider slider = sliderController.GetSlider();
        slider.value = newSpeed / sliderController.step;
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
        GameManager.TogglePlay();
    }

    public void Die()
    {
        // avoid dying while in animation
        if (!inDeathAnim)
        {
            // animation trigger and no movement
            animator.SetTrigger("Death");
            rb.simulated = false;
            inDeathAnim = true;
        }
        if (GameManager.Instance.Playing)
        {
            // sfx and death counter
            AudioManager.Instance.Play("Smack");
            GameManager.Instance.PlayerDeaths++;
        }

        // update coin counter
        GameManager.Instance.CollectedCoins = 0;
        if (GameManager.Instance.CurrentState != null)
        {
            GameManager.Instance.CollectedCoins = GameManager.Instance.CurrentState.collectedCoins.Count;
        }
    }

    public void ActivateCheckpoint(int mx, int my)
    {
        // mx my coords of checkpointfield
        Vector2 statePlayerStartingPos = new(mx, my);
        List<Vector2> coinsCollected = new();
        List<Vector2> keysCollected = new();

        // serialize game state

        foreach (Transform coin in GameManager.Instance.CoinContainer.transform)
        {
            CoinController controller = coin.GetChild(0).GetComponent<CoinController>();
            if (controller.pickedUp)
            {
                coinsCollected.Add(coin.position);
            }
        }

        foreach (Transform key in GameManager.Instance.KeyContainer.transform)
        {
            KeyController controller = key.GetChild(0).GetComponent<KeyController>();
            if (controller.pickedUp)
            {
                keysCollected.Add(key.position);
            }
        }

        GameState state = new(statePlayerStartingPos, coinsCollected, keysCollected);
        GameManager.Instance.CurrentState = state;

        print("Saved game state");
    }

    public bool CoinsCollected()
    {
        foreach(Transform coin in GameManager.Instance.CoinContainer.transform)
        {
            CoinController controller = coin.GetChild(0).GetComponent<CoinController>();
            if (!controller.pickedUp)
            {
                return false;
            }
        }
        return true;
    }
    public bool KeysCollected(FieldManager.KeyDoorColor color)
    {
        foreach (Transform key in GameManager.Instance.KeyContainer.transform)
        {
            KeyController controller = key.GetChild(0).GetComponent<KeyController>();
            if (!controller.pickedUp && controller.color == color)
            {
                return false;
            }
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
        rb.transform.position = (Vector2) GameManager.Instance.PlayerStartPos;
    }

    public void DeathAnimFinish()
    {
        GameState state = GameManager.Instance.CurrentState;

        inDeathAnim = false;

        float applySpeed = speed;

        Destroy(sliderController.GetSliderObject());
        Destroy(gameObject);

        // create new player at start position
        GameObject player = GameManager.Instance.Player;
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.speed = speed;

        Vector2 spawnPos = !GameManager.Instance.Playing || state == null ? (Vector2) GameManager.Instance.PlayerStartPos : state.playerStartPos;

        GameObject newPlayer = Instantiate(player, spawnPos, Quaternion.identity, GameManager.Instance.PlayerContainer.transform);
        PlayerController controller = newPlayer.GetComponent<PlayerController>();
        controller.SetSpeed(applySpeed);

        // reset coins
        foreach (Transform coin in GameManager.Instance.CoinContainer.transform)
        {
            bool respawns = true;
            if(state != null) { 
                foreach (Vector2 collected in state.collectedCoins)
                {
                    if (collected.x == coin.position.x && collected.y == coin.position.y)
                    {
                        // if coin is collected or no state exists it doesnt respawn
                        respawns = false;
                        break;
                    }
                }
            }
            
            if(respawns)
            {
                CoinController coinController = coin.GetChild(0).GetComponent<CoinController>();
                coinController.pickedUp = false;

                Animator anim = coin.GetComponent<Animator>();
                anim.SetBool("PickedUp", false);
            }
        }

        // reset keys
        foreach (Transform key in GameManager.Instance.KeyContainer.transform)
        {
            bool respawns = true;
            if (state != null)
            {
                foreach (Vector2 collected in state.collectedKeys)
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
                KeyController keyController = key.GetChild(0).GetComponent<KeyController>();
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
