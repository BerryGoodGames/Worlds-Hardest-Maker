using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// for doing color palettes'n'stuff
/// </summary>

public class MColorPalette : MonoBehaviour
{
    public static MColorPalette Instance { get; private set; }

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
        foreach (CColorPalette CPC in Resources.FindObjectsOfTypeAll<CColorPalette>())
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