using MyBox;
using UnityEditor;
using UnityEngine;

public class SettingGenerator : MonoBehaviour
{
    public enum SettingVersion
    {
        Dropdown,
        Checkbox,
        Slider,
        NumberInput,
        Header,
        Space,
    }

    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject dropdownOptionPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject checkboxOptionPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject sliderOptionPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject numberInputOptionPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject headerOptionPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject spaceOptionPrefab;
    [Space] [SerializeField] [InitializationField] [MustBeAssigned] private Transform container;
    
    #region Options
    
    #if UNITY_EDITOR
    [Header("Options")] [SerializeField] private string label;
    public SettingVersion Version;
    [SerializeField] private int amount = 1;
    [SerializeField] private float fontSize = 40;
    [SerializeField] private float height = 80;
    
    // custom properties
    // dropdown: dropdown width
    [SerializeField] private float dropdownWidth;
    
    // checkbox: none
    
    // slider: slider width, slider size
    [SerializeField] private float sliderWidth = 400;
    [SerializeField] private float sliderSize = 10;
    
    // numberinput: input width
    [SerializeField] private float numberInputWidth = 250;
    #endif
    
    #endregion
    
    #if UNITY_EDITOR
    public void GenerateSetting()
    {
        if (label.Length == 0)
        {
            Debug.LogWarning("You need to specify label!");
            return;
        }
        
        GameObject prefab = GetPrefab(Version);
        
        // iterate for the amount
        for (int i = 0; i < amount; i++)
        {
            // generate setting
            GameObject setting = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container);
            
            SettingOption option = setting.GetComponent<SettingOption>();
            
            // set label and object name
            option.Label.text = label;
            option.FontSize = fontSize;
            option.Height = height;
            option.Response();
            
            setting.name = setting.name.Replace("Option", label);
            
            // set customized settings
            CustomizeSettingOption(ref option, Version);
        }
    }
    
    private void CustomizeSettingOption(ref SettingOption option, SettingVersion version)
    {
        switch (version)
        {
            case SettingVersion.Dropdown:
                DropdownMenuOption dmo = (DropdownMenuOption)option;
                
                // width
                dmo.OriginalWidth = dmo.OriginalHeight * dropdownWidth / height;
                dmo.Response();
                
                break;
            
            case SettingVersion.NumberInput:
                NumberInputOption nio = (NumberInputOption)option;
                
                // width
                nio.Width = numberInputWidth;
                nio.Response();
                break;
            
            case SettingVersion.Checkbox: break;
            
            case SettingVersion.Slider:
                SliderUI s = ((SliderOption)option).SliderUI;
                
                // width, size
                s.Width = sliderWidth;
                s.Size = sliderSize;
                
                s.Response();
                break;
            
            case SettingVersion.Header: break;
            
            case SettingVersion.Space: break;
        }
    }
    
    private GameObject GetPrefab(SettingVersion version)
    {
        GameObject prefab;
        switch (version)
        {
            case SettingVersion.Dropdown:
                prefab = dropdownOptionPrefab;
                break;
            case SettingVersion.Checkbox:
                prefab = checkboxOptionPrefab;
                break;
            case SettingVersion.Slider:
                prefab = sliderOptionPrefab;
                break;
            case SettingVersion.NumberInput:
                prefab = numberInputOptionPrefab;
                break;
            case SettingVersion.Header:
                prefab = headerOptionPrefab;
                break;
            case SettingVersion.Space:
                prefab = spaceOptionPrefab;
                break;
            default:
                Debug.LogWarning($"You probably forgot to put a prefab for {version} here, defaulted to dropdown");
                prefab = dropdownOptionPrefab;
                break;
        }
        
        return prefab;
    }
    #endif
}