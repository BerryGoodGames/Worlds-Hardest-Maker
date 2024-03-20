using DG.Tweening;
using UnityEngine;

public class CameraPlayController : MonoBehaviour
{
    private void Start()
    {
        PlayManager.Instance.OnSwitchToPlay += JumpToStart;
    }

    private void JumpToStart()
    {
        Camera cam = GetComponent<Camera>();
        
        int roomWidth = RoomOutlineGenerator.ROOM_WIDTH;
        int roomHeight = RoomOutlineGenerator.ROOM_HEIGHT;
        
        // calculate zoom
        const float infobarHeight = 70;
        float screenHeight = ((RectTransform)ReferenceManager.Instance.Canvas.transform).rect.height;
        float heightZoom = roomHeight * 0.5f / (1 - infobarHeight / screenHeight);
        
        float widthZoom = roomWidth * 0.5f / cam.aspect;
        
        cam.orthographicSize = Mathf.Max(widthZoom, heightZoom);
        
        // calculate position
        Vector2 playerPos = PlayerManager.Instance.Player.StartPos;
        Vector2 playerRoomPos = new(
            Mathf.Round(playerPos.x / roomWidth),
            Mathf.Round(playerPos.y / roomHeight)
        );

        float yOffset = heightZoom > widthZoom
            ? roomHeight * 0.5f - cam.orthographicSize
            : -infobarHeight * cam.orthographicSize / screenHeight;
        
        Transform t = transform;
        t.position = new(
            playerRoomPos.x * roomWidth, 
            playerRoomPos.y * roomHeight + yOffset,
            t.position.z
        );
    }
    
    private void OnDestroy()
    {
        PlayManager.Instance.OnSwitchToPlay -= JumpToStart;
    }
}
