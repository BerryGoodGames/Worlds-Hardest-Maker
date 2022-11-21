using UnityEngine;

public class UIAttachToPoint : MonoBehaviour
{
    public Vector2 point;
    public bool zoomWithCamera;
    public bool restrictToScreen;
    public float restrictPadding;

    private void LateUpdate()
    {
        Camera cam = Camera.main;

        RectTransform rt = GetComponent<RectTransform>();
        rt.position = cam.WorldToScreenPoint(point);

        if (zoomWithCamera)
        {
            float scl = 10.0f / cam.orthographicSize;
            rt.localScale = new(scl, scl);
        }

        if (restrictToScreen)
        {
            // TODO: restrict to fit on screen with padding
        }
    }
}
