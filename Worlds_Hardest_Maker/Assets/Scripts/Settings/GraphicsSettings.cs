using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphicsSettings : MonoBehaviour
{
    public static GraphicsSettings Instance { get; private set; }

    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    #region Setting variables

    [HideInInspector] public int qualityLevel;
    [HideInInspector] public bool fullscreen;
    public Resolution resolution;

    [FormerlySerializedAs("oneColorStartGoal")] [HideInInspector]
    public bool oneColorStartGoalCheckpoint;

    #endregion

    #region Graphics settings

    public void SetQuality(int index, bool setPrefs)
    {
        QualitySettings.SetQualityLevel(index);

        Instance.qualityLevel = index;

        if (setPrefs) SettingsManager.Instance.SavePrefs();
    }

    public void SetQuality(int index)
    {
        SetQuality(index, true);
    }

    public void Fullscreen(bool fullscreen, bool setPrefs)
    {
        Screen.fullScreen = fullscreen;

        Instance.fullscreen = fullscreen;

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

        Instance.resolution = res;

        if (setPrefs) SettingsManager.Instance.SavePrefs();
    }

    public void SetResolution(int index)
    {
        SetResolution(index, true);
    }

    public void SetOneColorStartGoal(bool oneColor, bool setPrefs)
    {
        // REF
        foreach (Transform field in ReferenceManager.Instance.fieldContainer)
        {
            List<Color> colors = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors;
            if (oneColor)
            {
                // set start, goal, checkpoints fields unique color
                string[] tags = { "StartField", "GoalField", "CheckpointField" };
                foreach (string t in tags)
                {
                    if (!field.CompareTag(t)) continue;

                    field.GetComponent<SpriteRenderer>().color = colors[4];

                    if (field.TryGetComponent(out Animator anim))
                    {
                        anim.enabled = false;
                    }
                }
            }
            else
            {
                // set colorful colors to start, goal, checkpoints fields
                string[] tags = { "StartField", "GoalField" };

                for (int i = 0; i < tags.Length; i++)
                {
                    SpriteRenderer renderer = field.GetComponent<SpriteRenderer>();
                    if (field.CompareTag(tags[i]))
                    {
                        renderer.color = colors[i];
                    }
                    else if (field.CompareTag("CheckpointField"))
                    {
                        CheckpointController checkpoint = field.GetComponent<CheckpointController>();
                        Color checkpointUnactivated =
                            ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors[2];
                        Color checkpointActivated =
                            ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors[3];

                        renderer.color = checkpoint.activated ? checkpointActivated : checkpointUnactivated;

                        if (field.TryGetComponent(out Animator anim))
                        {
                            anim.enabled = true;
                        }
                    }
                }
            }
        }

        Instance.oneColorStartGoalCheckpoint = oneColor;

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

        resolutionDropdown.ClearOptions();
        List<string> options = new();

        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].ToString().Replace(" ", string.Empty));

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void Start()
    {
        UpdateResolutionOptions();

        Instance.qualityLevel = QualitySettings.GetQualityLevel();
        Instance.resolution = resolutions[0];
        Instance.fullscreen = false;
        Instance.oneColorStartGoalCheckpoint = false;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}