using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToolbarSizing))]
public class ToolbarSpacingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ToolbarSizing script = (ToolbarSizing)target;

        if (GUILayout.Button("Set Toolbar height"))
        {
            script.UpdateSize();
        }

        if (GUILayout.Button("Scale every option in optionbars"))
        {
            script.ScaleOptionsInOptionbars();
        }

        if (GUILayout.Button("Update optionbar heights"))
        {
            script.UpdateOptionbarHeights();
        }
    }

}