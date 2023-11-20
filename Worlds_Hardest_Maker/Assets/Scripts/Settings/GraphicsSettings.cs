using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphicsSettings : MonoBehaviour
{
    public static GraphicsSettings Instance { get; private set; }

    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    #region Setting variables

    [HideInInspector] public int QualityLevel;

    public Resolution Resolution;

    [HideInInspector] public bool OneColorStartGoalCheckpoint;

    #endregion

    #region Graphics settings

    public void SetQuality(int index, bool updateDropdown)
    {
        QualitySettings.SetQualityLevel(index);

        print("s");

        Instance.QualityLevel = index;

        if (!updateDropdown) return;

        ReferenceManager.Instance.QualityDropdown.value = index;
    }

    public void SetQuality(int index) => SetQuality(index, false);

    public void Fullscreen(bool fullscreen, bool updateToggle) => Screen.fullScreen = fullscreen;

    // if (!updateToggle) return;
    public void Fullscreen(bool fullscreen) => Fullscreen(fullscreen, false);


    public void SetResolution(int index, bool updateDropdown)
    {
        if (index >= resolutions.Length) index = Instance.resolutions.Length - 1;
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        Resolution = res;

        if (!updateDropdown) return;
        ReferenceManager.Instance.ResolutionDropdown.value = index;
    }

    public void SetResolution(int index) => SetResolution(index, false);


    public void SetOneColorStartGoal(bool oneColor, bool updateToggle)
    {
        foreach (Transform field in ReferenceManager.Instance.FieldContainer)
        {
            FieldManager.ApplyStartGoalCheckpointFieldColor(field.gameObject, oneColor);
        }

        Instance.OneColorStartGoalCheckpoint = oneColor;

        if (!updateToggle) return;
        ReferenceManager.Instance.OneColorToggle.isOn = oneColor;
    }

    public void SetOneColorStartGoal(bool oneColor) => SetOneColorStartGoal(oneColor, false);

    #endregion

    private void UpdateResolutionOptions()
    {
        // dropdown for resolution: clear and fill in unity's resolutions options
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        List<string> options = new();

        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].ToString().Replace(" ", string.Empty));

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height) currentResIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
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