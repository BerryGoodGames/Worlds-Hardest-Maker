using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CColorPalette : MonoBehaviour
{
    [SerializeField] private string colorPaletteName;
    [SerializeField] private int colorPaletteIndex;

    public void UpdateColor()
    {
        ColorPalette colorPalette = MColorPalette.GetColorPalette(colorPaletteName);
        if (colorPalette == null || colorPalette.colors.Count <= colorPaletteIndex)
        {
            Debug.LogWarning("ColorPaletteController: color does't exist");
            return;
        }

        Color newColor = colorPalette.colors[colorPaletteIndex];

        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.color = newColor;
        }
        else if (TryGetComponent(out Image image))
        {
            image.color = newColor;
        }
    }
}