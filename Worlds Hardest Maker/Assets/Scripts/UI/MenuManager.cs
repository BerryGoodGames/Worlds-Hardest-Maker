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
        GRAPHIC = 0, UI = 2, SOUND = 1
    }
    [Header("Constants & References")]
    public GameObject graphicSettingsUI;
    public GameObject UISettingsUI;
    public GameObject soundSettingsUI;
    [Space]
    public TMPro.TMP_Dropdown resolutionDropdown;
    public AudioMixer mainMixer;
    public GameObject toolBar;
    public GameObject infobarEdit;
    public GameObject infobarPlay;
    public GameObject toolbarSizeSlider;
    public GameObject toolbarSizeInput;
    public GameObject infobarSizeSlider;
    public GameObject infobarSizeInput;
    [Space]
    [Header("Variables")]
    public MenuTab currentMenuTab;


    [HideInInspector] public Resolution[] resolutions;
    private ToolbarSpacing toolbarSpacing;
    private InfobarResize infobarPlayResize;
    private InfobarResize infobarEditResize;

    private void Awake()
    {
        toolbarSpacing = toolBar.GetComponent<ToolbarSpacing>();

        infobarPlayResize = infobarPlay.GetComponent<InfobarResize>();
        infobarEditResize = infobarEdit.GetComponent<InfobarResize>();

#if UNITY_EDITOR
        mainMixer.SetFloat("MusicVolume", -80);
        if(Instance != null)
        {
            Instance = this;
        }
#endif
    }

    private void Start()
    {
        

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        List<string> options = new();

        int currResIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].ToString().Replace(" ", string.Empty));

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currResIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void ChangeMenuTab(int tab)
    {
        Dictionary<MenuTab, GameObject> dict = GetTabDict();
        for (int i = 0; i < System.Enum.GetValues(typeof(MenuTab)).Length; i++)
        {
            dict[(MenuTab)i].SetActive(false);
        }
        dict[(MenuTab)tab].SetActive(true);
        currentMenuTab = (MenuTab)tab;
    }

    public Dictionary<MenuTab, GameObject> GetTabDict()
    {
        GameObject text = graphicSettingsUI;
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

    public static void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public static void Fullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetMusicVolume(float vol)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(vol) * 20);
    }

    public void SetSoundEffectVolume(float vol)
    {
        mainMixer.SetFloat("SoundEffectVolume", Mathf.Log10(vol) * 20);
    }

    public void SetToolbarSize(float size)
    {
        if(toolbarSpacing != null)
        {
            toolbarSpacing.toolbarHeight = size;
            toolbarSpacing.UpdateSize();
            if(toolbarSizeSlider.TryGetComponent(out Slider slider))
            {
                if(slider.value != size)
                {
                    slider.value = size;
                }
            }
            if(toolbarSizeInput.TryGetComponent(out TMPro.TMP_InputField input))
            {
                string conv = size.ToString();
                if(input.text != conv)
                {
                    Dbg.Text(size);
                    input.text = conv;
                }
            }
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
            if (infobarSizeSlider.TryGetComponent(out Slider slider))
            {
                if (slider.value != size)
                {
                    slider.value = size;
                }
            }
            if (infobarSizeInput.TryGetComponent(out TMPro.TMP_InputField input))
            {
                string conv = size.ToString();
                if (input.text != conv)
                {
                    Dbg.Text(size);
                    input.text = conv;
                }
            }
        }
    }
    public void SetInfobarSize(string size)
    {
        if (float.TryParse(size, out float conv)) SetInfobarSize(conv);
    }
}
