using MyBox;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class PlayModeBlocker : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private RectTransform cutout;
    [SerializeField] [InitializationField] [Required] private RectTransform blackScreen;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<StartPlaytestEvent>(_ => Enable());
        eventBus.Subscribe<SetupPlaySceneEvent>(_ => Enable());
        eventBus.Subscribe<SwitchToEditEvent>(_ => Disable());
    }

    private void Start()
    {
        Disable();
        
        LevelSettings.Instance.OnLevelSettingsImported += SetupBlackScreenMask;
        LevelSettings.Instance.OnUpdateRoomSize += SetupBlackScreenMask;
        
        SetupBlackScreenMask();
    }
    
    public void SetupBlackScreenMask()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        
        CameraPlayJumpInfo jumpInfo = CameraPlayJumpInfo.GetCurrentJumpInfo(cam);
        
        const float INFOBAR_HEIGHT = CameraPlayJumpInfo.INFOBAR_HEIGHT;
        
        (float cutoutWidth, float cutoutHeight) = GetCutoutSize(jumpInfo);
        
        cutout.sizeDelta = new(cutoutWidth, cutoutHeight);
        cutout.anchoredPosition = new(0, INFOBAR_HEIGHT / 2);
        
        blackScreen.sizeDelta = new(jumpInfo.ScreenWidth, jumpInfo.ScreenHeight);
        blackScreen.anchoredPosition = new(0, -INFOBAR_HEIGHT / 2);
    }
    
    private static (float width, float height) GetCutoutSize(CameraPlayJumpInfo jumpInfo)
    {
        const float INFOBAR_HEIGHT = CameraPlayJumpInfo.INFOBAR_HEIGHT;
        
        float cutoutWidth, cutoutHeight;
        
        if (jumpInfo.HeightZoom > jumpInfo.WidthZoom)
        {
            cutoutHeight = jumpInfo.ScreenHeight - INFOBAR_HEIGHT;
            cutoutWidth = cutoutHeight * LevelSettings.Instance.RoomWidth / LevelSettings.Instance.RoomHeight;
        }
        else
        {
            cutoutWidth = jumpInfo.ScreenWidth;
            cutoutHeight = cutoutWidth * LevelSettings.Instance.RoomHeight / LevelSettings.Instance.RoomWidth;
        }
        
        return (cutoutWidth, cutoutHeight);
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<StartPlaytestEvent>(_ => Enable());
        eventBus.Unsubscribe<SetupPlaySceneEvent>(_ => Enable());
        eventBus.Unsubscribe<SwitchToEditEvent>(_ => Disable());
        LevelSettings.Instance.OnLevelSettingsImported -= SetupBlackScreenMask;
        LevelSettings.Instance.OnUpdateRoomSize -= SetupBlackScreenMask;
    }
    
    private void Enable() => cutout.gameObject.SetActive(true);
    private void Disable() => cutout.gameObject.SetActive(false);
}