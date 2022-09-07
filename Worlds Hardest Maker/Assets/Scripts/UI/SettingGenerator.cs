using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SettingGenerator : MonoBehaviour
{
    private enum settingVersion
    {
        DROPDOWN, CHECKBOX, SLIDER
    }

    #region OPTIONS
#if UNITY_EDITOR
    [Header("Options")]
    [SerializeField] private string label;
    [SerializeField] private settingVersion version;
    [SerializeField] private int amount = 1;
#endif
#endregion

    [Space]

#region REFERENCES
    [Header("References")]
    [SerializeField] private GameObject dropdownPrefab;
    [SerializeField] private GameObject checkboxPrefab;
    [SerializeField] private GameObject sliderPrefab;
    [SerializeField] private Transform container;
#endregion

    public void GenerateSetting()
    {
#if UNITY_EDITOR
        GameObject prefab;
        // get right prefab, defaults to dropdow prefab
        switch(version)
        {
            case settingVersion.DROPDOWN:
                prefab = dropdownPrefab;
                break;
            case settingVersion.CHECKBOX:
                prefab = checkboxPrefab;
                break;
            case settingVersion.SLIDER:
                prefab = sliderPrefab;
                break;
            default:
                Debug.LogWarning("you probably forgor ?? to put prefab here, defaulted to dropdown");
                prefab = dropdownPrefab;
                break;
        }

        // iterate for the amount
        for (int i = 0; i < amount; i++)
        {
            // generate setting
            GameObject setting = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container);

            // set label and object name
            setting.GetComponentInChildren<TMPro.TMP_Text>().text = label;
            setting.name = setting.name.Replace("Option", label);

        }

#endif
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(SettingGenerator))]
public class SettingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SettingGenerator script = (SettingGenerator)target;
        // generates setting on click
        if (GUILayout.Button("Generate"))
        {
            script.GenerateSetting();
        }
    }
}
#endif