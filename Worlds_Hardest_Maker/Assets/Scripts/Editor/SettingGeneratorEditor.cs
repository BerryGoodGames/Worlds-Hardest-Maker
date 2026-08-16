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
    
    private SerializedProperty dropdownOptionPrefab;
    private SerializedProperty checkboxOptionPrefab;
    private SerializedProperty sliderOptionPrefab;
    private SerializedProperty numberInputOptionPrefab;
    private SerializedProperty headerOptionPrefab;
    private SerializedProperty spaceOptionPrefab;
    
    private SerializedProperty container;
    
    private SerializedProperty dropdownWidth;
    
    private SerializedProperty sliderWidth;
    private SerializedProperty sliderSize;
    
    private SerializedProperty numberInputWidth;
    
    #endregion
    
    private void OnEnable()
    {
        label = serializedObject.FindProperty("label");
        version = serializedObject.FindProperty("Version");
        amount = serializedObject.FindProperty("amount");
        fontSize = serializedObject.FindProperty("fontSize");
        height = serializedObject.FindProperty("height");
        
        dropdownOptionPrefab = serializedObject.FindProperty("dropdownOptionPrefab");
        checkboxOptionPrefab = serializedObject.FindProperty("checkboxOptionPrefab");
        sliderOptionPrefab = serializedObject.FindProperty("sliderOptionPrefab");
        numberInputOptionPrefab = serializedObject.FindProperty("numberInputOptionPrefab");
        headerOptionPrefab = serializedObject.FindProperty("headerOptionPrefab");
        spaceOptionPrefab = serializedObject.FindProperty("spaceOptionPrefab");
        
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
        
        EditorGUILayout.PropertyField(dropdownOptionPrefab);
        EditorGUILayout.PropertyField(checkboxOptionPrefab);
        EditorGUILayout.PropertyField(sliderOptionPrefab);
        EditorGUILayout.PropertyField(numberInputOptionPrefab);
        EditorGUILayout.PropertyField(headerOptionPrefab);
        EditorGUILayout.PropertyField(spaceOptionPrefab);
        EditorGUILayout.PropertyField(container);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(label);
        EditorGUILayout.PropertyField(version);
        EditorGUILayout.PropertyField(amount);
        EditorGUILayout.PropertyField(fontSize);
        EditorGUILayout.PropertyField(height);
        
        EditorGUILayout.Space();
        
        // customized settings
        switch (script.Version)
        {
            case SettingGenerator.SettingVersion.Dropdown:
                // dropdown settings
                EditorGUILayout.PropertyField(dropdownWidth);
                break;
            
            case SettingGenerator.SettingVersion.Checkbox:
                // nothing for checkbox
                break;
            
            case SettingGenerator.SettingVersion.Slider:
                // slider settings
                EditorGUILayout.PropertyField(sliderWidth);
                EditorGUILayout.PropertyField(sliderSize);
                break;
            
            case SettingGenerator.SettingVersion.NumberInput:
                // numberinput settings
                EditorGUILayout.PropertyField(numberInputWidth);
                break;
            
            default:
                Debug.Log($"Customized settings for settings generator isn't set for {script.Version} yet!");
                break;
        }
        
        EditorGUILayout.Space();
        
        // generates setting on click
        if (GUILayout.Button("Generate")) script.GenerateSetting();
        
        serializedObject.ApplyModifiedProperties();
    }
}