using MyBox;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
///     This script handles every action/configuration of the settings in LevelSession scene,
///     which enables SettingsManager to be scene-independent.
/// </summary>
public class LevelSessionSettingsSetup : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private SettingsManager settingsManager;
    [SerializeField] [InitializationField] [Required] private ToolbarSizing toolbarSpacing;
    [SerializeField] [InitializationField] [Required] private InfobarResize infobarPlayResize;
    [SerializeField] [InitializationField] [Required] private InfobarResize infobarEditResize;
    [SerializeField] [InitializationField] [Required] private RoomOutlineGenerator roomOutlines;
    
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
    
    private void SetOneColorSafeFieldsWhenPlaying(bool oneColor) =>
        FieldManager.ApplySafeFieldsColor(LevelSessionEditManager.Instance.Playing && oneColor);
    
    private void SetShowRoomGrid(bool show) => roomOutlines.SetEnabledSetting(show);
    
    private void Start()
    {
        settingsManager.LoadPrefs();
        
        LevelSettings.Instance.OnLevelSettingsImported += roomOutlines.CalcSize;
    }
    
    private void Awake()
    {
        settingsManager.OnSetToolbarSize += SetToolbarSize;
        settingsManager.OnSetInfobarSize += SetInfobarSize;
        settingsManager.OnSetOneColorSafeFieldsWhenPlaying += SetOneColorSafeFieldsWhenPlaying;
        settingsManager.OnSetShowRoomGrid += SetShowRoomGrid;
    }
    
    private void OnDestroy()
    {
        LevelSettings.Instance.OnLevelSettingsImported -= roomOutlines.CalcSize;
        settingsManager.OnSetInfobarSize -= SetToolbarSize;
        settingsManager.OnSetInfobarSize -= SetInfobarSize;
        settingsManager.OnSetOneColorSafeFieldsWhenPlaying -= SetOneColorSafeFieldsWhenPlaying;
        settingsManager.OnSetShowRoomGrid -= SetShowRoomGrid;
    }
}