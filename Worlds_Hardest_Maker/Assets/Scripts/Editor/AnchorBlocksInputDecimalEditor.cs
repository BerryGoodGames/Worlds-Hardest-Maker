using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnchorBlocksInputDecimal))]
public class AnchorBlocksInputDecimalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnchorBlocksInputDecimal script = (AnchorBlocksInputDecimal)target;
        
        if(GUILayout.Button("Update color")) script.UpdateColor();
    }
}
