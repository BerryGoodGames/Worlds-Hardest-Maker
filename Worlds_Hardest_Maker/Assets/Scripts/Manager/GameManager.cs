using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] [InitializationField] [MustBeAssigned] private LoadingScreen loadingScreen;
    [SerializeField] [InitializationField] [MustBeAssigned] private ChainableTween swipeTween;

    [Separator("Save")] [SerializeField] [PositiveValueOnly] private float autoSaveInterval = 300;

    private RectTransform canvasRT;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Utils.ForceDecimalSeparator(".");

        SetCameraUnitWidth(23);

        DOTween.Init(useSafeMode: false);
    }

    private void Start()
    {
        canvasRT = ReferenceManager.Instance.Canvas.GetComponent<RectTransform>();

        PlayerManager.Instance.SetPlayer(Vector2.zero);

        if (!LevelSessionManager.IsSessionFromEditor)
        {
            // user loaded editor scene from main menu
            // force enable start swipe
            swipeTween.gameObject.SetActive(true);
            swipeTween.StartChain();

            // make main menu also enable start swipe
            TransitionManager.Instance.HasMainMenuStartSwipe = true;
        }

        // start saving interval if either Dbg auto load enabled or any path given
        if ((LevelSessionManager.Instance.IsEdit && Dbg.Instance.AutoLoadLevel) ||
            LevelSessionManager.Instance.LevelSessionPath != string.Empty) StartCoroutine(AutoSave());
    }

    #region Save system

    public Coroutine LoadLevel(string path)
    {
        LevelData levelData = SaveSystem.LoadLevel(path);

        return levelData != null ? StartCoroutine(LoadLevelFromData(levelData, path)) : null;
    }

    public void LoadLevel()
    {
        (LevelData levelData, string path) = SaveSystem.LoadLevel();

        if (levelData != null) StartCoroutine(LoadLevelFromData(levelData, path));
    }

    public IEnumerator LoadLevelFromData(LevelData levelData, string levelPath)
    {
        yield return new WaitForEndOfFrame();

        // store data in LevelSessionManager
        LevelSessionManager.Instance.LoadedLevelData = levelData;

        // load
        List<Data> levelObjects = levelData.Objects;

        if (levelObjects.Count == 0)
        {
            // newly created level
            levelData.Objects = SaveSystem.SerializeCurrentLevel();

            SaveSystem.SerializeLevelData(levelPath, levelData);
            yield break;
        }

        LoadLevelFromDataRaw(levelObjects);
    }

    public void LoadLevelFromDataRaw(List<Data> levelObjects)
    {
        ClearLevel();

        List<FieldData> fieldData = new();
        PlayerData playerData = null;
        LevelSettingsData levelSettingsData = null;

        // load ball, coins
        foreach (Data levelObject in levelObjects)
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
        foreach (FieldData field in fieldData) field.ImportToLevel();

        // load player last
        playerData?.ImportToLevel();

        // load level settings
        levelSettingsData?.ImportToLevel();
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            // wait for next auto save
            yield return new WaitForSeconds(autoSaveInterval);

            SaveSystem.SaveCurrentLevel();
        }

        // ReSharper disable once IteratorNeverReturns
    }

    #endregion

    private static void SetCameraUnitWidth(float width)
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

    public static Vector2 ScreenToMainCanvas(Vector2 position) => position * (Instance.canvasRT.sizeDelta / new Vector2(Screen.width, Screen.height));

    public void ClearLevel()
    {
        if (PlayerManager.Instance.Player != null) PlayerManager.Instance.Player.DestroySelf();

        List<Transform> containers = new()
        {
            ReferenceManager.Instance.FieldContainer,
        };

        foreach (Transform t in ReferenceManager.Instance.EntityContainer) containers.Add(t);

        foreach (Transform container in containers)
        {
            for (int i = container.childCount - 1; i >= 0; i--) DestroyImmediate(container.GetChild(i).gameObject);
        }
    }

    public static void RemoveObjectInContainer(Vector2 position, Transform container)
    {
        Collider2D[] hits = new Collider2D[container.childCount];
        _ = Physics2D.OverlapCircleNonAlloc(position, 0.005f, hits, 128);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            if (hit.transform.parent == container) Destroy(hit.gameObject);
            else if (hit.transform.parent.parent == container) Destroy(hit.transform.parent.gameObject);
        }
    }

    public static void RemoveObjectInContainerIntersect(Vector2 position, Transform container)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };

        foreach (Vector2 d in deltas) RemoveObjectInContainer(position + d, container);
    }

    public void MainMenu()
    {
        BackupLevel();

        loadingScreen.LoadScene(0);
    }

    public void BackupLevel()
    {
        // save level if any path given
        if (LevelSessionManager.Instance.LevelSessionPath != string.Empty) SaveSystem.SaveCurrentLevel();
    }

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
            if (dropdown.options[i].text == option) return i;
        }

        Debug.LogWarning("There was no option found");
        return -1;
    }

    private void OnApplicationQuit() => BackupLevel();
}