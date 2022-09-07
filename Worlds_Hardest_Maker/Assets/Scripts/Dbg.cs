using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// utility class for fast debug: custom for default log, count, fps, mouse pos
/// attach to game manager
/// </summary>
public class Dbg : MonoBehaviour
{
    public static Dbg Instance { get; private set; }
    public enum DbgTextMode
    {
        DISABLED, CUSTOM, COUNT, FPS, MOUSE_POSITION_UNITS, MOUSE_POSITION_PIXELS
    }

    [Header("Settings")]
    public bool dbgEnabled = true;
    [Space]
    public DbgTextMode textMode;
    public float count;
    [Space]
    public bool wallOutlines = true;
    public bool drawRays = false;

    [Space]
    [Header("References")]
    public GameObject DebugText;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Update()
    {
        if(dbgEnabled)
        {
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
                case DbgTextMode.MOUSE_POSITION_UNITS:
                    Text((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    break;
                case DbgTextMode.MOUSE_POSITION_PIXELS:
                    Text((Vector2)Input.mousePosition);
                    break;
            }
        }
    }

    public static void Text(object obj)
    {
        try
        {
            Instance.DebugText.GetComponent<Text>().text = obj.ToString();
        }
        catch
        {
            Instance.DebugText.GetComponent<Text>().text = "failed";
        }
    }
}
