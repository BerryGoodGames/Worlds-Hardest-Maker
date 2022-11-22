using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttachToPoint : MonoBehaviour
{
    public Vector2 point;
    public bool zoomWithCamera;
    public bool restrictToScreen;

    public float restrictPaddingLeft;
    public float restrictPaddingRight;
    public float restrictPaddingTop;
    public float restrictPaddingBottom;

    private Vector2 canvasSize;
    Camera cam;

    private void Awake()
    {
        canvasSize = GameManager.Instance.Canvas.GetComponent<RectTransform>().sizeDelta;
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.position = cam.WorldToScreenPoint(point);

        if (zoomWithCamera)
        {
            float scl = 10.0f / cam.orthographicSize;
            rt.localScale = new(scl, scl);
        }

        if (restrictToScreen)
        {
            float x = Mathf.Clamp(rt.position.x, rt.pivot.x * rt.sizeDelta.x + restrictPaddingLeft, canvasSize.x - ((1 - rt.pivot.x) * rt.sizeDelta.x) - restrictPaddingRight);
            float y = Mathf.Clamp(rt.position.y, rt.pivot.y * rt.sizeDelta.y + restrictPaddingBottom, canvasSize.y - ((1 - rt.pivot.y) * rt.sizeDelta.y) - restrictPaddingTop);
            
            rt.position = new(x, y);
        }
    }
}
