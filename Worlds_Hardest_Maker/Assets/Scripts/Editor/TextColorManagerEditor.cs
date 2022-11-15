using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextColorManager))]
public class TextColorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TextColorManager script = (TextColorManager)target;
        if (GUILayout.Button("Apply Default Color"))
        {
            script.ApplyDefaultColor();
        }
    }
}