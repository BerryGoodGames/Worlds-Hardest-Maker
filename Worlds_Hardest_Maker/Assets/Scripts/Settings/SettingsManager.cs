using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [FormerlySerializedAs("mainMixer")] public AudioMixer MainMixer;

    [FormerlySerializedAs("toolbarContainer")]
    public GameObject ToolbarContainer;

    [FormerlySerializedAs("infobarEdit")] public GameObject InfobarEdit;
    [FormerlySerializedAs("infobarPlay")] public GameObject InfobarPlay;

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

#if UNITY_EDITOR
        MainMixer.SetFloat("MusicVolume", -80);
        if (Instance != null) Instance = this;
#endif
    }

    private void Start()
    {
#if UNITY_EDITOR
        Instance.SetMusicVolume(0.0001f);
        Instance.LoadPrefs();
#endif
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("MusicVolume", GetMusicVolume());
        PlayerPrefs.SetFloat("SoundEffectVolume", GetSoundEffectVolume());
        PlayerPrefs.SetFloat("ToolbarSize", GetToolbarSize());
        PlayerPrefs.SetFloat("InfobarSize", GetInfobarSize());

        // graphics
        PlayerPrefs.SetInt("Quality", GraphicsSettings.Instance.QualityLevel);
        PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt("OneColorStartGoal", GraphicsSettings.Instance.OneColorStartGoalCheckpoint ? 1 : 0);
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
        float newVol = Mathf.Log10(vol.Map(0, 100, 0.0001f, 3)) * 20;

        MainMixer.SetFloat("MusicVolume", newVol);

        if (setPrefs) SavePrefs();
    }

    public void SetMusicVolume(float vol)
    {
        SetMusicVolume(vol, true);
    }

    public void SetSoundEffectVolume(float vol, bool setPrefs)
    {
        float newVol = Mathf.Log10(vol.Map(0, 100, 0.0001f, 3)) * 20;

        MainMixer.SetFloat("SoundEffectVolume", newVol);

        if (setPrefs) SavePrefs();
    }

    public void SetSoundEffectVolume(float vol)
    {
        SetSoundEffectVolume(vol, true);
    }

    public float GetMusicVolume()
    {
        if (MainMixer.GetFloat("MusicVolume", out float value)) return value;

        throw new Exception("Failed to access music volume");
    }

    public float GetSoundEffectVolume()
    {
        if (MainMixer.GetFloat("SoundEffectVolume", out float value)) return value;

        throw new Exception("Failed to access sound effect volume");
    }

    #endregion

    #region UI Settings

    public void SetToolbarSize(float size, bool setPrefs)
    {
        if (toolbarSpacing == null) return;

        toolbarSpacing.ToolbarHeight = size;
        toolbarSpacing.UpdateSize();

        if (setPrefs) SavePrefs();
    }

    public void SetToolbarSize(string size, bool setPrefs)
    {
        if (float.TryParse(size, out float conv)) SetToolbarSize(conv, setPrefs);
    }

    public void SetToolbarSize(float size)
    {
        SetToolbarSize(size, true);
    }

    public void SetToolbarSize(string size)
    {
        SetToolbarSize(size, true);
    }

    public float GetToolbarSize() => toolbarSpacing.ToolbarHeight;

    public void SetInfobarSize(float size, bool setPrefs)
    {
        if (infobarPlayResize == null || infobarEditResize == null) return;

        infobarPlayResize.InfobarHeight = size;
        infobarEditResize.InfobarHeight = size;
        infobarPlayResize.UpdateSize();
        infobarEditResize.UpdateSize();

        if (setPrefs) SavePrefs();
    }

    public void SetInfobarSize(string size, bool setPrefs)
    {
        if (float.TryParse(size, out float conv)) SetInfobarSize(conv, setPrefs);
    }

    public void SetInfobarSize(float size)
    {
        SetInfobarSize(size, true);
    }

    public void SetInfobarSize(string size)
    {
        SetInfobarSize(size, true);
    }

    public float GetInfobarSize() => infobarPlayResize.InfobarHeight;

    #endregion
}