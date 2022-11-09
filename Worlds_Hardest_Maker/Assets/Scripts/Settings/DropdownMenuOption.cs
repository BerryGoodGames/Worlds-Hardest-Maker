using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenuOption : SettingOption
{
    [Space]
    [SerializeField] private RectTransform arrowRt;
    [SerializeField] private BackgroundLineSize bgLineSize;
    [Space]
    [SerializeField] private float originalHeight = 80;
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

        float newLineSize = mLineSize * height;
        float newBaseLineSize = mBaseLineSize * height;
        float newArrowScl = mArrowScl * height;
        float newArrowDist = mArrowDist * height;

        BackgroundLineSize lineSizeController = bgLineSize;

        lineSizeController.SetLineSize(newLineSize);
        lineSizeController.SetLineSizeBottom(newBaseLineSize);

        arrowRt.localScale = newArrowScl * Vector2.one;
        arrowRt.anchoredPosition = new(newArrowDist, arrowRt.anchoredPosition.y);
    }
}
