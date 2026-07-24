using MyBox;
using NaughtyAttributes;
using UnityEngine;
using VContainer;

public class RoomOutlineGenerator : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private MapController map;
    [SerializeField] [InitializationField] [Required] private RoomOutline roomOutlinePrefab;
    [SerializeField] [InitializationField] [Required] private Camera cam;
    
    private Vector2 prevPosition;
    
    private bool hasInitiallyCalculated;
    
    private static bool EnabledInSettings => SettingsManager.Instance.ShowRoomGrid;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<StartPlaytestEvent>(OnPlaytest);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
    
    private void Start()
    {
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
        
        hasInitiallyCalculated = true;
    }
    
    public void CalcSize() => CalcSize(map.ZoomLimits.Max);
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<StartPlaytestEvent>(OnPlaytest);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        LevelSettings.Instance.OnUpdateRoomSize -= CalcSize;
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        if (!EnabledInSettings) return;
        
        SetActive(true);
    }
    
    private void OnPlaytest(StartPlaytestEvent evt) => SetActive(false);
    
    public void SetEnabledSetting(bool enabled)
    {
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        SetActive(enabled && !(LevelSessionEditManager.Instance.Playing && LevelSessionEditManager.Instance.InPlaytest));
    }
    
    private void SetActive(bool active) => gameObject.SetActive(active);
    
    private void OnEnable()
    {
        if (!hasInitiallyCalculated) CalcSize();
    }
}