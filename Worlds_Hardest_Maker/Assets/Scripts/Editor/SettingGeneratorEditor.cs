using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingGenerator))]
public class SettingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SettingGenerator script = (SettingGenerator)target;
        // generates setting on click
        if (GUILayout.Button("Generate"))
        {
            script.GenerateSetting();
        }
    }
}