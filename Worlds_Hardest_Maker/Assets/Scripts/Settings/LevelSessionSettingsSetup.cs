using MyBox;
using NaughtyAttributes;
using UnityEngine;
using VContainer;

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
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SetToolbarSizeEvent>(SetToolbarSize);
        eventBus.Subscribe<SetInfobarSizeEvent>(SetInfobarSize);
        eventBus.Subscribe<SetOneColorSafeFieldsEvent>(SetOneColorSafeFieldsWhenPlaying);
        eventBus.Subscribe<SetShowRoomGridEvent>(SetShowRoomGrid);
    }
    
    private void SetToolbarSize(SetToolbarSizeEvent evt)
    {
        if (toolbarSpacing == null) return;
        
        toolbarSpacing.ToolbarHeight = evt.Size;
        toolbarSpacing.UpdateSize();
    }
    
    private void SetInfobarSize(SetInfobarSizeEvent evt)
    {
        if (infobarPlayResize == null || infobarEditResize == null) return;
        
        infobarPlayResize.InfobarHeight = evt.Size;
        infobarPlayResize.UpdateSize();
        infobarEditResize.InfobarHeight = evt.Size;
        infobarEditResize.UpdateSize();
    }
    
    private void SetOneColorSafeFieldsWhenPlaying(SetOneColorSafeFieldsEvent evt)
    {
        FieldManager.ApplySafeFieldsColor(LevelSessionEditManager.Instance.Playing && evt.IsOneColor);
    }
    
    private void SetShowRoomGrid(SetShowRoomGridEvent evt)
    {
        roomOutlines.SetEnabledSetting(evt.ShowRoomGrid);
    }
    
    private void Start()
    {
        settingsManager.LoadPrefs();
        
        LevelSettings.Instance.OnLevelSettingsImported += roomOutlines.CalcSize;
    }
    
    private void OnDestroy()
    {
        LevelSettings.Instance.OnLevelSettingsImported -= roomOutlines.CalcSize;
        
        eventBus.Unsubscribe<SetToolbarSizeEvent>(SetToolbarSize);
        eventBus.Unsubscribe<SetInfobarSizeEvent>(SetInfobarSize);
        eventBus.Unsubscribe<SetOneColorSafeFieldsEvent>(SetOneColorSafeFieldsWhenPlaying);
        eventBus.Unsubscribe<SetShowRoomGridEvent>(SetShowRoomGrid);
    }
}