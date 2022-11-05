using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SyncInputToSlider))]
public class SyncInputToSliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SyncInputToSlider script = (SyncInputToSlider)target;
        if (GUILayout.Button("Synchronise"))
        {
            script.Synchronise();
            script.UpdateInput();
        }
    }
}