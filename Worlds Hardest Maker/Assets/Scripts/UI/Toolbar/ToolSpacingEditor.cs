using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ToolbarSpacing))]
public class ToolbarSpacingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ToolbarSpacing script = (ToolbarSpacing)target;

        if (GUILayout.Button("Set Toolbar height"))
        {
            script.UpdateSize();
        }
    }
    
}
#endif
