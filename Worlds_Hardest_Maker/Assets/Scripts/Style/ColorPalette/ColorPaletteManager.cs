using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     for doing color palettes 'n' stuff
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
        foreach (ColorPaletteController cpc in Resources.FindObjectsOfTypeAll<ColorPaletteController>())
        {
            cpc.UpdateColor();
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