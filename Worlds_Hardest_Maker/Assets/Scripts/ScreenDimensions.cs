using System;
using MyBox;
using UnityEngine;

public class ScreenDimensions : MonoBehaviour
{
    [SerializeField] private bool setScreenWidth;
    [SerializeField] private bool setScreenHeight;

    public bool ApplyMaxZoomFromMapController;

    [ConditionalField(nameof(ApplyMaxZoomFromMapController), true)] [SerializeField] private float maxZoom;

    public bool HasRectTransform;

    [ConditionalField(nameof(HasRectTransform))] [SerializeField] private RectTransform canvas;

    private void Start()
    {
        if (HasRectTransform)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new((setScreenWidth ? canvas : rt).rect.width, (setScreenHeight ? canvas : rt).rect.height);
        }
        else
        {
            Camera cam = Camera.main
                ? Camera.main
                : throw new Exception("Couldn't set gameObject to screen dimensions because main camera is null");

            float zoom;
            if (ApplyMaxZoomFromMapController)
            {
                MapController map = cam.GetComponent<MapController>();
                zoom = map.ZoomLimits.Max;
            }
            else zoom = maxZoom;

            float height = 2 * zoom;
            float width = cam.aspect * height;
            transform.localScale = new(
                setScreenWidth ? width : transform.localScale.x,
                setScreenHeight ? height : transform.localScale.y
            );
        }
    }
}