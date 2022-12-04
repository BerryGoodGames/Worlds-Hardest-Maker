using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(UIRestrict))]
public class UIRestrictEditor : Editor
{
    SerializedProperty restrictPaddingLeft;
    SerializedProperty restrictPaddingRight;
    SerializedProperty restrictPaddingTop;
    SerializedProperty restrictPaddingBottom;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(restrictPaddingLeft, new GUIContent("Left"));
        EditorGUILayout.PropertyField(restrictPaddingRight, new GUIContent("Right"));
        EditorGUILayout.PropertyField(restrictPaddingTop, new GUIContent("Top"));
        EditorGUILayout.PropertyField(restrictPaddingBottom, new GUIContent("Bottom"));
        
        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        restrictPaddingLeft = serializedObject.FindProperty("restrictPaddingLeft");
        restrictPaddingRight = serializedObject.FindProperty("restrictPaddingRight");
        restrictPaddingTop = serializedObject.FindProperty("restrictPaddingTop");
        restrictPaddingBottom = serializedObject.FindProperty("restrictPaddingBottom");
    }
}
