using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenDimensions : MonoBehaviour
{
    [SerializeField] private bool setScreenWidth;
    [SerializeField] private bool setScreenHeight;
    public bool hasRectTransform;
    [SerializeField] private RectTransform canvas;

    private void Start()
    {
        if(hasRectTransform)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new((setScreenWidth ? canvas : rt).rect.width, (setScreenHeight ? canvas : rt).rect.height);
        } else
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            MapController map = cam.GetComponent<MapController>();
            float zoom = map.MaxZoom;
            float height = 2 * zoom;
            float width = cam.aspect * height;
            transform.localScale = new(
                setScreenWidth ? width : transform.localScale.x,
                setScreenHeight ? height : transform.localScale.y
            );
        }
    }
}
