using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public enum MenuTab
    {
        GRAPHIC = 0,
        UI = 1,
        SOUND = 2
    }

    [Header("Constants & References")] public GameObject graphicSettingsUI;
    [FormerlySerializedAs("UISettingsUI")] public GameObject uiSettingsUI;
    public GameObject soundSettingsUI;
    [Space] [Header("Variables")] public MenuTab currentMenuTab;
    private MenuTab prevMenuTab;

    [HideInInspector] public bool blockMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        ChangeMenuTab(currentMenuTab);
    }

    public static void ExitGame()
    {
        PlayManager.QuitGame();
    }

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
            currentMenuTab = tab;
        }

        prevMenuTab = tab;
    }

    public void ChangeMenuTab(int tab)
    {
        ChangeMenuTab((MenuTab)tab);
    }

    public void ChangeMenuTab()
    {
        ChangeMenuTab(currentMenuTab);
    }

    public Dictionary<MenuTab, GameObject> GetTabDict()
    {
        // REF
        Dictionary<MenuTab, GameObject> dict = new()
        {
            { MenuTab.GRAPHIC, graphicSettingsUI },
            { MenuTab.SOUND, soundSettingsUI },
            { MenuTab.UI, uiSettingsUI }
        };
        return dict;
    }

    #endregion
}