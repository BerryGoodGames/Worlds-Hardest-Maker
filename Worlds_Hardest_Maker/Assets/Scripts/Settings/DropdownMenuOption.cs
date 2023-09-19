using UnityEngine;
using UnityEngine.UI;

public class DropdownMenuOption : SettingOption
{
    [Space] [SerializeField] private RectTransform arrowRt;
    [SerializeField] private BackgroundLineSize bgLineSize;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    [Space] [SerializeField] private float originalHeight = 80;

    public float OriginalHeight
    {
        get => originalHeight;
        set => originalHeight = value;
    }

    [SerializeField] private float originalWidth = 250;

    public float OriginalWidth
    {
        get => originalWidth;
        set => originalWidth = value;
    }

    [SerializeField] private float originalLineSize = 4;
    [SerializeField] private float originalBaseLineSize = 10;
    [SerializeField] private float originalArrowScl = 0.3f;
    [SerializeField] private float originalArrowDist = -15;

    public override void Response()
    {
        base.Response();

        // calculate line sizes + arrow scale
        float mLineSize = originalLineSize / originalHeight;
        float mBaseLineSize = originalBaseLineSize / originalHeight;
        float mArrowScl = originalArrowScl / originalHeight;
        float mArrowDist = originalArrowDist / originalHeight;

        float newLineSize = mLineSize * Height;
        float newBaseLineSize = mBaseLineSize * Height;
        float newArrowScl = mArrowScl * Height;
        float newArrowDist = mArrowDist * Height;

        float newRatio = originalWidth / originalHeight;
        aspectRatioFitter.aspectRatio = newRatio;

        BackgroundLineSize lineSizeController = bgLineSize;

        lineSizeController.SetLineSize(newLineSize);
        lineSizeController.SetLineSizeBottom(newBaseLineSize);

        arrowRt.localScale = newArrowScl * Vector2.one;
        arrowRt.anchoredPosition = new(newArrowDist, arrowRt.anchoredPosition.y);
    }
}