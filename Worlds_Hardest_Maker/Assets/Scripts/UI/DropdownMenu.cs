using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu : MonoBehaviour
{
    public RectTransform rt;
    public RectTransform arrowRt;
    public BackgroundLineSize bgLineSize;
    [Space]
    public float originalHeight = 80;
    public float originalLineSize = 4;
    public float originalBaseLineSize = 10;
    public float originalArrowScl = 0.3f;
    public float originalArrowDist = -15;
    [Space]
    public float height = 100;
}
