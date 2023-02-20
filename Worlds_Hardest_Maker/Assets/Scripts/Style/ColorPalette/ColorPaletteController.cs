using UnityEngine;
using UnityEngine.UI;

public class ColorPaletteController : MonoBehaviour
{
    [SerializeField] private string colorPaletteName;
    [SerializeField] private int colorPaletteIndex;

    public void UpdateColor()
    {
        ColorPalette colorPalette = ColorPaletteManager.GetColorPalette(colorPaletteName);
        if (colorPalette == null || colorPalette.Colors.Count <= colorPaletteIndex)
        {
            Debug.LogWarning("ColorPaletteController: color doesn't exist");
            return;
        }

        Color newColor = colorPalette.Colors[colorPaletteIndex];

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