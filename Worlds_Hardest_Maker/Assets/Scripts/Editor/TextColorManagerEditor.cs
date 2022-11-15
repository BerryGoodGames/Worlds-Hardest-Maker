using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MTextColor))]
public class TextColorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MTextColor script = (MTextColor)target;
        if (GUILayout.Button("Apply Default Color"))
        {
            script.ApplyDefaultColor();
        }
    }
}