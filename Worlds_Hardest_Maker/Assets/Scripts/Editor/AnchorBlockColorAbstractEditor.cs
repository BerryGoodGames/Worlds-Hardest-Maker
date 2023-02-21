using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnchorBlockColorController), true)]
public class AnchorBlockAbstractEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnchorBlockColorController script = (AnchorBlockColorController)target;

        GUILayout.Space(15);
        if (!GUILayout.Button("Update color")) return;

        script.UpdateColor();
        EditorApplication.QueuePlayerLoopUpdate();
    }
}