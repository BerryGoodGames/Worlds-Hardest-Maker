using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    
    public enum MenuTab
    {
        Graphic = 0,
        UI = 1,
        Sound = 2,
        KeyBinds = 3,
    }
    
    [Header("Constants & References")] [SerializeField] [Required] private AlphaTween menuTween;
    [SerializeField] [Required] private GameObject graphicSettingsUI;
    [SerializeField] [Required] private GameObject uiSettingsUI;
    [SerializeField] [Required] private GameObject soundSettingsUI;
    [SerializeField] [Required] private GameObject keyBindSettingsUI;
    
    [Space] [Header("Variables")] public MenuTab CurrentMenuTab;
    
    private MenuTab prevMenuTab;
    
    [HideInInspector] public bool BlockMenu;
    
    [HideInInspector] public bool IsAddingKeyBind;
    [HideInInspector] public KeyBindSetterController AddingKeyBindSetter;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    
    private void Start() => ChangeMenuTab(CurrentMenuTab);
    
    public void ToggleMenu() => SetMenuVisible(!menuTween.IsVisible);
    
    public void SetMenuVisible(bool visible) => menuTween.SetVisible(visible);
    
    #region Menu Tab
    
    public void ChangeMenuTab(MenuTab tab)
    {
        // REF
        if (tab != prevMenuTab)
        {
            Dictionary<MenuTab, GameObject> dict = GetTabDict();
            for (int i = 0; i < Enum.GetValues(typeof(MenuTab)).Length; i++) dict[(MenuTab)i].SetActive(false);
            
            dict[tab].SetActive(true);
            CurrentMenuTab = tab;
        }
        
        prevMenuTab = tab;
    }
    
    private Dictionary<MenuTab, GameObject> GetTabDict()
    {
        Dictionary<MenuTab, GameObject> dict = new()
        {
            { MenuTab.Graphic, graphicSettingsUI },
            { MenuTab.Sound, soundSettingsUI },
            { MenuTab.UI, uiSettingsUI },
            { MenuTab.KeyBinds, keyBindSettingsUI },
        };
        
        return dict;
    }
    
    public void ChangeMenuTab(int tab) => ChangeMenuTab((MenuTab)tab);
    
    public void ChangeMenuTab() => ChangeMenuTab(CurrentMenuTab);
    
    #endregion
}