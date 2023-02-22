using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ScreenDimensions : MonoBehaviour
{
    [SerializeField] private bool setScreenWidth;
    [SerializeField] private bool setScreenHeight;

    [FormerlySerializedAs("applyMaxZoomFromMapController")]
    public bool ApplyMaxZoomFromMapController;

    [SerializeField] private float maxZoom;

    [FormerlySerializedAs("hasRectTransform")]
    public bool HasRectTransform;

    [SerializeField] private RectTransform canvas;

    private void Start()
    {
        if (HasRectTransform)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new((setScreenWidth ? canvas : rt).rect.width, (setScreenHeight ? canvas : rt).rect.height);
        }
        else
        {
            Camera cam = Camera.main;
            if (cam == null)
                throw new Exception("Couldn't set gameObject to screen dimensions because main camera is null");

            float zoom;
            if (ApplyMaxZoomFromMapController)
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