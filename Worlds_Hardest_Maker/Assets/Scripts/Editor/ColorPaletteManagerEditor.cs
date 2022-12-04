using UnityEditor;
using UnityEngine;

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