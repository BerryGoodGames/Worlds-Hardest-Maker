using MyBox;
using UnityEngine;

public class PlayModeBlocker : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform cutout;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform blackScreen;

    private void Start()
    {
        Disable();
        
        PlayManager.Instance.OnSwitchToPlay += Enable;
        PlayManager.Instance.OnSwitchToEdit += Disable;
        
        SetupBlackScreenMask();
    }

    private void SetupBlackScreenMask()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        
        CameraPlayJumpInfo jumpInfo = CameraPlayJumpInfo.GetCurrentJumpInfo(cam);

        const float infobarHeight = CameraPlayJumpInfo.InfobarHeight;

        (float cutoutWidth, float cutoutHeight) = GetCutoutSize(jumpInfo);

        cutout.sizeDelta = new(cutoutWidth, cutoutHeight);
        cutout.anchoredPosition = new(0, infobarHeight / 2);

        blackScreen.sizeDelta = new(jumpInfo.ScreenWidth, jumpInfo.ScreenHeight);
        blackScreen.anchoredPosition = new(0, -infobarHeight / 2);
    }

    private static (float width, float height) GetCutoutSize(CameraPlayJumpInfo jumpInfo)
    {
        const float infobarHeight = CameraPlayJumpInfo.InfobarHeight;
        
        float cutoutWidth, cutoutHeight;

        if (jumpInfo.HeightZoom > jumpInfo.WidthZoom)
        {
            cutoutHeight = jumpInfo.ScreenHeight - infobarHeight;
            cutoutWidth = cutoutHeight * RoomOutlineGenerator.ROOM_WIDTH / RoomOutlineGenerator.ROOM_HEIGHT;
        }
        else
        {
            cutoutWidth = jumpInfo.ScreenWidth;
            cutoutHeight = cutoutWidth * RoomOutlineGenerator.ROOM_HEIGHT / RoomOutlineGenerator.ROOM_WIDTH;
        }

        return (cutoutWidth, cutoutHeight);
    }

    private void OnDestroy()
    {
        PlayManager.Instance.OnSwitchToPlay -= Enable;
        PlayManager.Instance.OnSwitchToEdit -= Disable;
    }
    
    private void Enable() => cutout.gameObject.SetActive(true);
    private void Disable() => cutout.gameObject.SetActive(false);
}
