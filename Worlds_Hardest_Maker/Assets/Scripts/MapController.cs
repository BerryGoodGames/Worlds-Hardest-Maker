using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

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

    [SerializeField] private float zoomAnimDuration;

    private Vector2? lastMousePos = null;

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
                movement = new(GameManager.PixelToUnit(movement.x), GameManager.PixelToUnit(movement.y));
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
        if (zoomInput != 0f && MouseManager.Instance.OnScreen) // zoom
        {
            Camera cam = GetComponent<Camera>();
            if (cam.orthographicSize + zoomInput * zoomSpeed >= minZoom && cam.orthographicSize + zoomInput * zoomSpeed <= maxZoom)
            {
                Vector2 prevMousePos = MouseManager.Instance.MouseWorldPos;
                Vector2 prevMouseOffsetUnits = prevMousePos - (Vector2)transform.position;
                Vector2 prevMouseOffsetPixels = GameManager.UnitToPixel(prevMouseOffsetUnits);

                float newOrthoSize = cam.orthographicSize * (zoomInput * zoomSpeed + 1);
                if (newOrthoSize > maxZoom) newOrthoSize = maxZoom;
                if (newOrthoSize < minZoom) newOrthoSize = minZoom;

                Vector2 newMouseOffset = GameManager.PixelToUnit(prevMouseOffsetPixels, newOrthoSize);
                Vector3 newCamPos = prevMousePos - newMouseOffset;

                // apply
                cam.DOKill();
                cam.DOOrthoSize(newOrthoSize, zoomAnimDuration);
                transform.DOKill();
                transform.DOMove(new Vector3(newCamPos.x, newCamPos.y, transform.position.z), zoomAnimDuration);
            }
        }
    }
}
