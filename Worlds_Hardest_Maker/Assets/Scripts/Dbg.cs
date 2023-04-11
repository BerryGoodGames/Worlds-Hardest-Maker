using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
        DISABLED,
        CUSTOM,
        COUNT,
        FPS,
        PLAYER_POSITION,
        MOUSE_POSITION_UNITS,
        MOUSE_POSITION_PIXELS
    }

    [FormerlySerializedAs("dbgEnabled")] [Header("Settings")]
    public bool DbgEnabled = true;

    [FormerlySerializedAs("textMode")] [Space]
    public DbgTextMode TextMode;

    [FormerlySerializedAs("count")] public float Count;

    [FormerlySerializedAs("wallOutlines")] [Space]
    public bool WallOutlines = true;

    [FormerlySerializedAs("drawRays")] public bool DrawRays;

    [FormerlySerializedAs("gameSpeed")] [Space]
    public float GameSpeed = 1;

    [FormerlySerializedAs("debugText")] [Space] [Header("References")]
    public GameObject DebugText;

    private Camera cam;
    private Text dbgText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        cam = Camera.main;

        dbgText = Instance.DebugText.GetComponent<Text>();
    }

    private void Update()
    {
        if (!DbgEnabled) return;

        Time.timeScale = GameSpeed;
        switch (TextMode)
        {
            case DbgTextMode.DISABLED:
                Text(string.Empty);
                break;
            case DbgTextMode.CUSTOM:
                break;
            case DbgTextMode.COUNT:
                Text(Count);
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