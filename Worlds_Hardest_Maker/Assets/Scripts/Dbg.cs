using System;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
        MousePositionPixels
    }

    [Header("Settings")] public bool Enabled = true;
    public float GameSpeed = 1;

    [Space] [Foldout("Debug Text")] public DbgTextMode TextMode;
    [Foldout("Debug Text")] public float Count;

    [Foldout("Level")] public bool AutoLoadLevel;
    [Foldout("Level")] public string LevelName = "DebugLevel";

    [Foldout("Wall Outlines")] public bool WallOutlines = true;
    [Foldout("Wall Outlines")] public bool DrawRays;

    [FormerlySerializedAs("DebugText")] [Foldout("References")] [SerializeField] [MustBeAssigned]
    private TMP_Text debugText;

    private Camera cam;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        cam = Camera.main;
    }

    private void Start()
    {
        if (!AutoLoadLevel) return;
        GameManager.Instance.LoadLevel(Application.persistentDataPath + $"\\{LevelName}.lvl");
    }

    private void Update()
    {
        if (!Enabled) return;

        Time.timeScale = GameSpeed;
        switch (TextMode)
        {
            case DbgTextMode.Disabled:
                Text(string.Empty);
                break;
            case DbgTextMode.Custom:
                break;
            case DbgTextMode.Count:
                Text(Count);
                break;
            case DbgTextMode.FPS:
                Text(Mathf.Round(1 / Time.deltaTime));
                break;
            case DbgTextMode.PlayerPosition:
                try
                {
                    Text((Vector2)PlayerManager.GetPlayer().transform.position);
                }
                catch (Exception)
                {
                    Text("-");
                }

                break;
            case DbgTextMode.MousePositionUnits:
                Text((Vector2)cam.ScreenToWorldPoint(Input.mousePosition));
                break;
            case DbgTextMode.MousePositionPixels:
                Text((Vector2)Input.mousePosition);
                break;
        }
    }

    public static void Text(object obj)
    {
        try
        {
            Instance.debugText.text = obj.ToString();
        }
        catch
        {
            Instance.debugText.text = "failed";
        }
    }

    public static void PrintScriptAttachments<T>() where T : MonoBehaviour
    {
        Object[] list = FindObjectsOfType(typeof(T), true);
        string scriptName = typeof(T).Name;

        print($"Debug - Count of script {scriptName}: {list.Length}");
        foreach (Object o in list)
        {
            print($"Debug - {o.name}");
        }
    }
}