using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToolOptionbar))]
public class ToolOptionbarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        base.OnInspectorGUI();

        ToolOptionbar script = (ToolOptionbar)target;

        GUILayout.BeginHorizontal();
            if (GUILayout.Button("Scale options"))
            {
                script.ScaleOptions();
            }

            if (GUILayout.Button("Update Height"))
            {
                script.UpdateHeight();
            }
        GUILayout.EndHorizontal();
    }

}