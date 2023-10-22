using System;
using MyBox;
using UnityEngine;

/// <summary>
/// This manager exists to assure that the session data (e.g. path of currently edited level) is always available and loaded,
/// unlike TransitionManager which is only loaded when the scene is entered after main menu
/// </summary>
public class LevelSessionManager : MonoBehaviour
{
    public static LevelSessionManager Instance { get; private set; }

    public static bool IsSessionFromEditor => !TransitionManager.HasLoaded;

    [ReadOnly] public string LevelSessionPath = string.Empty;
    [ReadOnly] public LevelData LoadedLevelData;
    [ReadOnly] public LevelSessionMode LevelSessionMode;

    public bool IsEdit => LevelSessionMode is LevelSessionMode.Edit;

    public TimeSpan EditTime = TimeSpan.Zero;

    private void Update() => EditTime += TimeSpan.FromSeconds(Time.deltaTime);

    private void Start()
    {
        if (IsSessionFromEditor)
        {
            // load level from Dbg if session is from editor
            if (Dbg.Instance.AutoLoadLevel) LevelSessionPath = SaveSystem.LevelSavePath + $"/{Dbg.Instance.LevelName}.lvl";

            LevelSessionMode = Dbg.Instance.EditorLevelSessionMode;
        }
        else
        {
            // pass data from TransitionManager over to LevelSessionManager
            LevelSessionPath = TransitionManager.Instance.LoadLevelPath;
            LevelSessionMode = TransitionManager.Instance.LevelSessionMode;

            // load user-selected level
            GameManager.Instance.LoadLevel(LevelSessionPath);

            if (TransitionManager.Instance.HasCreatedNewLevel)
            {
                // create new level info
                LoadedLevelData.Info = new()
                {
                    Description = TransitionManager.Instance.Inputs.Description,
                    Creator = TransitionManager.Instance.Inputs.Creator
                };
            }
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}