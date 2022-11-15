using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingVersionExtension
{
    public static GameObject GetPrefab(this SettingGenerator.SettingVersion version)
    {
        GameObject prefab;
        switch (version)
        {
            case SettingGenerator.SettingVersion.DROPDOWN:
                prefab = PrefabManager.Instance.DropdownOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.CHECKBOX:
                prefab = PrefabManager.Instance.CheckboxOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.SLIDER:
                prefab = PrefabManager.Instance.SliderOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.NUMBER_INPUT:
                prefab = PrefabManager.Instance.NumberInputOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.HEADER:
                prefab = PrefabManager.Instance.HeaderOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.SPACE:
                prefab = PrefabManager.Instance.SpaceOptionPrefab;
                break;
            default:
                Debug.LogWarning($"You probably forgot to put a prefab for {version} here, defaulted to dropdown");
                prefab = PrefabManager.Instance.DropdownOptionPrefab;
                break;
        }
        return prefab;
    }
}
