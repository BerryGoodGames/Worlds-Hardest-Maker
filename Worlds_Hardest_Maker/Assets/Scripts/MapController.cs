using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     controls map / camera movement
///     attach to main camera
/// </summary>
public class MapController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 4f;

    public float ZoomSpeed
    {
        get => zoomSpeed;
        set => zoomSpeed = value;
    }

    [SerializeField] private float maxZoom = 15;

    public float MaxZoom
    {
        get => maxZoom;
        set => maxZoom = value;
    }

    [SerializeField] private float minZoom = 3;

    public float MinZoom
    {
        get => minZoom;
        set => minZoom = value;
    }

    [SerializeField] private float zoomAnimDuration;

    private Vector2? lastMousePos;
    private Camera cam;

    private void Update()
    {
        // right click drag to pan
        if (Input.GetMouseButton(KeybindManager.Instance.panMouseButton))
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
                movement = new(Utils.PixelToUnit(movement.x), Utils.PixelToUnit(movement.y));
                if (EventSystem.current.IsPointerOverGameObject()) movement = Vector2.zero;

                transform.position += (Vector3)movement;

                lastMousePos = currentMousePos;
            }
        }

        if (Input.GetMouseButtonUp(KeybindManager.Instance.panMouseButton)) lastMousePos = null;

        float zoomInput = EventSystem.current.IsPointerOverGameObject() ? 0 : -Input.GetAxis("Mouse ScrollWheel");
        Zoom(zoomInput);
    }

    private void Zoom(float zoomInput)
    {
        if (zoomInput == 0f || !MouseManager.Instance.IsOnScreen) return; // zoom

        if (!(cam.orthographicSize + zoomInput * zoomSpeed >= minZoom) ||
            !(cam.orthographicSize + zoomInput * zoomSpeed <= maxZoom)) return;

        Vector2 prevMousePos = MouseManager.Instance.MouseWorldPos;
        Vector2 prevMouseOffsetUnits = prevMousePos - (Vector2)transform.position;
        Vector2 prevMouseOffsetPixels = Utils.UnitToPixel(prevMouseOffsetUnits);

        float newOrthoSize = cam.orthographicSize * (zoomInput * zoomSpeed + 1);
        if (newOrthoSize > maxZoom) newOrthoSize = maxZoom;
        if (newOrthoSize < minZoom) newOrthoSize = minZoom;

        Vector2 newMouseOffset = Utils.PixelToUnit(prevMouseOffsetPixels, newOrthoSize);
        Vector3 newCamPos = prevMousePos - newMouseOffset;

        // apply
        cam.DOKill();
        cam.DOOrthoSize(newOrthoSize, zoomAnimDuration);
        transform.DOKill();
        transform.DOMove(new Vector3(newCamPos.x, newCamPos.y, transform.position.z), zoomAnimDuration);
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
    }
}