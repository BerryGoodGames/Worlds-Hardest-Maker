using DG.Tweening;
using MyBox;
using UnityEngine;

public class CameraPlayController : MonoBehaviour
{
    [SerializeField] private bool smoothMovement;
    [SerializeField] [PositiveValueOnly] [ConditionalField(nameof(smoothMovement))] private float movementDuration;

    private Camera cam;
    private float camOrthoSize;

    private Vector2Int currentRoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        camOrthoSize = cam.orthographicSize;
    }

    private void Start()
    {
        PlayManager.Instance.OnPlaytest += JumpToStart;
        PlayManager.Instance.OnPlaySceneSetup += JumpToStartInstant;
        PlayManager.Instance.OnLevelReset += JumpToStart;
    }

    private void Update()
    {
        if (!LevelSessionEditManager.Instance.InPlaytest) return;

        Vector2Int playerRoomPos = PlayerManager.GetCurrentRoom();
        if (currentRoom.x == playerRoomPos.x && currentRoom.y == playerRoomPos.y) return;

        currentRoom = PlayerManager.GetCurrentRoom();
        JumpToRoom(currentRoom);
    }

    private void JumpToStart(bool instant)
    {
        // calculate zoom
        CameraPlayJumpInfo jumpInfo = CameraPlayJumpInfo.GetCurrentJumpInfo(cam);

        camOrthoSize = Mathf.Max(jumpInfo.WidthZoom, jumpInfo.HeightZoom);
        if (smoothMovement && !instant) cam.DOOrthoSize(camOrthoSize, movementDuration).SetEase(Ease.InOutCubic);
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
            : -CameraPlayJumpInfo.InfobarHeight * camOrthoSize / info.ScreenHeight;

        Transform t = transform;
        Vector3 newPosition = new(
            cell.x * roomWidth,
            cell.y * roomHeight + yOffset,
            t.position.z
        );

        // move
        if (smoothMovement && !instant) t.DOMove(newPosition, movementDuration).SetEase(Ease.InOutCubic);
        else t.position = newPosition;
    }

    private void OnDestroy()
    {
        PlayManager.Instance.OnPlaytest -= JumpToStart;
        PlayManager.Instance.OnPlaySceneSetup -= JumpToStartInstant;
        PlayManager.Instance.OnLevelReset -= JumpToStart;
    }
}

public sealed class CameraPlayJumpInfo
{
    public const float InfobarHeight = 70;
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
            HeightZoom = LevelSettings.Instance.RoomHeight * 0.5f / (1 - InfobarHeight / screenHeight),
            ScreenWidth = screenWidth,
            ScreenHeight = screenHeight,
        };
    }
}