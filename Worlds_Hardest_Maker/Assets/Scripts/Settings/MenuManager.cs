using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public enum MenuTab
    {
        Graphic = 0,
        UI = 1,
        Sound = 2
    }

    [Header("Constants & References")] public GameObject GraphicSettingsUI;

    public GameObject UISettingsUI;

    public GameObject SoundSettingsUI;

    [Space] [Header("Variables")] public MenuTab CurrentMenuTab;

    private MenuTab prevMenuTab;

    [HideInInspector] public bool BlockMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start() => ChangeMenuTab(CurrentMenuTab);

    public static void ExitGame() => PlayManager.QuitGame();

    #region Menu Tab

    public void ChangeMenuTab(MenuTab tab)
    {
        // REF
        if (tab != prevMenuTab)
        {
            Dictionary<MenuTab, GameObject> dict = GetTabDict();
            for (int i = 0; i < Enum.GetValues(typeof(MenuTab)).Length; i++)
            {
                dict[(MenuTab)i].SetActive(false);
            }

            dict[tab].SetActive(true);
            CurrentMenuTab = tab;
        }

        prevMenuTab = tab;
    }

    public void ChangeMenuTab(int tab) => ChangeMenuTab((MenuTab)tab);

    public void ChangeMenuTab() => ChangeMenuTab(CurrentMenuTab);

    public Dictionary<MenuTab, GameObject> GetTabDict()
    {
        // REF
        Dictionary<MenuTab, GameObject> dict = new()
        {
            { MenuTab.Graphic, GraphicSettingsUI },
            { MenuTab.Sound, SoundSettingsUI },
            { MenuTab.UI, UISettingsUI }
        };
        return dict;
    }

    #endregion
}