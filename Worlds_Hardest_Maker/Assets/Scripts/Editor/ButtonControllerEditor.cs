using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonController))]
public class ButtonControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ButtonController script = (ButtonController)target;
        if (GUILayout.Button("Update outline / background panel size"))
        {
            script.UpdateSomeShit();
        }

        if (GUILayout.Button("Update EVERY button"))
        {
            ButtonController.UpdateEVERYFUCKINGShit();
        }
    }
}