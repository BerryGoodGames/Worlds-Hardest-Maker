using MyBox;
using UnityEngine;

/// <summary>
/// This script handles every action/configuration of the settings in LevelSession scene,
/// which enables SettingsManager to be scene-independent.
/// </summary>
public class LevelSessionSettingsSetup : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private SettingsManager settingsManager;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToolbarSizing toolbarSpacing;
    [SerializeField] [InitializationField] [MustBeAssigned] private InfobarResize infobarPlayResize;
    [SerializeField] [InitializationField] [MustBeAssigned] private InfobarResize infobarEditResize;

    private void SetToolbarSize(float size)
    {
        if (toolbarSpacing == null) return;
        
        toolbarSpacing.ToolbarHeight = size;
        toolbarSpacing.UpdateSize();
    }

    private void SetInfobarSize(float size)
    {
        if (infobarPlayResize == null || infobarEditResize == null) return;
        
        infobarPlayResize.InfobarHeight = size;
        infobarPlayResize.UpdateSize();
        infobarEditResize.InfobarHeight = size;
        infobarEditResize.UpdateSize();
    }

    private void SetOneColorSafeFieldsWhenPlaying(bool oneColor)
    {
        FieldManager.ApplySafeFieldsColor(LevelSessionEditManager.Instance.Playing && oneColor);
    }

    private void Start()
    {
        settingsManager.LoadPrefs();
    }

    private void Awake()
    {
        settingsManager.OnSetToolbarSize += SetToolbarSize;
        settingsManager.OnSetInfobarSize += SetInfobarSize;
        settingsManager.OnSetOneColorSafeFieldsWhenPlaying += SetOneColorSafeFieldsWhenPlaying;
    }

    private void OnDestroy()
    {
        settingsManager.OnSetInfobarSize -= SetToolbarSize;
        settingsManager.OnSetInfobarSize -= SetInfobarSize;
        settingsManager.OnSetOneColorSafeFieldsWhenPlaying -= SetOneColorSafeFieldsWhenPlaying;
    }
}
