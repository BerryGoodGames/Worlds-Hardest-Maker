using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class ColorPaletteController : MonoBehaviour
{
    [SerializeField] private string colorPaletteName;
    [SerializeField] private int colorPaletteIndex;

    public void UpdateColor()
    {
        ColorPalette colorPalette = ColorPaletteManager.GetColorPalette(colorPaletteName);
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

#if UNITY_EDITOR
[CustomEditor(typeof(ColorPaletteController))]
public class ColorPaletteControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ColorPaletteController script = (ColorPaletteController)target;
        if (GUILayout.Button("Update Color"))
        {
            script.UpdateColor();
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}
#endif