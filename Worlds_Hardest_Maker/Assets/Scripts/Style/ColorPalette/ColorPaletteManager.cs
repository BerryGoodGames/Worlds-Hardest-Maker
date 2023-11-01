using System.Collections.Generic;
using UnityEngine;

public class ColorPaletteManager : MonoBehaviour
{
    public static ColorPaletteManager Instance { get; private set; }

    public List<ColorPalette> ColorPalettes;

    private void Awake() => UpdateInstance();

    public void UpdateInstance()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateColorPalettes()
    {
        foreach (ColorPaletteController cpc in Resources.FindObjectsOfTypeAll<ColorPaletteController>()) cpc.UpdateColor();
    }

    public static ColorPalette GetColorPalette(string name)
    {
        foreach (ColorPalette colorPalette in Instance.ColorPalettes)
        {
            if (colorPalette.Name.Equals(name)) return colorPalette;
        }

        throw new("ColorPalette doesn't exist");
    }
}