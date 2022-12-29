using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FontManager))]
public class FontManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FontManager script = (FontManager)target;
        if (GUILayout.Button("Apply Default Font"))
        {
            script.ApplyDefaultFont();
        }
    }
}