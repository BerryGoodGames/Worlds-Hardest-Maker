using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnchorBlockColor))]
public class AnchorBlockColorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnchorBlockColor script = (AnchorBlockColor)target;

        if (GUILayout.Button("Update color"))
        {
            script.UpdateColor();
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}