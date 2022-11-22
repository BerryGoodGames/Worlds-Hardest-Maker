using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIAttachToPoint))]
public class UIAttachToPointEditor : Editor
{
    SerializedProperty point;
    SerializedProperty zoomWithCamera;
    SerializedProperty restrictToScreen;
    SerializedProperty restrictPaddingLeft;
    SerializedProperty restrictPaddingRight;
    SerializedProperty restrictPaddingTop;
    SerializedProperty restrictPaddingBottom;

    bool showPadding = false;

    public override void OnInspectorGUI()
    {

        UIAttachToPoint script = (UIAttachToPoint)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(point);
        EditorGUILayout.PropertyField(zoomWithCamera);
        EditorGUILayout.PropertyField(restrictToScreen);
        if(script.restrictToScreen)
        {
            showPadding = EditorGUILayout.Foldout(showPadding, "Padding");
            if (showPadding)
            {
                EditorGUILayout.PropertyField(restrictPaddingLeft, new GUIContent("Left"));
                EditorGUILayout.PropertyField(restrictPaddingRight, new GUIContent("Right"));
                EditorGUILayout.PropertyField(restrictPaddingTop, new GUIContent("Top"));
                EditorGUILayout.PropertyField(restrictPaddingBottom, new GUIContent("Bottom"));
            }
        }
        

        serializedObject.ApplyModifiedProperties();

    }

    private void OnEnable()
    {
        point = serializedObject.FindProperty("point");
        zoomWithCamera = serializedObject.FindProperty("zoomWithCamera");
        restrictToScreen = serializedObject.FindProperty("restrictToScreen");
        restrictPaddingLeft = serializedObject.FindProperty("restrictPaddingLeft");
        restrictPaddingRight = serializedObject.FindProperty("restrictPaddingRight");
        restrictPaddingTop = serializedObject.FindProperty("restrictPaddingTop");
        restrictPaddingBottom = serializedObject.FindProperty("restrictPaddingBottom");
    }
}
