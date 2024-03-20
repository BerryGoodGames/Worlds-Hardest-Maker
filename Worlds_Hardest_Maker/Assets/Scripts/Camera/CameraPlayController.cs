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

    private void Start() => PlayManager.Instance.OnSwitchToPlay += JumpToStart;

    private void Update()
    {
        if (LevelSessionEditManager.Instance.Editing) return;
        
        Vector2Int playerRoomPos = PlayerManager.Instance.Player.GetCurrentRoom();
        if (currentRoom.x == playerRoomPos.x && currentRoom.y == playerRoomPos.y) return;
        
        currentRoom = PlayerManager.Instance.Player.GetCurrentRoom();
        JumpToRoom(currentRoom);
    }
    
    private void JumpToStart()
    {
        // calculate zoom
        JumpInfo jumpInfo = GetCurrentJumpInfo();
        
        camOrthoSize = Mathf.Max(jumpInfo.WidthZoom, jumpInfo.HeightZoom);
        if (smoothMovement) cam.DOOrthoSize(camOrthoSize, movementDuration).SetEase(Ease.InOutCubic);
        else cam.orthographicSize = camOrthoSize;
        
        currentRoom = PlayerManager.Instance.Player.GetStartRoom();
        JumpToRoom(currentRoom);
    }

    private void JumpToRoom(Vector2Int cell)
    {
        const int roomWidth = RoomOutlineGenerator.ROOM_WIDTH;
        const int roomHeight = RoomOutlineGenerator.ROOM_HEIGHT;
        
        JumpInfo jumpInfo = GetCurrentJumpInfo();
        
        float yOffset = jumpInfo.HeightZoom > jumpInfo.WidthZoom
            ? roomHeight * 0.5f - camOrthoSize
            : -JumpInfo.InfobarHeight * camOrthoSize / jumpInfo.ScreenHeight;
        
        Transform t = transform;
        Vector3 newPosition = new(
            cell.x * roomWidth,
            cell.y * roomHeight + yOffset,
            t.position.z
        );
        
        // move
        if (smoothMovement) t.DOMove(newPosition, movementDuration).SetEase(Ease.InOutCubic);
        else t.position = newPosition;
    }

    private JumpInfo GetCurrentJumpInfo()
    {
        float screenHeight = ((RectTransform)ReferenceManager.Instance.Canvas.transform).rect.height;
        return new JumpInfo
        {
            WidthZoom = RoomOutlineGenerator.ROOM_WIDTH * 0.5f / cam.aspect,
            HeightZoom = RoomOutlineGenerator.ROOM_HEIGHT * 0.5f / (1 - JumpInfo.InfobarHeight / screenHeight),
            ScreenHeight = screenHeight,
        };
    }
    
    private void OnDestroy()
    {
        PlayManager.Instance.OnSwitchToPlay -= JumpToStart;
    }

    private class JumpInfo
    {
        public const float InfobarHeight = 70;
        public float WidthZoom;
        public float HeightZoom;
        public float ScreenHeight;
    }
}
