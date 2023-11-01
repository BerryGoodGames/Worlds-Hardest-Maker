using System;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public AudioMixer MainMixer;

    public GameObject ToolbarContainer;

    public GameObject InfobarEdit;
    public GameObject InfobarPlay;

    private ToolbarSizing toolbarSpacing;
    private InfobarResize infobarPlayResize;
    private InfobarResize infobarEditResize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        toolbarSpacing = ToolbarContainer.GetComponent<ToolbarSizing>();
        infobarPlayResize = InfobarPlay.GetComponent<InfobarResize>();
        infobarEditResize = InfobarEdit.GetComponent<InfobarResize>();
    }

    private void Start() => Instance.LoadPrefs();

    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("MusicVolume", GetMusicVolume());
        PlayerPrefs.SetFloat("SoundEffectVolume", GetSoundEffectVolume());
        PlayerPrefs.SetFloat("ToolbarSize", GetToolbarSize());
        PlayerPrefs.SetFloat("InfobarSize", GetInfobarSize());

        // graphics
        PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt("OneColorStartGoal", GraphicsSettings.Instance.OneColorStartGoalCheckpoint ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        // check if preferences already exist, and if they don't then set the current (default) prefs
        if (!PlayerPrefs.HasKey("MusicVolume")) SavePrefs();

        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"), true);
        SetSoundEffectVolume(PlayerPrefs.GetFloat("SoundEffectVolume"), true);
        SetToolbarSize(PlayerPrefs.GetFloat("ToolbarSize"), true);
        SetInfobarSize(PlayerPrefs.GetFloat("InfobarSize"), true);

        // graphics
        GraphicsSettings.Instance.Fullscreen(PlayerPrefs.GetInt("Fullscreen") == 1, true);
        GraphicsSettings.Instance.SetOneColorStartGoal(PlayerPrefs.GetInt("OneColorStartGoal") == 1, true);
    }

    #region Sound settings

    public void SetMusicVolume(float vol, bool updateSlider)
    {
        // map vol from 0 - 100 to 0.0001 - 1 and convert it so vol acts linear
        float newVol = MathF.Log10(vol.Map(0, 100, 0.0001f, 3)) * 20;

        MainMixer.SetFloat("MusicVolume", newVol);

        if (!updateSlider) return;

        ReferenceManager.Instance.MusicSlider.Slider.value = vol;
        ReferenceManager.Instance.MusicSlider.UpdateInput();
    }

    public void SetMusicVolume(float vol) => SetMusicVolume(vol, false);


    public void SetSoundEffectVolume(float vol, bool updateSlider)
    {
        float newVol = Mathf.Log10(vol.Map(0, 100, 0.0001f, 3)) * 20;

        MainMixer.SetFloat("SoundEffectVolume", newVol);

        if (!updateSlider) return;

        ReferenceManager.Instance.SoundEffectSlider.Slider.value = vol;
        ReferenceManager.Instance.SoundEffectSlider.UpdateInput();
    }

    public void SetSoundEffectVolume(float vol) => SetSoundEffectVolume(vol, false);

    public float GetMusicVolume()
    {
        if (MainMixer.GetFloat("MusicVolume", out float value)) return MathF.Pow(10, value / 20).Map(0.0001f, 3, 0, 100);

        throw new("Failed to access music volume");
    }

    public float GetSoundEffectVolume()
    {
        if (MainMixer.GetFloat("SoundEffectVolume", out float value)) return MathF.Pow(10, value / 20).Map(0.0001f, 3, 0, 100);

        throw new("Failed to access sound effect volume");
    }

    #endregion

    #region UI Settings

    public void SetToolbarSize(float size, bool updateSlider)
    {
        if (toolbarSpacing == null) return;

        toolbarSpacing.ToolbarHeight = size;
        toolbarSpacing.UpdateSize();

        if (!updateSlider) return;

        ReferenceManager.Instance.ToolbarSizeSlider.Slider.value = size;
        ReferenceManager.Instance.ToolbarSizeSlider.UpdateInput();
    }

    public void SetToolbarSize(float size) => SetToolbarSize(size, false);

    public void SetToolbarSize(string size)
    {
        if (float.TryParse(size, out float conv)) SetToolbarSize(conv);
    }

    public float GetToolbarSize() => toolbarSpacing.ToolbarHeight;

    public void SetInfobarSize(float size, bool updateSlider)
    {
        if (infobarPlayResize == null || infobarEditResize == null) return;

        infobarPlayResize.InfobarHeight = size;
        infobarEditResize.InfobarHeight = size;
        infobarPlayResize.UpdateSize();
        infobarEditResize.UpdateSize();

        if (!updateSlider) return;

        ReferenceManager.Instance.InfobarSizeSlider.Slider.value = size;
        ReferenceManager.Instance.InfobarSizeSlider.UpdateInput();
    }

    public void SetInfobarSize(float size) => SetInfobarSize(size, false);

    public void SetInfobarSize(string size)
    {
        if (float.TryParse(size, out float conv)) SetInfobarSize(conv);
    }

    public float GetInfobarSize() => infobarPlayResize.InfobarHeight;

    #endregion

    private void OnDestroy() => SavePrefs();
}