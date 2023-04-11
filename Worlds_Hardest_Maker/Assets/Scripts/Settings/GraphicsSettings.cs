using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphicsSettings : MonoBehaviour
{
    public static GraphicsSettings Instance { get; private set; }

    [FormerlySerializedAs("resolutionDropdown")]
    public TMP_Dropdown ResolutionDropdown;

    private Resolution[] resolutions;

    #region Setting variables

    [FormerlySerializedAs("qualityLevel")] [HideInInspector]
    public int QualityLevel;

    public Resolution Resolution;

    [FormerlySerializedAs("oneColorStartGoalCheckpoint")] [FormerlySerializedAs("oneColorStartGoal")] [HideInInspector]
    public bool OneColorStartGoalCheckpoint;

    #endregion

    #region Graphics settings

    public void SetQuality(int index, bool setPrefs)
    {
        QualitySettings.SetQualityLevel(index);

        Instance.QualityLevel = index;

        if (setPrefs) SettingsManager.Instance.SavePrefs();
    }

    public void SetQuality(int index)
    {
        SetQuality(index, true);
    }

    public void Fullscreen(bool fullscreen, bool setPrefs)
    {
        Screen.fullScreen = fullscreen;

        if (setPrefs) SettingsManager.Instance.SavePrefs();
    }

    public void Fullscreen(bool fullscreen)
    {
        Fullscreen(fullscreen, true);
    }

    public void SetResolution(int index, bool setPrefs)
    {
        Resolution res = Instance.resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        Instance.Resolution = res;

        if (setPrefs) SettingsManager.Instance.SavePrefs();
    }

    public void SetResolution(int index)
    {
        SetResolution(index, true);
    }

    public void SetOneColorStartGoal(bool oneColor, bool setPrefs)
    {
        foreach (Transform field in ReferenceManager.Instance.FieldContainer)
            FieldManager.ApplyStartGoalCheckpointFieldColor(field.gameObject, oneColor);

        Instance.OneColorStartGoalCheckpoint = oneColor;

        if (setPrefs) SettingsManager.Instance.SavePrefs();
    }

    public void SetOneColorStartGoal(bool oneColor)
    {
        SetOneColorStartGoal(oneColor, true);
    }

    #endregion

    private void UpdateResolutionOptions()
    {
        // dropdown for resolution: clear and fill in unity's resolutions options
        resolutions = Screen.resolutions;

        ResolutionDropdown.ClearOptions();
        List<string> options = new();

        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].ToString().Replace(" ", string.Empty));

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResIndex = i;
        }

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = currentResIndex;
        ResolutionDropdown.RefreshShownValue();
    }

    private void Start()
    {
        UpdateResolutionOptions();

        Instance.QualityLevel = QualitySettings.GetQualityLevel();
        Instance.Resolution = resolutions[0];
        Instance.OneColorStartGoalCheckpoint = false;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}