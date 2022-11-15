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
                prefab = MPrefab.Instance.DropdownOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.CHECKBOX:
                prefab = MPrefab.Instance.CheckboxOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.SLIDER:
                prefab = MPrefab.Instance.SliderOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.NUMBER_INPUT:
                prefab = MPrefab.Instance.NumberInputOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.HEADER:
                prefab = MPrefab.Instance.HeaderOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.SPACE:
                prefab = MPrefab.Instance.SpaceOptionPrefab;
                break;
            default:
                Debug.LogWarning($"You probably forgot to put a prefab for {version} here, defaulted to dropdown");
                prefab = MPrefab.Instance.DropdownOptionPrefab;
                break;
        }
        return prefab;
    }
}
