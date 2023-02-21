using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnchorBlockDropdown))]
public class AnchorBlockDropdownEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnchorBlockDropdown script = (AnchorBlockDropdown)target;

        if (GUILayout.Button("Update color"))
        {
            script.UpdateColor();
        }
    }
}
