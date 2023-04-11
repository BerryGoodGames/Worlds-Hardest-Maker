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

    [FormerlySerializedAs("graphicSettingsUI")] [Header("Constants & References")]
    public GameObject GraphicSettingsUI;

    [FormerlySerializedAs("uiSettingsUI")] public GameObject UISettingsUI;

    [FormerlySerializedAs("soundSettingsUI")]
    public GameObject SoundSettingsUI;

    [FormerlySerializedAs("currentMenuTab")] [Space] [Header("Variables")]
    public MenuTab CurrentMenuTab;

    private MenuTab prevMenuTab;

    [FormerlySerializedAs("blockMenu")] [HideInInspector]
    public bool BlockMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        ChangeMenuTab(CurrentMenuTab);
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
            for (int i = 0; i < Enum.GetValues(typeof(MenuTab)).Length; i++) dict[(MenuTab)i].SetActive(false);

            dict[tab].SetActive(true);
            CurrentMenuTab = tab;
        }

        prevMenuTab = tab;
    }

    public void ChangeMenuTab(int tab)
    {
        ChangeMenuTab((MenuTab)tab);
    }

    public void ChangeMenuTab()
    {
        ChangeMenuTab(CurrentMenuTab);
    }

    public Dictionary<MenuTab, GameObject> GetTabDict()
    {
        // REF
        Dictionary<MenuTab, GameObject> dict = new()
        {
            { MenuTab.GRAPHIC, GraphicSettingsUI },
            { MenuTab.SOUND, SoundSettingsUI },
            { MenuTab.UI, UISettingsUI }
        };
        return dict;
    }

    #endregion
}