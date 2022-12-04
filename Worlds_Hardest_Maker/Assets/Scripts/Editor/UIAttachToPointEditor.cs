using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// [CustomEditor(typeof(UIAttachToPoint))]
public class UIAttachToPointEditor : Editor
{
    SerializedProperty point;
    SerializedProperty zoomSizeWithCamera;
    SerializedProperty zoomPositionWithCamera;
    SerializedProperty restrictToScreen;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(point);
        EditorGUILayout.PropertyField(zoomSizeWithCamera);
        EditorGUILayout.PropertyField(zoomPositionWithCamera);
        EditorGUILayout.PropertyField(restrictToScreen);

        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        point = serializedObject.FindProperty("point");
        zoomSizeWithCamera = serializedObject.FindProperty("zoomSizeWithCamera");
        zoomPositionWithCamera = serializedObject.FindProperty("zoomPositionWithCamera");
        restrictToScreen = serializedObject.FindProperty("restrictToScreen");
    }
}
