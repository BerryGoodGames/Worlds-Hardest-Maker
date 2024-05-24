using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorPaletteController))]
public class ColorPaletteControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ColorPaletteController script = (ColorPaletteController)target;
        
        if (!GUILayout.Button("Update Color")) return;
        
        script.UpdateColor();
        EditorApplication.QueuePlayerLoopUpdate();
    }
}