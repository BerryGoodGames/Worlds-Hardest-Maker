using MyBox;
using UnityEngine;
using VContainer;

public class PlayModeBlocker : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform cutout;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform blackScreen;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<StartPlaytestEvent>(OnStartPlaytest);
        eventBus.Subscribe<SetupPlaySceneEvent>(OnSetupPlayScene);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
    
    private void OnStartPlaytest(StartPlaytestEvent evt) => Enable();
    private void OnSetupPlayScene(SetupPlaySceneEvent evt) => Enable();
    private void OnSwitchToEdit(SwitchToEditEvent evt) => Disable();

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
        eventBus.Unsubscribe<StartPlaytestEvent>(OnStartPlaytest);
        eventBus.Unsubscribe<SetupPlaySceneEvent>(OnSetupPlayScene);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        LevelSettings.Instance.OnLevelSettingsImported -= SetupBlackScreenMask;
        LevelSettings.Instance.OnUpdateRoomSize -= SetupBlackScreenMask;
    }
    
    private void Enable() => cutout.gameObject.SetActive(true);
    private void Disable() => cutout.gameObject.SetActive(false);
}