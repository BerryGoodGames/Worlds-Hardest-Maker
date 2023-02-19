using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnchorBlocksText))]
public class AnchorBlocksTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnchorBlocksText script = (AnchorBlocksText)target;

        if(GUILayout.Button("Update layout element width")) script.UpdateLayoutWidth();
    }
}
