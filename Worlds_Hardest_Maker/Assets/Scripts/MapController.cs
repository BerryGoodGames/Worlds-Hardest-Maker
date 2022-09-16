using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// controls map / camera movement
/// attach to main camera
/// </summary>
public class MapController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 4f;
    public float ZoomSpeed { get { return zoomSpeed; } set { zoomSpeed = value; } }
    [SerializeField] private float maxZoom = 15;
    public float MaxZoom { get { return maxZoom; } set { maxZoom = value; } }
    [SerializeField] private float minZoom = 3;
    public float MinZoom { get { return minZoom; } set { minZoom = value; } }

    private Vector2? lastMousePos = null;

    private void Update()
    {
        // right click drag to pan
        if (Input.GetMouseButton(1))
        {
            if(lastMousePos == null)
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
                movement = new(GameManager.PixelToUnit(movement.x), GameManager.PixelToUnit(movement.y));
                if (EventSystem.current.IsPointerOverGameObject()) movement = Vector2.zero;

                transform.position += (Vector3) movement;

                lastMousePos = currentMousePos;
            }
        }

        if (Input.GetMouseButtonUp(1)) lastMousePos = null;

        float zoomInput = EventSystem.current.IsPointerOverGameObject() ? 0 : -Input.GetAxis("Mouse ScrollWheel");
        if (zoomInput != 0f && MouseManager.Instance.OnScreen) // zoom
        {
            Camera cam = GetComponent<Camera>();
            if (cam.orthographicSize + zoomInput * zoomSpeed >= minZoom && GetComponent<Camera>().orthographicSize + zoomInput * zoomSpeed <= maxZoom)
            {
                Vector2 prevMousePos = MouseManager.GetMouseWorldPos();
                Vector2 prevMouseOffsetUnits = prevMousePos - (Vector2)transform.position;
                Vector2 prevMouseOffsetPixels = GameManager.UnitToPixel(prevMouseOffsetUnits);

                cam.orthographicSize *= zoomInput * zoomSpeed + 1;
                if (cam.orthographicSize > maxZoom) cam.orthographicSize = maxZoom;
                if (cam.orthographicSize < minZoom) cam.orthographicSize = minZoom;

                Vector2 newMouseOffset = GameManager.PixelToUnit(prevMouseOffsetPixels);
                Vector2 newCamPos = prevMousePos - newMouseOffset;
                transform.position = new(newCamPos.x, newCamPos.y, transform.position.z);
            }
        }
    }
}
