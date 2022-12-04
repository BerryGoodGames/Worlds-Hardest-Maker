using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public AudioMixer mainMixer;
    public GameObject toolbarContainer;
    public GameObject infobarEdit;
    public GameObject infobarPlay;

    private ToolbarSizing toolbarSpacing;
    private InfobarResize infobarPlayResize;
    private InfobarResize infobarEditResize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        toolbarSpacing = toolbarContainer.GetComponent<ToolbarSizing>();
        infobarPlayResize = infobarPlay.GetComponent<InfobarResize>();
        infobarEditResize = infobarEdit.GetComponent<InfobarResize>();

#if UNITY_EDITOR
        mainMixer.SetFloat("MusicVolume", -80);
        if (Instance != null) Instance = this;
#endif
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("MusicVolume", GetMusicVolume());
        PlayerPrefs.SetFloat("SoundEffectVolume", GetSoudEffectVolume());
        PlayerPrefs.SetFloat("ToolbarSize", GetToolbarSize());
        PlayerPrefs.SetFloat("InfobarSize", GetInfobarSize());

        // graphics
        PlayerPrefs.SetInt("Quality", GraphicsSettings.Instance.qualityLevel);
        PlayerPrefs.SetInt("Fullscreen", GraphicsSettings.Instance.fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("OneColorStartGoal", GraphicsSettings.Instance.oneColorStartGoal ? 1 : 0);

    }

    public void LoadPrefs()
    {
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"), false);
        SetSoundEffectVolume(PlayerPrefs.GetFloat("SoundEffectVolume"), false);
        SetToolbarSize(PlayerPrefs.GetFloat("ToolbarSize"), false);
        SetInfobarSize(PlayerPrefs.GetFloat("InfobarSize"), false);

        // graphics
        GraphicsSettings.Instance.SetQuality(PlayerPrefs.GetInt("Quality"), false);
        GraphicsSettings.Instance.Fullscreen(PlayerPrefs.GetInt("Fullscreen") == 1, false);
        GraphicsSettings.Instance.SetOneColorStartGoal(PlayerPrefs.GetInt("OneColorStartGoal") == 1, false);
    }

    #region Sound settings
    public void SetMusicVolume(float vol, bool setPrefs)
    {
        // map vol from 0 - 100 to 0.0001 - 1 and convert it so vol acts linear
        float newVol = Mathf.Log10((float)GameManager.Map(vol, 0, 100, 0.0001, 3)) * 20;

        mainMixer.SetFloat("MusicVolume", newVol);

        if (setPrefs) SavePrefs();
    }
    public void SetMusicVolume(float vol) => SetMusicVolume(vol, true);
    public void SetSoundEffectVolume(float vol, bool setPrefs)
    {
        float newVol = Mathf.Log10((float)GameManager.Map(vol, 0, 100, 0.0001, 3)) * 20;

        mainMixer.SetFloat("SoundEffectVolume", newVol);

        if (setPrefs) SavePrefs();
    }
    public void SetSoundEffectVolume(float vol) => SetSoundEffectVolume(vol, true);
    public float GetMusicVolume() 
    {
        if (mainMixer.GetFloat("MusicVolume", out float value))
        {
            return value;
        }
        throw new System.Exception("Failed to acces music volume");
    }
    public float GetSoudEffectVolume()
    {
        if (mainMixer.GetFloat("SoundEffectVolume", out float value))
        {
            return value;
        }
        throw new System.Exception("Failed to acces sound effect volume");
    }
    #endregion

    #region UI Settings
    public void SetToolbarSize(float size, bool setPrefs)
    {
        if (toolbarSpacing != null)
        {
            toolbarSpacing.toolbarHeight = size;
            toolbarSpacing.UpdateSize();

            if (setPrefs) SavePrefs();
        }
    }
    public void SetToolbarSize(string size, bool setPrefs)
    {
        if (float.TryParse(size, out float conv)) SetToolbarSize(conv, setPrefs);
    }
    public void SetToolbarSize(float size) => SetToolbarSize(size, true);
    public void SetToolbarSize(string size) => SetToolbarSize(size, true);

    public float GetToolbarSize() => toolbarSpacing.toolbarHeight;

    public void SetInfobarSize(float size, bool setPrefs)
    {
        if (infobarPlayResize != null && infobarEditResize != null)
        {
            infobarPlayResize.infobarHeight = size;
            infobarEditResize.infobarHeight = size;
            infobarPlayResize.UpdateSize();
            infobarEditResize.UpdateSize();

            if(setPrefs) SavePrefs();
        }
    }
    public void SetInfobarSize(string size, bool setPrefs)
    {
        if (float.TryParse(size, out float conv)) SetInfobarSize(conv, setPrefs);
    }
    public void SetInfobarSize(float size) => SetInfobarSize(size, true);
    public void SetInfobarSize(string size) => SetInfobarSize(size, true);

    public float GetInfobarSize() => infobarPlayResize.infobarHeight;
    #endregion
}
