using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Update()
    {
        // right click drag to pan
        if (Input.GetMouseButton(KeybindManager.Instance.PanMouseButton))
        {
            if (lastMousePos == null)
            {
                // save mouse pos in first frame
                lastMousePos = Input.mousePosition;
            }
            else
            {
                // move camera the same amount as the mouse moved since the last frame
                Vector2 lastPos = (Vector2)lastMousePos;
                Vector2 currentMousePos = Input.mousePosition;

                Vector2 movement = lastPos - currentMousePos;
                movement = new(UnitPixelUtils.PixelToUnit(movement.x), UnitPixelUtils.PixelToUnit(movement.y));
                if (EventSystem.current.IsPointerOverGameObject()) movement = Vector2.zero;

                transform.position += (Vector3)movement;

                lastMousePos = currentMousePos;
            }
        }

        if (Input.GetMouseButtonUp(KeybindManager.Instance.PanMouseButton)) lastMousePos = null;

        float zoomInput = EventSystem.current.IsPointerOverGameObject() ? 0 : -Input.GetAxis("Mouse ScrollWheel");
        Zoom(zoomInput);
    }

    private void Zoom(float zoomInput)
    {
        if (zoomInput == 0f || !MouseManager.Instance.IsOnScreen) return; // zoom

        if (!(cam.orthographicSize + zoomInput * zoomSpeed >= ZoomLimits.Min) ||
            !(cam.orthographicSize + zoomInput * zoomSpeed <= ZoomLimits.Max)) return;

        Transform t = transform;

        Vector2 prevMousePos = MouseManager.Instance.MouseWorldPos;
        Vector2 prevMouseOffsetUnits = prevMousePos - (Vector2)t.position;
        Vector2 prevMouseOffsetPixels = UnitPixelUtils.UnitToPixel(prevMouseOffsetUnits);

        float newOrthoSize = cam.orthographicSize * (zoomInput * zoomSpeed + 1);
        if (newOrthoSize > ZoomLimits.Max) newOrthoSize = ZoomLimits.Max;
        if (newOrthoSize < ZoomLimits.Min) newOrthoSize = ZoomLimits.Min;

        Vector2 newMouseOffset = UnitPixelUtils.PixelToUnit(prevMouseOffsetPixels, newOrthoSize);
        Vector3 newCamPos = prevMousePos - newMouseOffset;

        // apply
        cam.DOKill();
        cam.DOOrthoSize(newOrthoSize, zoomAnimDuration);
        t.DOKill();
        t.DOMove(new Vector3(newCamPos.x, newCamPos.y, t.position.z), zoomAnimDuration);
    }

    private void Start() => cam = GetComponent<Camera>();
}