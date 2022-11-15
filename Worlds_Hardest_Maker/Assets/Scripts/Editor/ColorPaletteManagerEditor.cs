using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MColorPalette))]
public class ColorPaletteManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MColorPalette script = (MColorPalette)target;
        if (GUILayout.Button("Update Color Palettes"))
        {
            script.UpdateInstance();
            script.UpdateColorPalettes();
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}