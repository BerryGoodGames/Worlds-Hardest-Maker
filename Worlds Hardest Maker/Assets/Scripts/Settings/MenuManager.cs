using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    public enum MenuTab
    {
        GRAPHIC = 0, UI = 1, SOUND = 2
    }
    [Header("Constants & References")]
    public GameObject graphicSettingsUI;
    public GameObject UISettingsUI;
    public GameObject soundSettingsUI;
    [Space]
    public AudioMixer mainMixer;
    public GameObject toolBar;
    public GameObject infobarEdit;
    public GameObject infobarPlay;
    [Space]
    [Header("Variables")]
    public MenuTab currentMenuTab;
    private MenuTab prevMenuTab;

    private ToolbarSizing toolbarSpacing;
    private InfobarResize infobarPlayResize;
    private InfobarResize infobarEditResize;

    private void Awake()
    {
        toolbarSpacing = toolBar.GetComponent<ToolbarSizing>();

        infobarPlayResize = infobarPlay.GetComponent<InfobarResize>();
        infobarEditResize = infobarEdit.GetComponent<InfobarResize>();

        prevMenuTab = currentMenuTab;

#if UNITY_EDITOR
        mainMixer.SetFloat("MusicVolume", -80);
        if(Instance != null)
        {
            Instance = this;
        }
#endif
    }

    public void ChangeMenuTab(int tab)
    {
        // REF
        MenuTab newTab = (MenuTab)tab;

        if(newTab != prevMenuTab)
        {
            Dictionary<MenuTab, GameObject> dict = GetTabDict();
            for (int i = 0; i < System.Enum.GetValues(typeof(MenuTab)).Length; i++)
            {
                dict[(MenuTab)i].SetActive(false);
            }
            dict[newTab].SetActive(true);
            currentMenuTab = (MenuTab)tab;
        }
        prevMenuTab = newTab;
    }

    public Dictionary<MenuTab, GameObject> GetTabDict()
    {
        // REF
        Dictionary<MenuTab, GameObject> dict = new()
        {
            { MenuTab.GRAPHIC, graphicSettingsUI },
            { MenuTab.SOUND, soundSettingsUI } ,
            { MenuTab.UI, UISettingsUI }
        };
        return dict;
    }
    public static void ExitGame()
    {
        Application.Quit();
    }    

    #region SOUND SETTINGS
    public void SetMusicVolume(float vol)
    {
        // map vol from 0 - 100 to 0.0001 - 1 and convert it so vol acts linear
        float newVol = Mathf.Log10((float)GameManager.Map(vol, 0, 100, 0.0001, 3)) * 20;

        mainMixer.SetFloat("MusicVolume", newVol);
    }

    public void SetSoundEffectVolume(float vol)
    {
        float newVol = Mathf.Log10((float)GameManager.Map(vol, 0, 100, 0.0001, 3)) * 20;
        print(newVol);
        mainMixer.SetFloat("SoundEffectVolume", newVol);
    }
    #endregion

    #region UI SETTINGS
    public void SetToolbarSize(float size)
    {
        if(toolbarSpacing != null)
        {
            toolbarSpacing.toolbarHeight = size;
            toolbarSpacing.UpdateSize();
        }
    }

    public void SetToolbarSize(string size) 
    {
        if (float.TryParse(size, out float conv)) SetToolbarSize(conv);
    }

    public void SetInfobarSize(float size)
    {
        if (infobarPlayResize != null && infobarEditResize != null)
        {
            infobarPlayResize.infobarHeight = size;
            infobarEditResize.infobarHeight = size;
            infobarPlayResize.UpdateSize();
            infobarEditResize.UpdateSize();
        }
    }
    public void SetInfobarSize(string size)
    {
        if (float.TryParse(size, out float conv)) SetInfobarSize(conv);
    }
    #endregion
}
