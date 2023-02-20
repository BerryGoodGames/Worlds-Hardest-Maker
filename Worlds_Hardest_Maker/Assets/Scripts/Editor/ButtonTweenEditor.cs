using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonTween))]
public class ButtonTweenEditor : Editor
{
    #region Properties
    private SerializedProperty content;
    private SerializedProperty backgroundPanel;

    private SerializedProperty clickDuration;
    private SerializedProperty highlightElevation;
    private SerializedProperty highlightFloating;
    private SerializedProperty highlightElevateDuration;
    private SerializedProperty highlightFloatingDuration;

    private SerializedProperty isWarningButton;
    private SerializedProperty singleShakeDuration;
    private SerializedProperty shake1;
    private SerializedProperty shake2;
    #endregion

    private void OnEnable()
    {
        content = serializedObject.FindProperty("content");
        backgroundPanel = serializedObject.FindProperty("backgroundPanel");

        clickDuration = serializedObject.FindProperty("clickDuration");
        highlightElevation = serializedObject.FindProperty("highlightElevation");
        highlightFloating = serializedObject.FindProperty("highlightFloating");
        highlightElevateDuration = serializedObject.FindProperty("highlightElevateDuration");
        highlightFloatingDuration = serializedObject.FindProperty("highlightFloatingDuration");

        isWarningButton = serializedObject.FindProperty("isWarningButton");
        singleShakeDuration = serializedObject.FindProperty("singleShakeDuration");
        shake1 = serializedObject.FindProperty("shake1");
        shake2 = serializedObject.FindProperty("shake2");
    }

    public override void OnInspectorGUI()
    {
        ButtonTween script = (ButtonTween)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(content);
        EditorGUILayout.PropertyField(backgroundPanel);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(clickDuration);
        EditorGUILayout.PropertyField(highlightElevation);
        EditorGUILayout.PropertyField(highlightFloating);
        EditorGUILayout.PropertyField(highlightElevateDuration);
        EditorGUILayout.PropertyField(highlightFloatingDuration);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(isWarningButton);

        if (!script.IsWarningButton) GUI.enabled = false;
        EditorGUILayout.PropertyField(singleShakeDuration);
        EditorGUILayout.PropertyField(shake1);
        EditorGUILayout.PropertyField(shake2);
        if (!script.IsWarningButton) GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
}
