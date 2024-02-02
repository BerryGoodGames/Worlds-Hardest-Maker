using System;
using JetBrains.Annotations;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using Object = UnityEngine.Object;

/// <summary>
///     Utility class for fast debug: custom for default log, count, fps, mouse pos
///     <para>Attach to game manager</para>
/// </summary>
public class Dbg : MonoBehaviour
{
    public static Dbg Instance { get; private set; }

    public enum DbgTextMode
    {
        Disabled,
        Custom,
        Count,
        FPS,
        PlayerPosition,
        MousePositionUnits,
        MousePositionPixels,
    }

    [field: Foldout("Settings")] [field: SerializeField] public bool Enabled { get; set; } = true;
    [field: Foldout("Settings")] [field: SerializeField] [field: PositiveValueOnly] public float GameSpeed { get; set; } = 1;

    [Foldout("Debug Text")] public DbgTextMode TextMode;
    [Foldout("Debug Text")] public uint Count;

    [Foldout("Level")] public bool AutoLoadLevel;
    [Foldout("Level")] [ConditionalField(nameof(AutoLoadLevel), true)] [SerializeField] private bool autoPlacePlayer;
    [Foldout("Level")] [ConditionalField(nameof(AutoLoadLevel))] public string LevelName = "DebugLevel";

    [Foldout("Wall Outlines")] public bool WallOutlines = true;
    [Foldout("Wall Outlines")] public bool DrawRays;

    [Foldout("Other")] public LevelSessionMode EditorLevelSessionMode;

    [Foldout("References")] [SerializeField] [MustBeAssigned] private TMP_Text debugText;

    private Camera cam;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        cam = Camera.main;
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (AutoLoadLevel)
        {
            if (!LevelSessionManager.IsSessionFromEditor) return;

            try
            {
                // create debug level if not existing
                if (!File.Exists(LevelSessionManager.Instance.LevelSessionPath))
                    LevelCreationController.CreateLevel(LevelName, "This is the debugging level", "BerryGoodGames");

                // load debug level
                GameManager.Instance.LoadLevel(LevelSessionManager.Instance.LevelSessionPath);
            }
            catch
            {
                // ignored
            }
        }
        else if (autoPlacePlayer) PlayerManager.Instance.SetPlayer(Vector2.zero, true);
#endif
    }

    private void Update()
    {
        if (!Enabled) return;

        Time.timeScale = GameSpeed;

        try
        {
            object message = TextMode switch
            {
                DbgTextMode.Disabled => string.Empty,
                DbgTextMode.Custom => string.Empty,
                DbgTextMode.Count => Count,
                DbgTextMode.FPS => Mathf.Round(1 / Time.deltaTime),
                DbgTextMode.PlayerPosition => (Vector2)PlayerManager.Instance.Player.transform.position,
                DbgTextMode.MousePositionUnits => (Vector2)cam.ScreenToWorldPoint(Input.mousePosition),
                DbgTextMode.MousePositionPixels => (Vector2)Input.mousePosition,
                _ => throw new ArgumentOutOfRangeException(),
            };

            Text(message);
        }
        catch (Exception) { Text("-"); }
    }

    [UsedImplicitly]
    public static void Text(object obj)
    {
        try { Instance.debugText.text = obj.ToString(); }
        catch { Instance.debugText.text = "failed"; }
    }

    [UsedImplicitly]
    public static void PrintScriptAttachments<T>() where T : MonoBehaviour
    {
        Object[] list = FindObjectsOfType(typeof(T), true);
        string scriptName = typeof(T).Name;

        print($"Debug - Count of script {scriptName}: {list.Length}");
        foreach (Object o in list) print($"Debug - {o.name}");
    }

    [ButtonMethod]
    // ReSharper disable once UnusedMember.Local
    private static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        print("Deleted Player Preferences");
    }
}