using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MFont))]
public class FontManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MFont script = (MFont)target;
        if (GUILayout.Button("Apply Default Font"))
        {
            script.ApplyDefaultFont();
        }
    }
}