using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CButton))]
public class ButtonControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CButton script = (CButton)target;
        if (GUILayout.Button("Update outline / background panel size"))
        {
            script.UpdateSomeShit();
        }

        if (GUILayout.Button("Update EVERY button"))
        {
            CButton.UpdateEVERYFUCKINGShit();
        }
    }
}