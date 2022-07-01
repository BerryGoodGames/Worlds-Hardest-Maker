using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controls map / camera movement
/// attach to main camera
/// </summary>
public class MapController : MonoBehaviour
{
    public float zoomSpeed = 4f; 
    public float maxZoom = 15;
    public float minZoom = 3;

    private Camera cam;
    private Vector2? lastMousePos = null;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
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

                transform.position += (Vector3) movement;

                lastMousePos = currentMousePos;
            }
        }

        if (Input.GetMouseButtonUp(1)) lastMousePos = null;

        float zoomInput = -Input.GetAxis("Mouse ScrollWheel");
        if (zoomInput != 0f) // zoom
        {
            if (GetComponent<Camera>().orthographicSize + zoomInput * zoomSpeed >= minZoom && GetComponent<Camera>().orthographicSize + zoomInput * zoomSpeed <= maxZoom)
            {
                GetComponent<Camera>().orthographicSize += zoomInput * zoomSpeed;
                // cam.GetComponent<BackgroundLoop>().CalcSize();
            }
        }
    }
}
