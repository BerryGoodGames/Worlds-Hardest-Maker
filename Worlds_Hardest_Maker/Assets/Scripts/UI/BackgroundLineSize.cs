using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLineSize : MonoBehaviour
{
    // order: top, right, bottom, left
    [Header("Line order: top, right, bottom, left")]
    public RectTransform[] lines;

    [Space]
    public float newLineSize;

    public void SetLineSize()
    {
        SetLineSize(newLineSize);
    }
    public void SetLineSize(float size)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            RectTransform line = lines[i];
            if (i % 2 == 0) line.sizeDelta = new(0, size);
            else line.sizeDelta = new(size, 0);
        }
    }
}