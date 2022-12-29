using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InfobarResize))]
public class InfobarResizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InfobarResize script = (InfobarResize)target;
        if (GUILayout.Button("PlaceEditModeAtPosition Infobar height"))
        {
            script.UpdateSize();
        }
    }
}