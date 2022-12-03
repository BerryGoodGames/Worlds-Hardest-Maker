using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BackgroundLineSize))]
public class BackgroundLineSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BackgroundLineSize script = (BackgroundLineSize)target;
        if (GUILayout.Button("Set Line Size"))
        {
            script.SetLineSize();
        }
    }
}