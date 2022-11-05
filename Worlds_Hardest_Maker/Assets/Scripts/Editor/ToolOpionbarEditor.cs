using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToolOptionbar))]
public class ToolOpionbarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ToolOptionbar script = (ToolOptionbar)target;

        if (GUILayout.Button("Scale options"))
        {
            script.ScaleOptions();
        }

        if (GUILayout.Button("Update Height"))
        {
            script.UpdateHeight();
        }
    }

}