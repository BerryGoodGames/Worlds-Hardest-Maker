using UnityEngine;

public static class SettingVersionExtension
{
    public static GameObject GetPrefab(this SettingGenerator.SettingVersion version)
    {
        GameObject prefab;
        switch (version)
        {
            case SettingGenerator.SettingVersion.Dropdown:
                prefab = PrefabManager.Instance.DropdownOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.Checkbox:
                prefab = PrefabManager.Instance.CheckboxOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.Slider:
                prefab = PrefabManager.Instance.SliderOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.NumberInput:
                prefab = PrefabManager.Instance.NumberInputOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.Header:
                prefab = PrefabManager.Instance.HeaderOptionPrefab;
                break;
            case SettingGenerator.SettingVersion.Space:
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