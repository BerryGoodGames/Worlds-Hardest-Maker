using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonTween))]
public class ButtonTweenEditor : Editor
{
    //[SerializeField] private Transform content;
    //[SerializeField] private Transform backgroundPanel;
    //[Space]
    //[SerializeField] private float clickDuration;
    //[SerializeField] private float highlightElevation;
    //[SerializeField] private float highlightFloating;
    //[SerializeField] private float highlightElevateDuration;
    //[SerializeField] private float highlightFloatingDuration;
    //[Space]
    //[SerializeField] private bool isWarningButton;
    //[SerializeField] private float singleShakeDuration;
    //[SerializeField] private float shake1;
    //[SerializeField] private float shake2;

    #region Properties
    SerializedProperty content;
    SerializedProperty backgroundPanel;

    SerializedProperty clickDuration;
    SerializedProperty highlightElevation;
    SerializedProperty highlightFloating;
    SerializedProperty highlightElevateDuration;
    SerializedProperty highlightFloatingDuration;

    SerializedProperty isWarningButton;
    SerializedProperty singleShakeDuration;
    SerializedProperty shake1;
    SerializedProperty shake2;
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

        if (!script.isWarningButton) GUI.enabled = false;
        EditorGUILayout.PropertyField(singleShakeDuration);
        EditorGUILayout.PropertyField(shake1);
        EditorGUILayout.PropertyField(shake2);
        if (!script.isWarningButton) GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
}
