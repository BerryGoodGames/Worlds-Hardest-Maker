using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingGenerator), true)]
public class SettingGeneratorEditor : Editor
{

    #region Properties
    private SerializedProperty label;
    private SerializedProperty version;
    private SerializedProperty amount;
    private SerializedProperty fontSize;
    private SerializedProperty height;

    private SerializedProperty container;

    private SerializedProperty dropdownWidth;

    private SerializedProperty sliderWidth;
    private SerializedProperty sliderSize;

    private SerializedProperty numberInputWidth;
    #endregion

    private void OnEnable()
    {
        label = serializedObject.FindProperty("label");
        version = serializedObject.FindProperty("version");
        amount = serializedObject.FindProperty("amount");
        fontSize = serializedObject.FindProperty("fontSize");
        height = serializedObject.FindProperty("height");

        container = serializedObject.FindProperty("container");

        dropdownWidth = serializedObject.FindProperty("dropdownWidth");

        sliderWidth = serializedObject.FindProperty("sliderWidth");
        sliderSize = serializedObject.FindProperty("sliderSize");

        numberInputWidth = serializedObject.FindProperty("numberInputWidth");

    }

    public override void OnInspectorGUI()
    {
        SettingGenerator script = (SettingGenerator)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(container);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(label);
        EditorGUILayout.PropertyField(version);
        EditorGUILayout.PropertyField(amount);
        EditorGUILayout.PropertyField(fontSize);
        EditorGUILayout.PropertyField(height);

        EditorGUILayout.Space();

        // customized settings
        switch (script.version)
        {
            case SettingGenerator.SettingVersion.DROPDOWN:
                // dropdown settings
                EditorGUILayout.PropertyField(dropdownWidth);
                break;

            case SettingGenerator.SettingVersion.CHECKBOX:
                // nothing for checkbox
                break;

            case SettingGenerator.SettingVersion.SLIDER:
                // slider settings
                EditorGUILayout.PropertyField(sliderWidth);
                EditorGUILayout.PropertyField(sliderSize);
                break;

            case SettingGenerator.SettingVersion.NUMBER_INPUT:
                // numberinput settings
                EditorGUILayout.PropertyField(numberInputWidth);
                break;

            default:
                Debug.LogWarning($"Customized settings for settings generator isn't set for {script.version} yet!");
                break;
        }

        EditorGUILayout.Space();

        // generates setting on click
        if (GUILayout.Button("Generate"))
        {
            script.GenerateSetting();
        }

        serializedObject.ApplyModifiedProperties();
    }
}