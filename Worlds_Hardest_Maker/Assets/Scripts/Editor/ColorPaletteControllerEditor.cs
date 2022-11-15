using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CColorPalette))]
public class ColorPaletteControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CColorPalette script = (CColorPalette)target;
        if (GUILayout.Button("Update Color"))
        {
            script.UpdateColor();
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}