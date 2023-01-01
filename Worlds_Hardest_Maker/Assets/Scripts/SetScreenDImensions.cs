using System;
using UnityEngine;

public class SetScreenDimensions : MonoBehaviour
{
    [SerializeField] private bool setScreenWidth;
    [SerializeField] private bool setScreenHeight;
    public bool applyMaxZoomFromMapController;
    [SerializeField] private float maxZoom;
    public bool hasRectTransform;
    [SerializeField] private RectTransform canvas;

    private void Start()
    {
        if (hasRectTransform)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new((setScreenWidth ? canvas : rt).rect.width, (setScreenHeight ? canvas : rt).rect.height);
        }
        else
        {
            Camera cam = Camera.main;
            if (cam == null) throw new Exception("Couldn't set gameObject to screen dimensions because main camera is null");

            float zoom;
            if (applyMaxZoomFromMapController)
            {
                MapController map = cam.GetComponent<MapController>();
                zoom = map.MaxZoom;
            }
            else
            {
                zoom = maxZoom;
            }
            float height = 2 * zoom;
            float width = cam.aspect * height;
            transform.localScale = new(
                setScreenWidth ? width : transform.localScale.x,
                setScreenHeight ? height : transform.localScale.y
            );
        }
    }


}