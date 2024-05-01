using MyBox;
using UnityEngine;

public class RoomOutlineGenerator : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private MapController map;
    [SerializeField] [InitializationField] [MustBeAssigned] private RoomOutline roomOutlinePrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Camera cam;
    
    private Vector2 prevPosition;
    
    private static bool EnabledInSettings => SettingsManager.Instance.ShowRoomGrid;
    
    
    private void Start()
    {
        PlayManager.Instance.OnPlaytest += Disable;
        PlayManager.Instance.OnSwitchToEdit += Enable;
        
        LevelSettings.Instance.OnUpdateRoomSize += CalcSize;
    }
    
    private void Update()
    {
        Vector2 camPosition = cam.transform.position;
        if (prevPosition != (Vector2)cam.transform.position)
        {
            transform.position = new(
                Mathf.Round(camPosition.x / LevelSettings.Instance.RoomWidth) * LevelSettings.Instance.RoomWidth,
                Mathf.Round(camPosition.y / LevelSettings.Instance.RoomHeight) * LevelSettings.Instance.RoomHeight
            );
        }
        
        prevPosition = camPosition;
    }
    
    private void CalcSize(float zoom)
    {
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        Transform t = transform;
        foreach (Transform child in t) Destroy(child.gameObject);
        
        float height = zoom;
        float width = height * cam.aspect;
        
        float minX = (Mathf.Ceil(-width / LevelSettings.Instance.RoomWidth) - 1) * LevelSettings.Instance.RoomWidth;
        float maxX = (Mathf.Ceil(width / LevelSettings.Instance.RoomWidth) + 1) * LevelSettings.Instance.RoomWidth;
        float minY = (Mathf.Ceil(-height / LevelSettings.Instance.RoomHeight) - 1) * LevelSettings.Instance.RoomHeight;
        float maxY = (Mathf.Ceil(height / LevelSettings.Instance.RoomHeight) + 1) * LevelSettings.Instance.RoomHeight;
        
        for (float i = minX; i < maxX; i += LevelSettings.Instance.RoomWidth)
        {
            for (float j = minY; j < maxY; j += LevelSettings.Instance.RoomHeight)
            {
                RoomOutline outline = Instantiate(
                    roomOutlinePrefab, new Vector3(i, j) + t.position,
                    Quaternion.identity, t
                );
                
                outline.SetDimensions(LevelSettings.Instance.RoomWidth, LevelSettings.Instance.RoomHeight);
            }
        }
    }
    
    public void CalcSize() => CalcSize(map.ZoomLimits.Max);
    
    private void OnDestroy()
    {
        PlayManager.Instance.OnPlaytest -= Disable;
        PlayManager.Instance.OnSwitchToEdit -= Enable;
        LevelSettings.Instance.OnUpdateRoomSize -= CalcSize;
    }
    
    private void Enable()
    {
        if (!EnabledInSettings) return;
        
        gameObject.SetActive(true);
    }
    
    private void Disable() => gameObject.SetActive(false);
    
    public void SetEnabledSetting(bool enabled)
    {
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        gameObject.SetActive(enabled && !(LevelSessionEditManager.Instance.Playing && LevelSessionEditManager.Instance.InPlaytest));
    }
}