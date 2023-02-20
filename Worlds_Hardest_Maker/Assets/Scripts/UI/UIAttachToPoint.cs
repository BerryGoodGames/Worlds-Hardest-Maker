using UnityEngine;
using UnityEngine.Serialization;

public class UIAttachToPoint : MonoBehaviour
{
    [FormerlySerializedAs("point")] public Vector2 Point;
    [FormerlySerializedAs("zoomSizeWithCamera")] public bool ZoomSizeWithCamera;
    [FormerlySerializedAs("zoomPositionWithCamera")] public bool ZoomPositionWithCamera;

    private Camera cam;
    private RectTransform rt;

    private void Awake()
    {
        cam = Camera.main;
        rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        rt.position = cam.WorldToScreenPoint(Point);

        if (!ZoomSizeWithCamera) return;

        float scl = 10.0f / cam.orthographicSize;
        rt.localScale = new(scl, scl);
    }
}