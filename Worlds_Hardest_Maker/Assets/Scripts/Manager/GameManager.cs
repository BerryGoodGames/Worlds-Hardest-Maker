using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     manages game (duh)
/// </summary>
public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance { get; private set; }

    #region Variables

    private static readonly int playingString = Animator.StringToHash("Playing");
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");
    
    private bool cheated;
    public bool Cheated
    {
        get => cheated;
        set
        {
            cheated = value;
            TextManager.Instance.timer.color = cheated ? TextManager.Instance.cheatedTimerColor : Color.black;
        }
    }

    #endregion

    #region Events

    public event Action OnGameQuit;

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

        ForceDecimalSeparator(".");

        // check if multiplayer or not
        MultiplayerManager.Instance.Multiplayer = PhotonNetwork.CurrentRoom != null;

        SetCameraUnitWidth(23);
    }

    private void Start()
    {
        SettingsManager.Instance.SetMusicVolume(0.0001f);

        SettingsManager.Instance.LoadPrefs();

        if (MultiplayerManager.Instance.Multiplayer)
        {
            OnIsMultiplayer();
        }
        else
        {
            PlayerManager.Instance.SetPlayer(0, 0, 3f);
        }

        LevelSettings.Instance.SetDrownDuration();
        LevelSettings.Instance.SetIceFriction();
        LevelSettings.Instance.SetIceMaxSpeed();
        LevelSettings.Instance.SetWaterDamping();

        EditModeManager.Instance.OnEdit += () => PlayManager.Instance.Cheated = false;
    }

    private void Update()
    {
        // check if toolbarContainer background is hovered
        UIManager.Instance.UIHovered = EventSystem.current.IsPointerOverGameObject();
    }

    private static void OnIsMultiplayer()
    {
        // enable photon player spawning
        ReferenceManager.Instance.playerSpawner.enabled = true;
    }


    #region Unit pixel conversion methods

    // convert stuff
    public static float PixelToUnit(float pixel)
    {
        Camera cam = Camera.main;
        if (cam != null) return pixel * 2 * cam.orthographicSize / cam.pixelHeight;
        throw new Exception($"Couldn't convert {pixel} pixels to units because main camera is null");
    }

    public static float PixelToUnit(float pixel, float ortho)
    {
        Camera cam = Camera.main;
        if (cam != null) return pixel * 2 * ortho / cam.pixelHeight;
        throw new Exception($"Couldn't convert {pixel} pixels to units because main camera is null");
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
        if (cam != null) return unit * cam.pixelHeight / (cam.orthographicSize * 2);
        throw new Exception($"Couldn't convert {unit} units to pixels because main camera is null");
    }

    public static Vector2 UnitToPixel(Vector2 unit)
    {
        return new(UnitToPixel(unit.x), UnitToPixel(unit.y));
    }

    public static Rect RtToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new((Vector2)transform.position - size * 0.5f, size);
    }

    #endregion

    #region Play / Edit mode methods
    

    public void TogglePlay(bool playSoundEffect = true)
    {
        if (ReferenceManager.Instance.menu.activeSelf) return;

        if (EditModeManager.Instance.Playing) SwitchToEdit(playSoundEffect);
        else SwitchToPlay();

        foreach (BarTween tween in BarTween.tweenList)
        {
            tween.SetPlay(EditModeManager.Instance.Playing);
        }
    }

    public static void SwitchToPlay()
    {
        foreach (Transform player in ReferenceManager.Instance.playerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            if (MultiplayerManager.Instance.Multiplayer && !controller.photonView.IsMine) continue;

            controller.currentFields.Clear();
            controller.currentGameState = null;
            controller.deaths = 0;
        }

        EditModeManager.Instance.Playing = true;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(false);

        // disable placement preview
        ReferenceManager.Instance.placementPreview.SetActive(false);

        // disable windows
        ReferenceManager.Instance.ballWindows.SetActive(false);

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
                anim.SetBool(playingString, true);
            }
        }

        // activate coin animations
        foreach (Transform coin in ReferenceManager.Instance.coinContainer)
        {
            anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, true);
            anim.SetBool(pickedUp, coin.GetChild(0).GetComponent<CoinController>().pickedUp);
        }

        // activate key animations
        foreach (Transform key in ReferenceManager.Instance.keyContainer)
        {
            anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, true);
            anim.SetBool(pickedUp, key.GetChild(0).GetComponent<KeyController>().pickedUp);
        }

        // camera jumps to last player if its not on screen
        if (Camera.main != null) Camera.main.GetComponent<JumpToEntity>().Jump(true);
        EditModeManager.Instance.OnPlay?.Invoke();

        // close level settings panel if open
        LevelSettingsPanelTween lspt =
            ReferenceManager.Instance.levelSettingsPanel.GetComponent<LevelSettingsPanelTween>();
        if (lspt.open) lspt.Toggle();
    }

    public static void SwitchToEdit(bool playSoundEffect = true)
    {
        EditModeManager.Instance.Playing = false;

        if (playSoundEffect) AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(true);

        ResetGame();

        // enable placement preview and place it at mouse
        ReferenceManager.Instance.placementPreview.SetActive(true);
        ReferenceManager.Instance.placementPreview.transform.position =
            FollowMouse.GetCurrentMouseWorldPos(ReferenceManager.Instance.placementPreview.GetComponent<FollowMouse>()
                .worldPosition);

        // enable windows
        if (EditModeManager.Instance.CurrentEditMode is EditMode.ANCHOR or EditMode.BALL)
            ReferenceManager.Instance.ballWindows.SetActive(true);

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
                anim.SetBool(playingString, false);
            }
        }

        // reset Anchors
        foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
        {
            anchor.transform.GetChild(0).GetComponent<PathController>().ResetState();
        }

        // deactivate coin animations
        foreach (Transform coin in ReferenceManager.Instance.coinContainer.transform)
        {
            coin.GetChild(0).GetComponent<CoinController>().pickedUp = false;

            anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUp, false);
        }

        // deactivate key animations
        foreach (Transform key in ReferenceManager.Instance.keyContainer.transform)
        {
            key.GetChild(0).GetComponent<KeyController>().pickedUp = false;

            anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUp, false);
        }

        // remove game states from players
        foreach (Transform player in ReferenceManager.Instance.playerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            controller.currentGameState = null;
        }
        
        EditModeManager.Instance.OnEdit?.Invoke();
    }

    /// <summary>
    ///     Place edit mode at position
    /// </summary>
    /// <param name="editMode">the type of field/entity you want</param>
    /// <param name="pos">the position where it will be set</param>
    public static void PlaceEditModeAtPosition(EditMode editMode, Vector2 pos)
    {
        int matrixX = (int)Mathf.Round(pos.x);
        int matrixY = (int)Mathf.Round(pos.y);

        bool multiplayer = MultiplayerManager.Instance.Multiplayer;
        PhotonView photonView = Instance.photonView;

        float gridX = Mathf.Round(pos.x * 2) * 0.5f;
        float gridY = Mathf.Round(pos.y * 2) * 0.5f;

        if (editMode.IsFieldType())
            FieldManager.Instance.SetField((int)pos.x, (int)pos.y, ConvertEnum<EditMode, FieldType>(editMode));
        else
            switch (editMode)
            {
                case EditMode.DELETE_FIELD:
                {
                    // delete field
                    if (multiplayer) photonView.RPC("RemoveField", RpcTarget.All, matrixX, matrixY, true);
                    else FieldManager.Instance.RemoveField(matrixX, matrixY, true);

                    // remove player if at deleted pos
                    if (multiplayer)
                        photonView.RPC("RemovePlayerAtPosIntersect", RpcTarget.All, (float)matrixX, (float)matrixY);
                    else PlayerManager.Instance.RemovePlayerAtPosIntersect(matrixX, matrixY);
                    break;
                }
                case EditMode.PLAYER:
                    // place player
                    PlayerManager.Instance.SetPlayer(gridX, gridY, true);
                    break;
                case EditMode.COIN when multiplayer:
                    // place coin
                    photonView.RPC("SetCoin", RpcTarget.All, gridX, gridY);
                    break;
                case EditMode.COIN:
                    CoinManager.Instance.SetCoin(gridX, gridY);
                    break;
                default:
                {
                    if (KeyManager.IsKeyEditMode(editMode))
                    {
                        // get key color
                        string keyColorStr = editMode.ToString()[..^4];
                        KeyManager.KeyColor keyColor =
                            (KeyManager.KeyColor)Enum.Parse(typeof(KeyManager.KeyColor), keyColorStr);

                        // place key
                        if (multiplayer) photonView.RPC("SetKey", RpcTarget.All, gridX, gridY, keyColor);
                        else KeyManager.Instance.SetKey(gridX, gridY, keyColor);
                    }

                    break;
                }
            }
    }

    /// <summary>
    ///     resets every field and entity to its starting state
    ///     used when switched to edit mode
    /// </summary>
    public static void ResetGame()
    {
        // reset players
        foreach (GameObject player in PlayerManager.GetPlayers())
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (MultiplayerManager.Instance.Multiplayer && !controller.photonView.IsMine) continue;
            controller.DieNormal();
        }


        // reset balls
        foreach (Transform ball in ReferenceManager.Instance.ballDefaultContainer)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallDefaultController defaultController = ballObject.GetComponent<BallDefaultController>();

            ballObject.transform.position = defaultController.startPosition;
        }

        foreach (Transform ball in ReferenceManager.Instance.ballCircleContainer)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallCircleController controller = ballObject.GetComponent<BallCircleController>();

            controller.currentAngle = controller.startAngle;
            controller.UpdateAnglePos();
        }

        // reset coins
        foreach (Transform coin in ReferenceManager.Instance.coinContainer)
        {
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUp, false);
        }

        // reset keys
        foreach (Transform key in ReferenceManager.Instance.keyContainer)
        {
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUp, false);
        }

        foreach (GameObject player in PlayerManager.GetPlayers())
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.coinsCollected.Clear();
            controller.keysCollected.Clear();
        }

        // reset checkpoints
        foreach (Transform field in ReferenceManager.Instance.fieldContainer)
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
        string[] tags =
            { "KeyDoorField", "RedKeyDoorField", "GreenKeyDoorField", "BlueKeyDoorField", "YellowKeyDoorField" };
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
        List<Data> levelData = SaveSystem.LoadLevel();

        if (levelData != null)
        {
            LoadLevelFromData(levelData.ToArray());
        }
    }

    [PunRPC]
    public void LoadLevelFromData(Data[] levelData)
    {
        ClearLevel();
        List<FieldData> fieldData = new();
        PlayerData playerData = null;
        LevelSettingsData levelSettingsData = null;

        // load ball, coins
        foreach (Data levelObject in levelData)
        {
            if (levelObject.GetType() == typeof(FieldData))
            {
                fieldData.Add((FieldData)levelObject);
                continue;
            }

            if (levelObject.GetType() == typeof(PlayerData))
            {
                playerData = (PlayerData)levelObject;
                continue;
            }

            if (levelObject.GetType() == typeof(LevelSettingsData))
            {
                levelSettingsData = (LevelSettingsData)levelObject;
                continue;
            }

            levelObject.ImportToLevel();
        }


        // load fields
        foreach (FieldData field in fieldData)
        {
            field.ImportToLevel();
        }

        // load player last
        playerData?.ImportToLevel();

        // load level settings
        levelSettingsData?.ImportToLevel();
    }

    [PunRPC]
    public void ReceiveLevel(string content)
    {
        BinaryFormatter formatter = new();
        Stream s = GenerateStreamFromString(content);

        List<Data> data = formatter.Deserialize(s) as List<Data>;

        s.Close();

        LoadLevelFromData(data.ToArray());
    }

    private static Stream GenerateStreamFromString(string s)
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
        if (cam != null) cam.orthographicSize = width * 0.5f / cam.aspect;
        else throw new Exception($"Couldn't set camera width (in units) to {width} because main camera is null");
    }

    public static void SetCameraUnitHeight(float height)
    {
        Camera cam = Camera.main;
        if (cam != null) cam.orthographicSize = height * 0.5f;
        else throw new Exception($"Couldn't set camera height (in units) to {height} because main camera is null");
    }


    [PunRPC]
    public void ClearLevel()
    {
        PlayerManager.Instance.RemoveAllPlayers();
        Transform[] containers =
        {
            ReferenceManager.Instance.fieldContainer, ReferenceManager.Instance.playerContainer,
            ReferenceManager.Instance.ballDefaultContainer, ReferenceManager.Instance.ballCircleContainer,
            ReferenceManager.Instance.coinContainer, ReferenceManager.Instance.keyContainer,
            ReferenceManager.Instance.anchorContainer
        };
        foreach (Transform container in containers)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(container.GetChild(i).gameObject);
            }
        }
    }

    public static void RemoveObjectInContainer(float mx, float my, Transform container)
    {
        Collider2D[] hits = new Collider2D[container.childCount];
        _ = Physics2D.OverlapCircleNonAlloc(new(mx, my), 0.005f, hits, 128);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            if (hit.transform.parent == container)
            {
                Destroy(hit.gameObject);
            }
            else if (hit.transform.parent.parent == container)
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

    public static TEnumTo ConvertEnum<TEnumFrom, TEnumTo>(TEnumFrom e)
    {
        return (TEnumTo)Enum.Parse(typeof(TEnumTo), e.ToString());
    }

    public static object TryConvertEnum<TEnumFrom, TEnumTo>(TEnumFrom e)
    {
        Enum.TryParse(typeof(TEnumTo), e.ToString(), out object convEnum);

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
        Instance.OnGameQuit?.Invoke();

        Application.Quit();
    }

    public static void ForceDecimalSeparator(string separator)
    {
        string cultureName = Thread.CurrentThread.CurrentCulture.Name;
        CultureInfo ci = new(cultureName);
        if (ci.NumberFormat.NumberDecimalSeparator == separator) return;

        // Forcing use of decimal separator for numerical values
        ci.NumberFormat.NumberDecimalSeparator = separator;
        Thread.CurrentThread.CurrentCulture = ci;
    }

    public static bool DoFloatsEqual(float x, float y, float tolerance = 1e-10f)
    {
        float diff = Math.Abs(x - y);

        return diff <= tolerance || diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
    }
}