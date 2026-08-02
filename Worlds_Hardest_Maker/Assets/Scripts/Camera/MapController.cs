using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

/// <summary>
///     Controls map / camera movement
///     <para>Attach to main camera</para>
/// </summary>
public class MapController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 4f;
    
    public MinMaxFloat ZoomLimits;
    
    [SerializeField] private float zoomAnimDuration;
    
    private Vector2? lastMousePos;
    private Camera cam;

    [Inject] private IMouseService mouseService;
    
    private void Update()
    {
        if (LevelSessionEditManager.Instance.InPlaytest) return;
        
        // right click drag to pan
        if (KeyBinds.GetKeyBind("Camera_Pan")) PanCamera();
        if (KeyBinds.GetKeyBindUp("Camera_Pan")) lastMousePos = null;
        
        float zoomInput = EventSystem.current.IsPointerOverGameObject() ? 0 : -Input.GetAxis("Mouse ScrollWheel");
        Zoom(zoomInput);
    }
    
    private void Zoom(float zoomInput)
    {
        if (zoomInput == 0f || !mouseService.IsOnScreen) return; // zoom
        
        float minZoom = ZoomLimits.Min;
        float maxZoom = ZoomLimits.Max;
        float currentZoom = cam.orthographicSize;
        float newZoom = currentZoom + zoomInput * zoomSpeed;
        if ((newZoom < minZoom && currentZoom > minZoom) ||
            (newZoom > maxZoom && currentZoom < maxZoom)) return;
        
        Transform t = transform;
        
        Vector2 prevMousePos = mouseService.MouseWorldPos;
        Vector2 prevMouseOffsetUnits = prevMousePos - (Vector2)t.position;
        Vector2 prevMouseOffsetPixels = UnitPixelUtils.UnitToPixel(prevMouseOffsetUnits);
        
        float newOrthoSize = cam.orthographicSize * (zoomInput * zoomSpeed + 1);
        if (newOrthoSize > ZoomLimits.Max) newOrthoSize = ZoomLimits.Max;
        if (newOrthoSize < ZoomLimits.Min) newOrthoSize = ZoomLimits.Min;
        
        Vector2 newMouseOffset = UnitPixelUtils.PixelToUnit(prevMouseOffsetPixels, newOrthoSize);
        Vector3 newCamPos = prevMousePos - newMouseOffset;
        
        // apply
        cam.DOKill();
        cam.DOOrthoSize(newOrthoSize, zoomAnimDuration).SetUpdate(true);
        t.DOKill();
        t.DOMove(new(newCamPos.x, newCamPos.y, t.position.z), zoomAnimDuration).SetUpdate(true);
    }
    
    private void PanCamera()
    {
        if (lastMousePos != null)
        {
            // move camera the same amount as the mouse moved since the last frame
            Vector2 lastPos = (Vector2)lastMousePos;
            Vector2 currentMousePos = Input.mousePosition;
            
            Vector2 movement = lastPos - currentMousePos;
            movement = new(UnitPixelUtils.PixelToUnit(movement.x), UnitPixelUtils.PixelToUnit(movement.y));
            if (EventSystem.current.IsPointerOverGameObject()) movement = Vector2.zero;
            
            transform.position += (Vector3)movement;
        }
        
        lastMousePos = Input.mousePosition;
    }
    
    private void Start() => cam = GetComponent<Camera>();
}