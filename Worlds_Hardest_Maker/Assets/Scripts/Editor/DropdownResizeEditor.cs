using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DropdownResize))]
public class DropdownResizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DropdownResize script = (DropdownResize)target;

        if(GUILayout.Button("Resize to biggest option")) script.UpdateToBiggestOption();
    }
}
