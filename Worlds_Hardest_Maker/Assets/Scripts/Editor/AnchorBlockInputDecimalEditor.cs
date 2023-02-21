using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnchorBlockInputDecimal))]
public class AnchorBlockInputDecimalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnchorBlockInputDecimal script = (AnchorBlockInputDecimal)target;
        
        if(GUILayout.Button("Update color")) script.UpdateColor();
    }
}
