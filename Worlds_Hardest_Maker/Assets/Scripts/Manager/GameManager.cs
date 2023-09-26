using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private LoadingScreen loadingScreen;
    private RectTransform canvasRT;

    private void Awake()
    {
        // init singleton
        if (Instance == null)
            Instance = this;
        // DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);

        Utils.ForceDecimalSeparator(".");

        SetCameraUnitWidth(23);
    }

    private void Start()
    {
        canvasRT = ReferenceManager.Instance.Canvas.GetComponent<RectTransform>();
        DOTween.Init(useSafeMode: false);

        if (!MultiplayerManager.Instance.Multiplayer) PlayerManager.Instance.SetPlayer(0, 0, 3f);

        LevelSettings.Instance.SetDrownDuration();
        LevelSettings.Instance.SetIceFriction();
        LevelSettings.Instance.SetIceMaxSpeed();
        LevelSettings.Instance.SetWaterDamping();
    }

    /// <summary>
    ///     Places edit mode at position
    /// </summary>
    /// <param name="editMode">the type of field/entity you want</param>
    public static void PlaceEditModeAtPosition(EditMode editMode, Vector2 pos)
    {
        int matrixX = (int)Mathf.Round(pos.x);
        int matrixY = (int)Mathf.Round(pos.y);

        bool multiplayer = MultiplayerManager.Instance.Multiplayer;
        PhotonView photonView = Instance.photonView;

        float gridX = Mathf.Round(pos.x * 2) * 0.5f;
        float gridY = Mathf.Round(pos.y * 2) * 0.5f;

        if (editMode.IsFieldType())
        {
            FieldManager.Instance.SetField((int)pos.x, (int)pos.y,
                EnumUtils.ConvertEnum<EditMode, FieldType>(editMode));
        }
        else
        {
            switch (editMode)
            {
                case EditMode.DeleteField:
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
                case EditMode.Player:
                    // place player
                    PlayerManager.Instance.SetPlayer(gridX, gridY, true);
                    break;
                case EditMode.Coin when multiplayer:
                    // place coin
                    photonView.RPC("SetCoin", RpcTarget.All, gridX, gridY);
                    break;
                case EditMode.Coin:
                    CoinManager.Instance.SetCoin(gridX, gridY);
                    break;
                case EditMode.AnchorBall:
                    AnchorBallManager.SetAnchorBall(gridX, gridY);
                    break;
                default:
                {
                    if (KeyManager.IsKeyEditMode(editMode))
                    {
                        // get key color
                        string editModeStr = editMode.ToString();
                        string keyColorStr = editModeStr.Remove(editModeStr.Length - 3);
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
    }

    #region Save system

    public void LoadLevel(string path)
    {
        List<Data> levelData = SaveSystem.LoadLevel(path);

        if (levelData != null) LoadLevelFromData(levelData.ToArray());
    }
    public void LoadLevel()
    {
        List<Data> levelData = SaveSystem.LoadLevel();

        if (levelData != null) LoadLevelFromData(levelData.ToArray());
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

        if (data == null)
            throw new Exception("Something went wrong when receiving level and parsing received information");

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

    public static Vector2 ScreenToMainCanvas(Vector2 position) =>
        position * (Instance.canvasRT.sizeDelta / new Vector2(Screen.width, Screen.height));


    [PunRPC]
    public void ClearLevel()
    {
        PlayerManager.Instance.RemoveAllPlayers();
        Transform[] containers =
        {
            ReferenceManager.Instance.FieldContainer, ReferenceManager.Instance.PlayerContainer,
            ReferenceManager.Instance.CoinContainer, ReferenceManager.Instance.KeyContainer,
            ReferenceManager.Instance.AnchorContainer
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
                Destroy(hit.gameObject);
            else if (hit.transform.parent.parent == container) Destroy(hit.transform.parent.gameObject);
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

    public void MainMenu() => loadingScreen.LoadScene(0);

    public static void DeselectInputs()
    {
        EventSystem eventSystem = EventSystem.current;
        if (!eventSystem.alreadySelecting) eventSystem.SetSelectedGameObject(null);
    }

    public static Vector2 GetCanvasDimensions()
    {
        Rect canvas = ReferenceManager.Instance.Canvas.GetComponent<RectTransform>().rect;
        return new(canvas.width, canvas.height);
    }

    public static int GetDropdownValue(string option, TMP_Dropdown dropdown)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == option)
                return i;
        }

        Debug.LogWarning("There was no option found");
        return -1;
    }
}