using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScreenDimensions))]
public class SetScreenDimensionsEditor : Editor
{
    private SerializedProperty setScreenWidth;
    private SerializedProperty setScreenHeight;
    private SerializedProperty applyMaxZoomFromMapController;
    private SerializedProperty maxZoom;
    private SerializedProperty hasRectTransform;
    private SerializedProperty canvas;

    private void OnEnable()
    {
        setScreenWidth = serializedObject.FindProperty("setScreenWidth");
        setScreenHeight = serializedObject.FindProperty("setScreenHeight");
        applyMaxZoomFromMapController = serializedObject.FindProperty("applyMaxZoomFromMapController");
        maxZoom = serializedObject.FindProperty("maxZoom");
        hasRectTransform = serializedObject.FindProperty("hasRectTransform");
        canvas = serializedObject.FindProperty("canvas");
    }

    public override void OnInspectorGUI()
    {
        ScreenDimensions script = (ScreenDimensions)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(setScreenWidth);
        EditorGUILayout.PropertyField(setScreenHeight);
        EditorGUILayout.PropertyField(hasRectTransform);
        EditorGUILayout.PropertyField(applyMaxZoomFromMapController);
        if (!script.ApplyMaxZoomFromMapController) EditorGUILayout.PropertyField(maxZoom);
        if (script.HasRectTransform) EditorGUILayout.PropertyField(canvas);

        serializedObject.ApplyModifiedProperties();
    }
}
