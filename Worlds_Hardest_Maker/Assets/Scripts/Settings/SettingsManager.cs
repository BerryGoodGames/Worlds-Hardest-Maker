using System;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private AudioMixer mainMixer;
    [SerializeField] [InitializationField] [MustBeAssigned] private SyncInputToSlider musicSlider;
    [SerializeField] [InitializationField] [MustBeAssigned] private SyncInputToSlider soundEffectSlider;
    [SerializeField] [InitializationField] [MustBeAssigned] private SyncInputToSlider toolbarSizeSlider;
    [SerializeField] [InitializationField] [MustBeAssigned] private SyncInputToSlider infobarSizeSlider;

    public event Action<float> OnSetToolbarSize = _ => { };
    public event Action<float> OnSetInfobarSize = _ => { };

    private void Start()
    {
        LoadPrefs();
    }

    public void SavePrefs()
    {
        print("Saving prefs...");
        
        PlayerPrefs.SetFloat("MusicVolume", GetMusicVolume());
        PlayerPrefs.SetFloat("SoundEffectVolume", GetSoundEffectVolume());
        PlayerPrefs.SetFloat("ToolbarSize", GetToolbarSize());
        PlayerPrefs.SetFloat("InfobarSize", GetInfobarSize());

        // graphics
        PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt("OneColorStartGoal", GraphicsSettings.Instance.OneColorSafeFields ? 1 : 0);

        // key binds
        foreach (KeyBind keyBind in KeyBinds.GetAllKeyBinds()) PlayerPrefs.SetString(keyBind.Name, keyBind.KeyCodesToString());

        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        print("Loading prefs...");
        
        // check if preferences already exist, and if they don't then set the current (default) prefs
        if (!PlayerPrefs.HasKey("MusicVolume")) SavePrefs();

        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"), true);
        SetSoundEffectVolume(PlayerPrefs.GetFloat("SoundEffectVolume"), true);
        SetToolbarSize(PlayerPrefs.GetFloat("ToolbarSize"), true);
        SetInfobarSize(PlayerPrefs.GetFloat("InfobarSize"), true);

        // graphics
        GraphicsSettings.Instance.Fullscreen(PlayerPrefs.GetInt("Fullscreen") == 1, true);
        GraphicsSettings.Instance.SetOneColorSafeFieldsWhenPlaying(PlayerPrefs.GetInt("OneColorStartGoal") == 1, true);

        // key binds
        foreach (KeyBind keyBind in KeyBinds.GetAllKeyBinds())
        {
            // check if key bind exists (in case new are added)
            if (!PlayerPrefs.HasKey(keyBind.Name)) continue;

            // get and deserialize key codes
            KeyCode[][] keyCodes = KeyBinds.KeyCodesFromString(PlayerPrefs.GetString(keyBind.Name));
            KeyBinds.ResetKeyBind(keyBind.Name);
            KeyBinds.AddKeyCodesToKeyBind(keyBind.Name, keyCodes);
        }
    }
    
    

    #region Sound settings

    public void SetMusicVolume(float vol, bool updateSlider)
    {
        // map vol from 0 - 100 to 0.0001 - 1 and convert it so vol acts linear
        float newVol = MathF.Log10(vol.Map(0, 100, 0.0001f, 3)) * 20;

        mainMixer.SetFloat("MusicVolume", newVol);

        if (!updateSlider) return;

        musicSlider.Slider.value = vol;
        musicSlider.UpdateInput();
    }

    public void SetMusicVolume(float vol) => SetMusicVolume(vol, false);
    
    public float GetMusicVolume()
    {
        if (mainMixer.GetFloat("MusicVolume", out float value)) return MathF.Pow(10, value / 20).Map(0.0001f, 3, 0, 100);

        throw new("Failed to access music volume");
    }

    public void SetSoundEffectVolume(float vol, bool updateSlider)
    {
        float newVol = Mathf.Log10(vol.Map(0, 100, 0.0001f, 3)) * 20;

        mainMixer.SetFloat("SoundEffectVolume", newVol);

        if (!updateSlider) return;

        soundEffectSlider.Slider.value = vol;
        soundEffectSlider.UpdateInput();
    }

    public void SetSoundEffectVolume(float vol) => SetSoundEffectVolume(vol, false);

    public float GetSoundEffectVolume()
    {
        if (mainMixer.GetFloat("SoundEffectVolume", out float value)) return MathF.Pow(10, value / 20).Map(0.0001f, 3, 0, 100);

        throw new("Failed to access sound effect volume");
    }

    #endregion

    #region UI Settings

    public void SetToolbarSize(float size, bool updateSlider)
    {
        OnSetToolbarSize.Invoke(size);
        
        if (!updateSlider) return;

        toolbarSizeSlider.Slider.value = size;
        toolbarSizeSlider.UpdateInput();
    }

    public void SetToolbarSize(float size) => SetToolbarSize(size, false);

    public void SetToolbarSize(string size)
    {
        if (float.TryParse(size, out float conv)) SetToolbarSize(conv);
    }

    public float GetToolbarSize() => toolbarSizeSlider.GetCurrentSliderValue();

    public void SetInfobarSize(float size, bool updateSlider)
    {
        OnSetInfobarSize.Invoke(size);

        if (!updateSlider) return;

        infobarSizeSlider.Slider.value = size;
        infobarSizeSlider.UpdateInput();
    }

    public void SetInfobarSize(float size) => SetInfobarSize(size, false);

    public void SetInfobarSize(string size)
    {
        if (float.TryParse(size, out float conv)) SetInfobarSize(conv);
    }

    public float GetInfobarSize() => infobarSizeSlider.GetCurrentSliderValue();

    #endregion

    private void OnDestroy() => SavePrefs();
}