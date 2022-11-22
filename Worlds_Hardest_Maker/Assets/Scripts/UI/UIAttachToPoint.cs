using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttachToPoint : MonoBehaviour
{
    public Vector2 point;
    public bool zoomSizeWithCamera;
    public bool zoomPositionWithCamera;    

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.position = cam.WorldToScreenPoint(point);

        if (zoomSizeWithCamera)
        {
            float scl = 10.0f / cam.orthographicSize;
            rt.localScale = new(scl, scl);
        }

        if (zoomPositionWithCamera)
        {
            // TODO
        }
    }
}
