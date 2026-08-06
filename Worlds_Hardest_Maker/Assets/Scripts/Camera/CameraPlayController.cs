using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.PlayerRecording;

public class CameraPlayController : MonoBehaviour
{
    [SerializeField] private bool smoothMovement;
    [SerializeField] [MinValue(0)] [EnableIf(nameof(smoothMovement))] private float movementDuration;
    
    private Camera cam;
    private float camOrthoSize;
    
    private Vector2Int currentRoom;

    private IRecordingService recordingService;
    private EventBus eventBus;
    
    [Inject]
    private void Construct(IRecordingService recordingService, EventBus eventBus)
    {
        this.recordingService = recordingService;
        this.eventBus = eventBus;
        
        eventBus.Subscribe<StartPlaytestEvent>(OnStartPlaytest);
        eventBus.Subscribe<SetupPlaySceneEvent>(OnSetupPlayScene);
        eventBus.Subscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Subscribe<PathRenderUpdateEvent>(OnPathRenderUpdate);
    }
    
    private void OnStartPlaytest(StartPlaytestEvent evt) => JumpToStart();
    private void OnSetupPlayScene(SetupPlaySceneEvent evt) => JumpToStartInstant();
    private void OnResetLevel(ResetLevelEvent evt) => JumpToStart();
    private void OnPathRenderUpdate(PathRenderUpdateEvent evt)
    {
        if (recordingService.IsReplaying) TrackPosition(evt.Position.GetRoom());
    }
    
    private void Awake()
    {
        cam = GetComponent<Camera>();
        camOrthoSize = cam.orthographicSize;
    }
    
    private void Update()
    {
        if (!LevelSessionEditManager.Instance.InPlaytest || recordingService.IsReplaying) return;
        
        Vector2Int playerRoomPos = PlayerManager.GetCurrentRoom();
        TrackPosition(playerRoomPos);
    }
    
    private void TrackPosition(Vector2Int room)
    {
        if (currentRoom.x == room.x && currentRoom.y == room.y) return;
        
        currentRoom = room;
        JumpToRoom(currentRoom);
    }
    
    private void JumpToStart(bool instant)
    {
        // calculate zoom
        CameraPlayJumpInfo jumpInfo = CameraPlayJumpInfo.GetCurrentJumpInfo(cam);
        
        camOrthoSize = Mathf.Max(jumpInfo.WidthZoom, jumpInfo.HeightZoom);
        if (smoothMovement && !instant) cam.DOOrthoSize(camOrthoSize, movementDuration).SetEase(Ease.InOutCubic).SetUpdate(true);
        else cam.orthographicSize = camOrthoSize;
        
        currentRoom = PlayerManager.GetStartRoom();
        JumpToRoom(currentRoom, instant);
    }
    
    private void JumpToStart() => JumpToStart(false);
    private void JumpToStartInstant() => JumpToStart(true);
    
    private void JumpToRoom(Vector2Int cell, bool instant = false)
    {
        int roomWidth = LevelSettings.Instance.RoomWidth;
        int roomHeight = LevelSettings.Instance.RoomHeight;
        
        CameraPlayJumpInfo info = CameraPlayJumpInfo.GetCurrentJumpInfo(cam);
        
        float yOffset = info.HeightZoom > info.WidthZoom
            ? roomHeight * 0.5f - camOrthoSize
            : -CameraPlayJumpInfo.INFOBAR_HEIGHT * camOrthoSize / info.ScreenHeight;
        
        Transform t = transform;
        Vector3 newPosition = new(
            cell.x * roomWidth,
            cell.y * roomHeight + yOffset,
            t.position.z
        );
        
        // move
        if (smoothMovement && !instant) t.DOMove(newPosition, movementDuration).SetEase(Ease.InOutCubic).SetUpdate(true);
        else t.position = newPosition;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<StartPlaytestEvent>(OnStartPlaytest);
        eventBus.Unsubscribe<SetupPlaySceneEvent>(OnSetupPlayScene);
        eventBus.Unsubscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Unsubscribe<PathRenderUpdateEvent>(OnPathRenderUpdate);
    }
}

public sealed class CameraPlayJumpInfo
{
    public const float INFOBAR_HEIGHT = 70;
    public float WidthZoom;
    public float HeightZoom;
    public float ScreenWidth;
    public float ScreenHeight;
    
    public static CameraPlayJumpInfo GetCurrentJumpInfo(Camera cam)
    {
        Rect screen = ((RectTransform)ReferenceManager.Instance.Canvas.transform).rect;
        float screenWidth = screen.width;
        float screenHeight = screen.height;
        return new CameraPlayJumpInfo
        {
            WidthZoom = LevelSettings.Instance.RoomWidth * 0.5f / cam.aspect,
            HeightZoom = LevelSettings.Instance.RoomHeight * 0.5f / (1 - INFOBAR_HEIGHT / screenHeight),
            ScreenWidth = screenWidth,
            ScreenHeight = screenHeight,
        };
    }
}