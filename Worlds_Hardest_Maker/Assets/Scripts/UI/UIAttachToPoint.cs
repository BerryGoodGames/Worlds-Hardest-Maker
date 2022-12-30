using UnityEngine;

public class UIAttachToPoint : MonoBehaviour
{
    public Vector2 point;
    public bool zoomSizeWithCamera;
    public bool zoomPositionWithCamera;

    private Camera cam;
    private RectTransform rt;

    private void Awake()
    {
        cam = Camera.main;
        rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        rt.position = cam.WorldToScreenPoint(point);

        if (!zoomSizeWithCamera) return;

        float scl = 10.0f / cam.orthographicSize;
        rt.localScale = new(scl, scl);
    }
}