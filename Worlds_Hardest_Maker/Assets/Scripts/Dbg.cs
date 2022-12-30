using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
///     utility class for fast debug: custom for default log, count, fps, mouse pos
///     attach to game manager
/// </summary>
public class Dbg : MonoBehaviour
{
    public static Dbg Instance { get; private set; }

    public enum DbgTextMode
    {
        DISABLED,
        CUSTOM,
        COUNT,
        FPS,
        PLAYER_POSITION,
        MOUSE_POSITION_UNITS,
        MOUSE_POSITION_PIXELS
    }

    [Header("Settings")] public bool dbgEnabled = true;
    [Space] public DbgTextMode textMode;
    public float count;
    [Space] public bool wallOutlines = true;
    public bool drawRays;
    [Space] public float gameSpeed = 1;

    [FormerlySerializedAs("DebugText")] [Space] [Header("References")]
    public GameObject debugText;

    private Camera cam;
    private Text dbgText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        cam = Camera.main;

        dbgText = Instance.debugText.GetComponent<Text>();
    }

    private void Update()
    {
        if (!dbgEnabled) return;

        Time.timeScale = gameSpeed;
        switch (textMode)
        {
            case DbgTextMode.DISABLED:
                Text(string.Empty);
                break;
            case DbgTextMode.CUSTOM:
                break;
            case DbgTextMode.COUNT:
                Text(count);
                break;
            case DbgTextMode.FPS:
                Text(Mathf.Round(1 / Time.deltaTime));
                break;
            case DbgTextMode.PLAYER_POSITION:
                try
                {
                    Text((Vector2)PlayerManager.GetPlayer().transform.position);
                }
                catch (Exception)
                {
                    Text("-");
                }

                break;
            case DbgTextMode.MOUSE_POSITION_UNITS:
                Text((Vector2)cam.ScreenToWorldPoint(Input.mousePosition));
                break;
            case DbgTextMode.MOUSE_POSITION_PIXELS:
                Text((Vector2)Input.mousePosition);
                break;
        }
    }

    public static void Text(object obj)
    {
        try
        {
            Instance.dbgText.text = obj.ToString();
        }
        catch
        {
            Instance.dbgText.text = "failed";
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