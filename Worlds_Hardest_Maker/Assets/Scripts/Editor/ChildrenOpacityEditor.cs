using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChildrenOpacity))]
public class ChildrenOpacityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChildrenOpacity script = (ChildrenOpacity)target;

        if (GUILayout.Button("Update Opacity"))
        {
            script.UpdateOpacity();
        }
    }

}