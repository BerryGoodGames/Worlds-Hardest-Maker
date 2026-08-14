using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

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
    
    [Header("Constants & References")] [SerializeField] [MustBeAssigned] private AlphaTween menuTween;
    [SerializeField] [MustBeAssigned] private GameObject graphicSettingsUI;
    [SerializeField] [MustBeAssigned] private GameObject uiSettingsUI;
    [SerializeField] [MustBeAssigned] private GameObject soundSettingsUI;
    [SerializeField] [MustBeAssigned] private GameObject keyBindSettingsUI;
    
    [Space] [Header("Variables")] public MenuTab CurrentMenuTab;
    
    private MenuTab prevMenuTab;
    
    [HideInInspector] public bool BlockMenu;
    
    [HideInInspector] public bool IsAddingKeyBind;
    [HideInInspector] public KeyBindSetterController AddingKeyBindSetter;

    private EventBus eventBus;

    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SelectionStartedEvent>(OnSelectionStarted);
        eventBus.Subscribe<SelectionClearedEvent>(OnSelectionCleared);
    }

    private void OnSelectionStarted(SelectionStartedEvent evt) => BlockMenu = true;
    private void OnSelectionCleared(SelectionClearedEvent evt) => BlockMenu = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    
    private void Start() => ChangeMenuTab(CurrentMenuTab);
    
    public void ToggleMenu() => SetMenuVisible(!menuTween.IsVisible);
    
    public void SetMenuVisible(bool visible) => menuTween.SetVisible(visible);
    
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

    private void OnDestroy()
    {
        eventBus.Unsubscribe<SelectionStartedEvent>(OnSelectionStarted);
        eventBus.Unsubscribe<SelectionClearedEvent>(OnSelectionCleared);
    }
}