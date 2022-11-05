using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// manages game (duh)
/// </summary>
public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance { get; private set; }

    public enum EditMode
    {
        DELETE_FIELD, 
        WALL_FIELD, 
        START_FIELD, GOAL_FIELD, CHECKPOINT_FIELD, 
        ONE_WAY_FIELD, CONVEYOR,
        WATER, ICE,
        VOID,
        GRAY_KEY_DOOR_FIELD, RED_KEY_DOOR_FIELD, GREEN_KEY_DOOR_FIELD, BLUE_KEY_DOOR_FIELD, YELLOW_KEY_DOOR_FIELD, 
        PLAYER, 
        ANCHOR, 
        BALL, BALL_DEFAULT, BALL_CIRCLE, 
        COIN, 
        GRAY_KEY, RED_KEY, GREEN_KEY, BLUE_KEY, YELLOW_KEY
    }

    #region CONSTANTS & REFERENCES
    [Header("Constants & References")]
    [Header("Prefabs")]
    public GameObject WallField;
    public GameObject StartField;
    public GameObject GoalField;
    public GameObject CheckpointField;
    public GameObject OneWayField;
    public GameObject Conveyor;
    public GameObject Water;
    public GameObject Ice;
    public GameObject Void;
    public GameObject GrayKeyDoorField;
    public GameObject RedKeyDoorField;
    public GameObject GreenKeyDoorField;
    public GameObject BlueKeyDoorField;
    public GameObject YellowKeyDoorField;
    public GameObject Player;
    public GameObject Anchor;
    public GameObject Ball;
    public GameObject BallDefault;
    public GameObject BallCircle;
    public GameObject Coin;
    public GameObject GrayKey;
    public GameObject RedKey;
    public GameObject GreenKey;
    public GameObject BlueKey;
    public GameObject YellowKey;
    public GameObject FillPreview;
    public GameObject Tooltip;
    [Space]
    [Header("Objects")]
    public GameObject Manager;
    public GameObject Canvas;
    public GameObject TooltipCanvas;
    public GameObject Menu;
    public GameObject PlayButton;
    public GameObject PlacementPreview;
    [Header("Containers")]
    public GameObject ToolbarContainer;
    public GameObject BallWindows;
    public GameObject SliderContainer;
    public GameObject NameTagContainer;
    public GameObject DrawContainer;
    public GameObject FillOutlineContainer;
    public GameObject FillPreviewContainer;
    public GameObject PlayerContainer;
    public GameObject AnchorContainer;
    public GameObject BallDefaultContainer;
    public GameObject BallCircleContainer;
    public GameObject CoinContainer;
    public GameObject KeyContainer;
    public GameObject FieldContainer;
    [Space]
    [Header("Key binds")]
    public KeyCode FillKey;
    public KeyCode EntityDeleteKey;
    public KeyCode EntityMoveKey;
    public KeyCode BallCircleRadiusKey;
    public KeyCode BallCircleAngleKey;
    public KeyCode EditSpeedKey;
    [Space]
    [Header("Materials")]
    public Material LineMaterial;
    public PhysicsMaterial2D NoFriction;
    [Space]
    [Header("Text References")]
    public TMPro.TMP_Text EditModeText;
    public TMPro.TMP_Text FillingText;
    public TMPro.TMP_Text DeathText;
    public TMPro.TMP_Text CoinText;
    #endregion

    #region VARIABLES
    [Header("Variables")]

    [SerializeField] private bool playing = false;
    public bool Playing { get { return playing; } set { playing = value; } }

    [SerializeField] private EditMode currentEditMode = EditMode.WALL_FIELD;
    public EditMode CurrentEditMode
    {
        get { return currentEditMode; }
        set
        {
            currentEditMode = value;

            // update toolbar
            GameObject[] tools = ToolbarManager.tools;
            foreach (GameObject tool in tools)
            {
                Tool t = tool.GetComponent<Tool>();
                if (t.toolName == value)
                {
                    // avoid recursion
                    t.SwitchGameMode(false);
                }
            }

            // enable/disable outlines and window when switching to/away from anchors/balls
            if (currentEditMode == EditMode.ANCHOR || currentEditMode == EditMode.BALL)
            {
                // enable stuff
                BallWindows.SetActive(true);
                if (AnchorManager.Instance.SelectedAnchor != null)
                {
                    // enable lines
                    AnchorManager.Instance.selectedPathController.drawLines = true;
                    AnchorManager.Instance.selectedPathController.DrawLines();

                    // switch animation to editing
                    foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
                    {
                        Animator anim = anchor.GetComponentInChildren<Animator>();
                        anim.SetBool("Editing", true);
                    }
                }
            }
            else if (currentEditMode != EditMode.ANCHOR && currentEditMode != EditMode.BALL)
            {
                // disable stuff
                BallWindows.SetActive(false);
                if (AnchorManager.Instance.SelectedAnchor != null)
                {
                    // disable lines
                    AnchorManager.Instance.selectedPathController.drawLines = false;
                    AnchorManager.Instance.selectedPathController.ClearLines();

                    // switch animation to editing
                    foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
                    {
                        Animator anim = anchor.GetComponentInChildren<Animator>();
                        anim.SetBool("Editing", false);
                    }
                }
            }
        }
    }

    public bool Filling { get; set; } = false;
    public bool Multiplayer { get; set; } = false;
    public bool KonamiActive { get; set; } = false;
    [HideInInspector] public List<Vector2> CurrentFillRange { get; set; } = null;
    [HideInInspector] public bool UIHovered { get; set; } = false;
    [HideInInspector] public int TotalCoins { get; set; } = 0;
    private int editRotation = 270;
    [HideInInspector] public int EditRotation
    {
        get => editRotation;
        set
        {
            editRotation = value;

            PlacementPreview.GetComponent<PreviewController>().UpdateRotation();
        }
    }

    public event Action onGameQuit;
    #endregion

    #region EVENTS
    public static event Action onPlay;
    public static event Action onEdit;
    #endregion

    private void Awake()
    {
        // init singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        // check if multiplayer or not
        Instance.Multiplayer = PhotonNetwork.CurrentRoom != null;
    }

    private void Start()
    {
        Menu.GetComponent<MenuManager>().SetMusicVolume(0.0001f);

        if (Instance.Multiplayer)
        {
            OnIsMultiplayer();
        } else {
            PlayerManager.Instance.SetPlayer(0.5f, 0.5f, 3f);
        }

        LevelSettings.Instance.SetDrownDuration();
        LevelSettings.Instance.SetIceFriction();
        LevelSettings.Instance.SetIceMaxSpeed();
        LevelSettings.Instance.SetWaterDamping();

    }

    private void Update()
    {
        // check if toolbar background is hovered
        Instance.UIHovered = EventSystem.current.IsPointerOverGameObject();
    }

    private void LateUpdate()
    {
        object playerDeaths;
        object playerCoinsCollected;

        try
        {
            PlayerController currentPlayer = PlayerManager.GetPlayer().GetComponent<PlayerController>();
            playerDeaths = currentPlayer.deaths;
            playerCoinsCollected = currentPlayer.coinsCollected.Count;
        }
        catch (Exception)
        {
            // no player placed
            playerDeaths = "-";
            playerCoinsCollected = "-";
        }

        // set edit mode text ui
        Instance.EditModeText.text = $"Edit: {Instance.CurrentEditMode.GetUIString()}";
        Instance.FillingText.text = $"Filling: {Filling}";
        Instance.DeathText.text = $"Deaths: {playerDeaths}";
        Instance.CoinText.text = $"Coins: {playerCoinsCollected}/{Instance.TotalCoins}";
    }

    private void OnIsMultiplayer()
    {
        // enable photon player spawning
        FindObjectOfType<PlayerSpawner>().enabled = true;
    }

    #region UNIT PIXEL CONVERSION METHODS
    // convert stuff
    public static float PixelToUnit(float pixel)
    {
        Camera cam = Camera.main;
        return pixel * 2 * cam.orthographicSize / cam.pixelHeight;
    }
    public static float PixelToUnit(float pixel, float ortho)
    {
        Camera cam = Camera.main;
        return pixel * 2 * ortho / cam.pixelHeight;
    }
    public static Vector2 PixelToUnit(Vector2 pixel)
    {
        return new(PixelToUnit(pixel.x), PixelToUnit(pixel.y));
    }
    public static Vector2 PixelToUnit(Vector2 pixel, float ortho)
    {
        return new(PixelToUnit(pixel.x, ortho), PixelToUnit(pixel.y, ortho));
    }
    public static float UnitToPixel(float unit)
    {
        Camera cam = Camera.main;
        return unit * cam.pixelHeight / (cam.orthographicSize * 2);
    }
    public static Vector2 UnitToPixel(Vector2 unit)
    {
        return new(UnitToPixel(unit.x), UnitToPixel(unit.y));
    }
    public static Rect RtToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new((Vector2)transform.position - (size * 0.5f), size);
    }
    #endregion

    #region PLAY / EDIT MODE METHODS
    public void TogglePlay(bool playSoundEffect = true)
    {
        if (!Instance.Menu.activeSelf)
        {
            if (Instance.Playing) SwitchToEdit(playSoundEffect);
            else SwitchToPlay();

            BarTween[] barTweens = FindObjectsOfType<BarTween>();
            foreach(BarTween tween in barTweens)
            {
                tween.SetPlay(Instance.Playing);
            }
        }
    }
    public static void SwitchToPlay()
    {
        //foreach (KeyValuePair<int, PlayerController> pair in Instance.PlayerControllerList)
        foreach (Transform player in Instance.PlayerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            if (Instance.Multiplayer && !controller.photonView.IsMine) continue;

            controller.currentFields.Clear();
            controller.currentState = null;
            controller.deaths = 0;
        }

        Instance.Playing = true;
        Instance.TotalCoins = Instance.CoinContainer.transform.childCount;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(false);

        // disable placement preview
        Instance.PlacementPreview.SetActive(false);

        // disable windows
        Instance.BallWindows.SetActive(false);

        Animator anim;

        if (AnchorManager.Instance.SelectedAnchor != null)
        {

            // disable anchor lines
            AnchorManager.Instance.selectedPathController.drawLines = false;
            AnchorManager.Instance.selectedPathController.ClearLines();

            // disable all anchor sprites / outlines
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool("Playing", true);
            }

        }

        // activate coin animations
        foreach (Transform coin in Instance.CoinContainer.transform)
        {
            anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", true);
            anim.SetBool("PickedUp", coin.GetChild(0).GetComponent<CoinController>().pickedUp);
        }

        // activate key animations
        foreach (Transform key in Instance.KeyContainer.transform)
        {
            anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", true);
            anim.SetBool("PickedUp", key.GetChild(0).GetComponent<KeyController>().pickedUp);
        }

        // camera jumps to last player if its not on screen
        Camera.main.GetComponent<JumpToEntity>().Jump(true);
        if(onPlay != null)
            onPlay();
    }
    public static void SwitchToEdit(bool playSoundEffect = true)
    {
        Instance.Playing = false;

        if(playSoundEffect) AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(true);

        ResetGame();

        // enable placement preview and place it at mouse
        Instance.PlacementPreview.SetActive(true);
        Instance.PlacementPreview.transform.position = FollowMouse.GetCurrentMouseWorldPos(Instance.PlacementPreview.GetComponent<FollowMouse>().worldPosition);

        // enable windows
        if(Instance.CurrentEditMode == EditMode.ANCHOR || Instance.CurrentEditMode == EditMode.BALL)
            Instance.BallWindows.SetActive(true);

        
        if (AnchorManager.Instance.selectedPathController != null)
        {
            // WTF
        }

        Animator anim;

        if (AnchorManager.Instance.SelectedAnchor != null)
        {
            // enable anchor lines
            AnchorManager.Instance.selectedPathController.drawLines = true;
            AnchorManager.Instance.selectedPathController.DrawLines();
            
            // enable all anchor sprites / outlines
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool("Playing", false);
            }
        }

        // reset Anchors
        foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
        {
            anchor.transform.GetChild(0).GetComponent<PathController>().ResetState();
        }

        // deactivate coin animations
        foreach (Transform coin in Instance.CoinContainer.transform)
        {
            coin.GetChild(0).GetComponent<CoinController>().pickedUp = false;

            anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // deactivate key animations
        foreach (Transform key in Instance.KeyContainer.transform)
        {
            key.GetChild(0).GetComponent<KeyController>().pickedUp = false;

            anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // remove game states from players
        //foreach (KeyValuePair<int, PlayerController> pair in Instance.PlayerControllerList)
        foreach (Transform player in Instance.PlayerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            controller.currentState = null;
        }
        if (onEdit != null)
            onEdit();
    }

    /// <summary>
    /// resets every field and entity to its starting state
    /// used when switched to edit mode
    /// </summary>
    public static void ResetGame()
    {
        // reset players
        foreach (GameObject player in PlayerManager.GetPlayers())
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (Instance.Multiplayer && !controller.photonView.IsMine) continue;
            controller.DieNormal();
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
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // reset keys
        foreach (Transform key in Instance.KeyContainer.transform)
        {
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        foreach(GameObject player in PlayerManager.GetPlayers())
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.coinsCollected.Clear();
            controller.keysCollected.Clear();
        }

        // reset checkpoints
        foreach (Transform field in Instance.FieldContainer.transform)
        {
            if (field.CompareTag("CheckpointField"))
            {
                CheckpointTween anim = field.GetComponent<CheckpointTween>();
                anim.Deactivate();

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
    #endregion

    #region SAVE SYSTEM
    public void LoadLevel()
    {
        List<IData> levelData = SaveSystem.LoadLevel();

        if (levelData != null)
        {
            LoadLevelFromData(levelData.ToArray());
        }
    }
    [PunRPC]
    public void LoadLevelFromData(IData[] levelData)
    {
        ClearLevel();
        List<FieldData> fieldDatas = new();
        PlayerData playerData = null;
        LevelSettingsData levelSettingsData = null;

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
            else if (levelObject.GetType() == typeof(LevelSettingsData))
            {
                levelSettingsData = (LevelSettingsData)levelObject;
                continue;
            }
            levelObject.ImportToLevel();
        }


        // load fields
        foreach (FieldData field in fieldDatas)
        {
            field.ImportToLevel();
        }

        // load player last
        if (playerData != null) playerData.ImportToLevel();

        // load level settings
        if (levelSettingsData != null) levelSettingsData.ImportToLevel();
    }
    [PunRPC]
    public void ReceiveLevel(string content)
    {
        BinaryFormatter formatter = new();
        Stream s = GenerateStreamFromString(content);

        List<IData> data = formatter.Deserialize(s) as List<IData>;

        s.Close();

        LoadLevelFromData(data.ToArray());
    }
    private Stream GenerateStreamFromString(string s)
    {
        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
    #endregion


    [PunRPC]
    public void ClearLevel()
    {
        PlayerManager.Instance.RemoveAllPlayers();
        GameObject[] containers = { Instance.FieldContainer, Instance.PlayerContainer, Instance.BallDefaultContainer, Instance.BallCircleContainer, Instance.CoinContainer, Instance.KeyContainer, Instance.AnchorContainer };
        foreach (GameObject container in containers)
        {
            for (int i = container.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(container.transform.GetChild(i).gameObject);
            }
        }
    }

    public static void RemoveObjectInContainer(float mx, float my, GameObject container)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.005f, 128);
        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent == container.transform)
            {
                Destroy(hit.gameObject);
            }
            else if(hit.transform.parent.parent == container.transform)
            {
                Destroy(hit.transform.parent.gameObject);
            }
        }
    }
    public static void RemoveObjectInContainerIntersect(float mx, float my, GameObject container)
    {
        float[] dx = { -0.5f, 0, 0.5f, -0.5f, 0, 0.5f, -0.5f, 0, 0.5f };
        float[] dy = { -0.5f, -0.5f, -0.5f, 0, 0, 0, 0.5f, 0.5f, 0.5f };
        for (int i = 0; i < dx.Length; i++)
        {
            RemoveObjectInContainer(mx + dx[i], my + dy[i], container);
        }
    }

    public static EnumTo ConvertEnum<EnumFrom, EnumTo>(EnumFrom e)
    {
        return (EnumTo)Enum.Parse(typeof(EnumTo), e.ToString());
    }

    public static object TryConvertEnum<EnumFrom, EnumTo>(EnumFrom e)
    {
        object convEnum;

        Enum.TryParse(typeof(EnumTo), e.ToString(), out convEnum);

        return convEnum;
    }

    public static float RoundToNearestStep(float value, float step)
    {
        return Mathf.Round(value / step) * step;
    }

    public static double Map(double value, double start1, double stop1, double start2, double stop2)
    {
        double range1 = stop1 - start1;
        double range2 = stop2 - start2;

        return range2 / range1 * (value - start1) + start2;
    }

    public static void QuitGame()
    {
        if(Instance.onGameQuit != null)
            Instance.onGameQuit();

        Application.Quit();
    }
}