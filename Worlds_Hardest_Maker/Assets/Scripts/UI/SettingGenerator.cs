using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SettingGenerator : MonoBehaviour
{
    private enum SettingVersion
    {
        DROPDOWN, CHECKBOX, SLIDER, NUMBER_INPUT
    }

    #region OPTIONS
#if UNITY_EDITOR
    [Header("Options")]
    [SerializeField] private string label;
    [SerializeField] private SettingVersion version;
    [SerializeField] private int amount = 1;
    [SerializeField] private float fontSize = 40;
    [SerializeField] private float height = 80;
#endif
    #endregion

    [Space]

    #region REFERENCES
    [Header("References")]
    [SerializeField] private GameObject dropdownPrefab;
    [SerializeField] private GameObject checkboxPrefab;
    [SerializeField] private GameObject sliderPrefab;
    [SerializeField] private GameObject numberInputPrefab;
    [SerializeField] private Transform container;
    #endregion

    public void GenerateSetting()
    {
#if UNITY_EDITOR
        GameObject prefab;
        // get right prefab, defaults to dropdow prefab
        switch(version)
        {
            case SettingVersion.DROPDOWN:
                prefab = dropdownPrefab;
                break;
            case SettingVersion.CHECKBOX:
                prefab = checkboxPrefab;
                break;
            case SettingVersion.SLIDER:
                prefab = sliderPrefab;
                break;
            case SettingVersion.NUMBER_INPUT:
                prefab = numberInputPrefab;
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

            SettingOption option = setting.GetComponent<SettingOption>();

            // set label and object name
            option.label.text = label;
            option.FontSize = fontSize;
            option.Height = height;
            option.Response();

            setting.name = setting.name.Replace("Option", label);
        }

#endif
    }
}