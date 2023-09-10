using System;
using UnityEngine;
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
        Disabled,
        Custom,
        Count,
        FPS,
        PlayerPosition,
        MousePositionUnits,
        MousePositionPixels
    }

    [Header("Settings")] public bool DbgEnabled = true;

    [Space] public DbgTextMode TextMode;

    public float Count;

    [Space] public bool WallOutlines = true;

    public bool DrawRays;

    [Space] public float GameSpeed = 1;

    [Space] [Header("References")] public GameObject DebugText;

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