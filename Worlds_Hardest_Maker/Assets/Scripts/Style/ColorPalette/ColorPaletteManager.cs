using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// for doing color palettes'n'stuff
/// </summary>

public class ColorPaletteManager : MonoBehaviour
{
    public static ColorPaletteManager Instance { get; private set; }

    public List<ColorPalette> colorPalettes;

    private void Awake()
    {
        UpdateInstance();
    }

    public void UpdateInstance()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateColorPalettes()   
    {
        foreach (ColorPaletteController CPC in Resources.FindObjectsOfTypeAll<ColorPaletteController>())
        {
            CPC.UpdateColor();
        }
    }

    public static ColorPalette GetColorPalette(string name)
    {
        foreach (ColorPalette colorPalette in Instance.colorPalettes)
        {
            if (colorPalette.name.Equals(name))
            {
                return colorPalette;
            }
        }

        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ColorPaletteManager))]
public class ColorPaletteManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ColorPaletteManager script = (ColorPaletteManager)target;
        if (GUILayout.Button("Update Color Palettes"))
        {
            script.UpdateInstance();
            script.UpdateColorPalettes();
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}
#endif