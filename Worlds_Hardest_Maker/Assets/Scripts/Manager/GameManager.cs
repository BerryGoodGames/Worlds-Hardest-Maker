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

    #region Variables
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

            if (prevEditMode != null && prevEditMode != currentEditMode) OnEditModeChange();

            prevEditMode = currentEditMode;

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
                ReferenceManager.Instance.BallWindows.gameObject.SetActive(true);
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
                ReferenceManager.Instance.BallWindows.SetActive(false);
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
    private EditMode? prevEditMode = null;
    public bool Multiplayer { get; set; } = false;
    [HideInInspector] public bool UIHovered { get; set; } = false;
    private int editRotation = 270;
    [HideInInspector] public int EditRotation
    {
        get => editRotation;
        set
        {
            editRotation = value;

            ReferenceManager.Instance.PlacementPreview.GetComponent<PreviewController>().UpdateRotation();
        }
    }
    #endregion

    #region Events
    public event Action OnGameQuit;
    public event Action OnPlay;
    public event Action OnEdit;
    public event Action OnEditModeChange;
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
        ReferenceManager.Instance.Menu.GetComponent<MenuManager>().SetMusicVolume(0.0001f);

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

        SetCameraUnitWidth(23);

        OnEdit += TextManager.Instance.StopTimer;
        OnPlay += TextManager.Instance.StartTimer;
    }

    private void Update()
    {
        // check if toolbar background is hovered
        Instance.UIHovered = EventSystem.current.IsPointerOverGameObject();
    }

    private void LateUpdate()
    {
        
    }

    private void OnIsMultiplayer()
    {
        // enable photon player spawning
        FindObjectOfType<PlayerSpawner>().enabled = true;
    }

    #region Unit pixel conversion methods
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

    #region Play / Edit mode methods
    public void TogglePlay(bool playSoundEffect = true)
    {
        if (!ReferenceManager.Instance.Menu.activeSelf)
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
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            if (Instance.Multiplayer && !controller.photonView.IsMine) continue;

            controller.currentFields.Clear();
            controller.currentState = null;
            controller.deaths = 0;
        }

        Instance.Playing = true;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(false);

        // disable placement preview
        ReferenceManager.Instance.PlacementPreview.SetActive(false);

        // disable windows
        ReferenceManager.Instance.BallWindows.SetActive(false);

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
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer)
        {
            anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", true);
            anim.SetBool("PickedUp", coin.GetChild(0).GetComponent<CoinController>().pickedUp);
        }

        // activate key animations
        foreach (Transform key in ReferenceManager.Instance.KeyContainer)
        {
            anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", true);
            anim.SetBool("PickedUp", key.GetChild(0).GetComponent<KeyController>().pickedUp);
        }

        // camera jumps to last player if its not on screen
        Camera.main.GetComponent<JumpToEntity>().Jump(true);
        Instance.OnPlay?.Invoke();

        // close level settings panel if open
        LevelSettingsPanelTween lspt = ReferenceManager.Instance.LevelSettingsPanel.GetComponent<LevelSettingsPanelTween>();
        if (lspt.open) lspt.Toggle();
    }
    public static void SwitchToEdit(bool playSoundEffect = true)
    {
        Instance.Playing = false;

        if(playSoundEffect) AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(true);

        ResetGame();

        // enable placement preview and place it at mouse
        ReferenceManager.Instance.PlacementPreview.SetActive(true);
        ReferenceManager.Instance.PlacementPreview.transform.position = FollowMouse.GetCurrentMouseWorldPos(ReferenceManager.Instance.PlacementPreview.GetComponent<FollowMouse>().worldPosition);

        // enable windows
        if(Instance.CurrentEditMode == EditMode.ANCHOR || Instance.CurrentEditMode == EditMode.BALL)
            ReferenceManager.Instance.BallWindows.SetActive(true);

        
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
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer.transform)
        {
            coin.GetChild(0).GetComponent<CoinController>().pickedUp = false;

            anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // deactivate key animations
        foreach (Transform key in ReferenceManager.Instance.KeyContainer.transform)
        {
            key.GetChild(0).GetComponent<KeyController>().pickedUp = false;

            anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // remove game states from players
        //foreach (KeyValuePair<int, PlayerController> pair in Instance.PlayerControllerList)
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            controller.currentState = null;
        }
        Instance.OnEdit?.Invoke();
    }

    /// <summary>
    /// Sets edit mode at position
    /// </summary>
    /// <param name="editMode">the type of field/entity you want</param>
    /// <param name="pos">the position where it will be set</param>
    public static void Set(EditMode editMode, Vector2 pos)
    {
        int matrixX = (int)Mathf.Round(pos.x);
        int matrixY = (int)Mathf.Round(pos.y);

        bool multiplayer = Instance.Multiplayer;
        PhotonView pview = Instance.photonView;

        float gridX = Mathf.Round(pos.x * 2) * 0.5f;
        float gridY = Mathf.Round(pos.y * 2) * 0.5f;

        if (editMode.IsFieldType())
            FieldManager.Instance.SetField((int)pos.x, (int)pos.y, ConvertEnum<EditMode, FieldType>(editMode));
        else if (editMode == EditMode.DELETE_FIELD)
        {
            // delete field
            if (multiplayer) pview.RPC("RemoveField", RpcTarget.All, matrixX, matrixY, true);
            else FieldManager.Instance.RemoveField(matrixX, matrixY, updateOutlines: true);

            // remove player if at deleted pos
            if (multiplayer) pview.RPC("RemovePlayerAtPosIntersect", RpcTarget.All, (float)matrixX, (float)matrixY);
            else PlayerManager.Instance.RemovePlayerAtPosIntersect(matrixX, matrixY);
        }
        else if (editMode == EditMode.PLAYER)
        {
            // place player
            PlayerManager.Instance.SetPlayer(gridX, gridY);
        }
        else if (editMode == EditMode.COIN)
        {
            // place coin
            if (multiplayer) pview.RPC("SetCoin", RpcTarget.All, gridX, gridY);
            else CoinManager.Instance.SetCoin(gridX, gridY);
        }
        else if (KeyManager.IsKeyEditMode(editMode))
        {
            // get keycolor
            string keyColorStr = editMode.ToString()[..^4];
            KeyManager.KeyColor keyColor = (KeyManager.KeyColor)Enum.Parse(typeof(KeyManager.KeyColor), keyColorStr);

            // place key
            if (multiplayer) pview.RPC("SetKey", RpcTarget.All, gridX, gridY, keyColor);
            else KeyManager.Instance.SetKey(gridX, gridY, keyColor);
        }
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
        foreach (Transform ball in ReferenceManager.Instance.BallDefaultContainer)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallController controller = ballObject.GetComponent<BallController>();

            ballObject.transform.position = controller.startPosition;
        }
        foreach (Transform ball in ReferenceManager.Instance.BallCircleContainer)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallCircleController controller = ballObject.GetComponent<BallCircleController>();

            controller.currentAngle = controller.startAngle;
            controller.UpdateAnglePos();
        }

        // reset coins
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer)
        {
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", false);
            anim.SetBool("PickedUp", false);
        }

        // reset keys
        foreach (Transform key in ReferenceManager.Instance.KeyContainer)
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
        foreach (Transform field in ReferenceManager.Instance.FieldContainer)
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

    #region Save system
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

    public static void SetCameraUnitWidth(float width)
    {
        Camera cam = Camera.main;
        cam.orthographicSize = width * 0.5f / cam.aspect;
    }
    public static void SetCamerUnitHeight(float height)
    {
        Camera cam = Camera.main;
        cam.orthographicSize = height * 0.5f;
    }


    [PunRPC]
    public void ClearLevel()
    {
        PlayerManager.Instance.RemoveAllPlayers();
        Transform[] containers = { ReferenceManager.Instance.FieldContainer, ReferenceManager.Instance.PlayerContainer, ReferenceManager.Instance.BallDefaultContainer, ReferenceManager.Instance.BallCircleContainer, ReferenceManager.Instance.CoinContainer, ReferenceManager.Instance.KeyContainer, ReferenceManager.Instance.AnchorContainer };
        foreach (Transform container in containers)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(container.GetChild(i).gameObject);
            }
        }
    }

    public static bool PointOnScreen(Vector2 point, bool worldPoint)
    {
        Vector3 screenPoint = worldPoint ? Camera.main.WorldToViewportPoint(point) : point;
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }

    public static void RemoveObjectInContainer(float mx, float my, Transform container)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.005f, 128);
        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent == container)
            {
                Destroy(hit.gameObject);
            }
            else if(hit.transform.parent.parent == container)
            {
                Destroy(hit.transform.parent.gameObject);
            }
        }
    }
    public static void RemoveObjectInContainerIntersect(float mx, float my, Transform container)
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
        Enum.TryParse(typeof(EnumTo), e.ToString(), out object convEnum);

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
        if(Instance.OnGameQuit != null)
            Instance.OnGameQuit();

        Application.Quit();
    }

    
}