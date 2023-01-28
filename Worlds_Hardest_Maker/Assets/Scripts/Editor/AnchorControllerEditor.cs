using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnchorController))]
public class AnchorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnchorController anchorController = (AnchorController)target;
        if (Application.isPlaying && GUILayout.Button("Execute blocks"))
        {
            anchorController.StartExecuting();
        }
        if (Application.isPlaying && GUILayout.Button("Reset execution"))
        {
            anchorController.ResetExecution();
        }
    }
}