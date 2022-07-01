using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// manages game (duh)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum EditMode
    {
        DELETE_FIELD, WALL_FIELD, START_FIELD, GOAL_FIELD, START_AND_GOAL_FIELD, CHECKPOINT_FIELD, ONE_WAY_FIELD, GRAY_KEY_DOOR_FIELD, RED_KEY_DOOR_FIELD, GREEN_KEY_DOOR_FIELD, BLUE_KEY_DOOR_FIELD, YELLOW_KEY_DOOR_FIELD, PLAYER, BALL_DEFAULT, BALL_CIRCLE, COIN, GRAY_KEY, RED_KEY, GREEN_KEY, BLUE_KEY, YELLOW_KEY
    }
    [Header("Constants & References")]
    public GameObject WallField;
    public GameObject StartField;
    public GameObject GoalField;
    public GameObject StartAndGoalField;
    public GameObject CheckpointField;
    public GameObject OneWayField;
    public GameObject GrayKeyDoorField;
    public GameObject RedKeyDoorField;
    public GameObject GreenKeyDoorField;
    public GameObject BlueKeyDoorField;
    public GameObject YellowKeyDoorField;
    public GameObject Player;
    public GameObject BallDefault;
    public GameObject BallCircle;
    public GameObject Coin;
    public GameObject GrayKey;
    public GameObject RedKey;
    public GameObject GreenKey;
    public GameObject BlueKey;
    public GameObject YellowKey;
    public GameObject FillPreview;
    [Space]
    public GameObject Manager;
    public GameObject Canvas;
    public GameObject Menu;
    public GameObject PlacementPreview;
    public GameObject SliderContainer;
    public GameObject DrawContainer;
    public GameObject FillOutlineContainer;
    public GameObject FillPreviewContainer;
    public GameObject PlayerContainer;
    public GameObject BallDefaultContainer;
    public GameObject BallCircleContainer;
    public GameObject CoinContainer;
    public GameObject KeyContainer;
    public GameObject FieldContainer;
    [Space]
    public KeyCode EntityDeleteKey;
    public KeyCode BallDragKey;
    public KeyCode EditSpeedKey;
    [Space]
    public Material LineMaterial;
    [Space]
    public Text EditModeText;
    public Text FillingText;
    public Text DeathText;
    public Text CoinText;

    // global variables
    [Header("Variables")]
    public bool Playing = false;
    public EditMode CurrentEditMode = EditMode.WALL_FIELD;
    public bool Filling = false;
    [HideInInspector] public Vector2? MouseDragStart = null;
    [HideInInspector] public Vector2? MouseDragEnd = null;
    [HideInInspector] public Vector2 PrevMousePos;
    [HideInInspector] public Vector2 MousePosWorldSpace;
    [HideInInspector] public Vector2 MousePosWorldSpaceRounded = new();
    [HideInInspector] public List<Vector2> CurrentFillRange = null;
    [HideInInspector] public bool UIHovered = false;
    [HideInInspector] public GameState CurrentState = null;
    [HideInInspector] public Vector2? PlayerStartPos = null;
    [HideInInspector] public int PlayerDeaths = 0;
    [HideInInspector] public int CollectedCoins = 0;
    [HideInInspector] public int TotalCoins = 0;

    private void Start()
    {
        PlayerManager.SetPlayer(0, 0);

#if UNITY_EDITOR
        //Menu.SetActive(true);
        //Menu.SetActive(false);
#endif
    }

    private void Update()
    {
        MousePosWorldSpace = GetMouseWorldPos();
        MousePosWorldSpaceRounded.x = Mathf.Round(MousePosWorldSpace.x);
        MousePosWorldSpaceRounded.y = Mathf.Round(MousePosWorldSpace.y);
    }

    private void LateUpdate()
    {
        // set edit mode text ui
        Instance.EditModeText.text = $"Edit: {EditModeUI(Instance.CurrentEditMode)}";
        Instance.FillingText.text = $"Filling: {Filling}";
        Instance.DeathText.text = $"Deaths: {Instance.PlayerDeaths}";
        Instance.CoinText.text = $"Coins: {Instance.CollectedCoins}/{Instance.TotalCoins}";

        // set previous mouse pos
        Instance.PrevMousePos = Input.mousePosition;
    }


    // functions
    public static Vector2 GetMouseWorldPos()
    {
        Vector2 mousePos = Input.mousePosition;

        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // convert stuff
    public static float PixelToUnit(float pixel)
    {
        Camera cam = Camera.main;
        return pixel * 2 * cam.orthographicSize / cam.pixelHeight;
    }
    public static float UnitToPixel(float unit)
    {
        Camera cam = Camera.main;
        return unit * cam.pixelHeight / (cam.orthographicSize * 2);
    }
    public static Rect RtToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new((Vector2)transform.position - (size * 0.5f), size);
    }

    /// <summary>
    /// list of every ui string which gets displayed based on edit mode
    /// </summary>
    /// <param name="mode">mode which gets converted into string</param>
    /// <returns>string displayed in edit mode UI text element</returns>
    public static string EditModeUI(EditMode mode)
    {
        string[] ui = {
            "Delete",
            "Wall Field",
            "Start Field",
            "Goal Field",
            "Start/Goal Field",
            "Checkpoint Field",
            "One Way Gate",
            "Key Door Field - Gray",
            "Key Door Field - Red",
            "Key Door Field - Green",
            "Key Door Field - Blue",
            "Key Door Field - Yellow",
            "Player",
            "Ball",
            "Ball - Circle",
            "Coin",
            "Key - Gray",
            "Key - Red",
            "Key - Green",
            "Key - Blue",
            "Key - Yellow",
        };
        return ui[(int)mode];
    }

    // play / edit mode stuff
    public static void TogglePlay()
    {
        if (Instance.Playing) SwitchToEdit();
        else SwitchToPlay();

        Animator canvasAnim = Instance.Canvas.GetComponent<Animator>();
        canvasAnim.SetBool("Playing", Instance.Playing);
    }
    public static void SwitchToPlay()
    {
        Instance.Playing = true;
        Instance.CurrentState = null;
        Instance.PlayerDeaths = 0;
        Instance.TotalCoins = Instance.CoinContainer.transform.childCount;
        Instance.CollectedCoins = 0;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(false);

        // disable placement preview
        Instance.PlacementPreview.SetActive(false);

        // set target of ball defaults
        foreach (Transform ball in Instance.BallDefaultContainer.transform)
        {
            BallController controller = ball.GetChild(0).GetComponent<BallController>();
            controller.ResetTarget();
        }

        // activate coin animations
        foreach (Transform coin in Instance.CoinContainer.transform)
        {
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", true);
            anim.SetBool("PickedUp", false);
        }

        // activate key animations
        foreach (Transform key in Instance.KeyContainer.transform)
        {
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", true);
            anim.SetBool("PickedUp", false);
        }
    }
    public static void SwitchToEdit()
    {
        Instance.Playing = false;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(true);

        ResetGame();

        // enable placement preview and place it at mouse
        Instance.PlacementPreview.SetActive(true);
        Instance.PlacementPreview.transform.position = Instance.MousePosWorldSpaceRounded;

        // set target of balls
        foreach (Transform ball in Instance.BallDefaultContainer.transform)
        {
            ball.GetChild(0).GetComponent<BallController>().ResetTarget();
        }

        // deactivate coin animations
        foreach (Transform coin in Instance.CoinContainer.transform)
        {
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // deactivate key animations
        foreach (Transform key in Instance.KeyContainer.transform)
        {
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        Instance.CurrentState = null;
    }

    /// <summary>
    /// resets every field and entity to its starting state
    /// used when switched to edit mode
    /// </summary>
    public static void ResetGame()
    {
        // reset player
        GameObject player = PlayerManager.GetCurrentPlayer();
        if (player != null)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.Die();
        }

        // reset balls
        foreach (Transform ball in Instance.BallDefaultContainer.transform)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallController controller = ballObject.GetComponent<BallController>();

            ballObject.transform.position = controller.startPosition;
        }
        foreach (Transform ball in Instance.BallCircleContainer.transform)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallCircleController controller = ballObject.GetComponent<BallCircleController>();

            controller.currentAngle = controller.startAngle;
            controller.UpdateAnglePos();
        }

        // reset coins
        foreach (Transform coin in Instance.CoinContainer.transform)
        {
            CoinController controller = coin.GetChild(0).GetComponent<CoinController>();
            controller.pickedUp = false;

            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // reset keys
        foreach (Transform key in Instance.KeyContainer.transform)
        {
            KeyController controller = key.GetChild(0).GetComponent<KeyController>();
            controller.pickedUp = false;

            Animator anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // reset checkpoints
        foreach (Transform field in Instance.FieldContainer.transform)
        {
            if (field.CompareTag("CheckpointField"))
            {
                Animator anim = field.GetComponent<Animator>();
                anim.SetBool("Active", false);

                CheckpointController controller = field.GetComponent<CheckpointController>();
                controller.activated = false;
            }
        }

        // reset key doors
        string[] tags = { "KeyDoorField", "RedKeyDoorField", "GreenKeyDoorField", "BlueKeyDoorField", "YellowKeyDoorField" };
        foreach (string tag in tags)
        {
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tag))
            {
                KeyDoorField comp = door.GetComponent<KeyDoorField>();
                comp.Lock(true);
            }
        }
    }

    public static void ClearLevel()
    {
        PlayerManager.RemoveAllPlayers();
        GameObject[] containers = { Instance.FieldContainer, Instance.PlayerContainer, Instance.BallDefaultContainer, Instance.CoinContainer, Instance.KeyContainer };
        foreach (GameObject container in containers)
        {
            for (int i = container.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(container.transform.GetChild(i).gameObject);
            }
        }
    }

    public static void LoadLevel()
    {
        List<IData> levelData = SaveSystem.LoadLevel();

        if (levelData != null)
        {
            ClearLevel();
            List<FieldData> fieldDatas = new();
            PlayerData playerData = null;

            // load ball, coins
            foreach (IData levelObject in levelData)
            {
                if (levelObject.GetType() == typeof(FieldData))
                {
                    fieldDatas.Add((FieldData)levelObject);
                    continue;
                }
                else if (levelObject.GetType() == typeof(PlayerData))
                {
                    playerData = (PlayerData)levelObject;
                    continue;
                }
                levelObject.CreateObject();
            }


            // load fields
            foreach (FieldData field in fieldDatas)
            {
                field.CreateObject();
            }

            // load player last
            if (playerData != null)
            {
                playerData.CreateObject();
            }
        }
    }

    public static void RemoveObjectInContainer(int mx, int my, GameObject container)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.25f);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.transform.parent == container.transform)
            {
                Destroy(hit.gameObject);
            }
            else if(hit.gameObject.transform.parent.parent == container.transform)
            {
                Destroy(hit.gameObject.transform.parent.gameObject);
            }
        }
    }

    public static void SetEditMode(EditMode mode)
    {
        GameObject[] tools = ToolbarManager.tools;
        foreach (GameObject tool in tools)
        {
            Tool t = tool.GetComponent<Tool>();
            if (t.toolName == mode)
            {
                t.SwitchGameMode();
            }
        }
    }

    public static void DisableAllOptionbars()
    {
        GameObject[] optionbars = GameObject.FindGameObjectsWithTag("ToolOptionbar");
        foreach (GameObject ob in optionbars)
        {
            ob.transform.localPosition *= 100;
        }
    }
    public static void EnableAllOptionbars()
    {
        GameObject[] optionbars = GameObject.FindGameObjectsWithTag("ToolOptionbar");
        foreach (GameObject ob in optionbars)
        {
            ob.transform.localPosition /= 100;
        }
    }

    public static Dictionary<EditMode, GameObject> GetPrefabs(){
        Dictionary<EditMode, GameObject> prefabs = new()
        {
            { EditMode.WALL_FIELD, Instance.WallField },
            { EditMode.START_FIELD, Instance.StartField },
            { EditMode.GOAL_FIELD, Instance.GoalField },
            { EditMode.START_AND_GOAL_FIELD, Instance.StartAndGoalField },
            { EditMode.CHECKPOINT_FIELD, Instance.CheckpointField },
            { EditMode.ONE_WAY_FIELD, Instance.OneWayField },
            { EditMode.GRAY_KEY_DOOR_FIELD, Instance.GrayKeyDoorField },
            { EditMode.RED_KEY_DOOR_FIELD, Instance.RedKeyDoorField },
            { EditMode.GREEN_KEY_DOOR_FIELD, Instance.GreenKeyDoorField },
            { EditMode.BLUE_KEY_DOOR_FIELD, Instance.BlueKeyDoorField },
            { EditMode.YELLOW_KEY_DOOR_FIELD, Instance.YellowKeyDoorField },
            { EditMode.PLAYER, Instance.Player },
            { EditMode.BALL_DEFAULT, Instance.BallDefault },
            { EditMode.BALL_CIRCLE, Instance.BallCircle },
            { EditMode.COIN, Instance.Coin },
            { EditMode.GRAY_KEY, Instance.GrayKey },
            { EditMode.RED_KEY, Instance.RedKey },
            { EditMode.GREEN_KEY, Instance.GreenKey },
            { EditMode.BLUE_KEY, Instance.BlueKey },
            { EditMode.YELLOW_KEY, Instance.YellowKey }
        };
        return prefabs;
    }

    // init singleton
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else
        {
            Destroy(gameObject);
        }
    }
}