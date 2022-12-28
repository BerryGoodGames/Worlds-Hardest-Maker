using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private RectTransform handleContainer;
    [SerializeField] private RectTransform leftBounce;
    [SerializeField] private RectTransform rightBounce;
    [Space]
    [SerializeField] private float width = 400;
    public float Width { 
        get => width;
        set => width = value;
    }
    [SerializeField] private float height = 80;
    public float Height {
        get => height;
        set => height = value;
    }
    [SerializeField] private float size = 10;
    public float Size { 
        get => size;
        set => size = value;
    }
    [Space]
    [SerializeField] private float originalSize;
    [SerializeField] private float originalBackgroundSize;
    [SerializeField] private float originalHandleSize;
    [SerializeField] private float originalBounceSize;

    public void Response()
    {
        float mBackgroundSize = originalBackgroundSize / originalSize;
        float mHandleSize = originalHandleSize / originalSize;
        float mBounceSize = originalBounceSize / originalSize;

        float newBackgroundSize = mBackgroundSize * size;
        float newHandleSize = mHandleSize * size;
        float newBounceSize = mBounceSize * size;

        background.sizeDelta = new(0, newBackgroundSize);
        handle.sizeDelta = Vector2.one * newHandleSize;
        handleContainer.sizeDelta = new(newHandleSize, 0);
        leftBounce.sizeDelta = Vector2.one * newBounceSize;
        rightBounce.sizeDelta = Vector2.one * newBounceSize;

        rectTransform.sizeDelta = new(width, height);
    }
}
