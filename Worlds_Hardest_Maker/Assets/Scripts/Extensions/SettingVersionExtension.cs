using UnityEngine;

public static class SettingVersionExtension
{
    public static GameObject GetPrefab(this SettingGenerator.SettingVersion version)
    {
        GameObject prefab;
        switch (version)
        {
            case SettingGenerator.SettingVersion.DROPDOWN:
                prefab = PrefabManager.Instance.dropdownOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.CHECKBOX:
                prefab = PrefabManager.Instance.checkboxOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.SLIDER:
                prefab = PrefabManager.Instance.sliderOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.NUMBER_INPUT:
                prefab = PrefabManager.Instance.numberInputOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.HEADER:
                prefab = PrefabManager.Instance.headerOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.SPACE:
                prefab = PrefabManager.Instance.spaceOptionPrefab;
                break;
            default:
                Debug.LogWarning($"You probably forgot to put a prefab for {version} here, defaulted to dropdown");
                prefab = PrefabManager.Instance.dropdownOptionPrefab;
                break;
        }

        return prefab;
    }
}