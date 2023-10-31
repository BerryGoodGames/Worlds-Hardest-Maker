using MyBox;
using UnityEngine;

public class BackgroundLineSize : MonoBehaviour
{
    // order: top, right, bottom, left
    [Header("Line order: top, right, bottom, left")] public RectTransform[] Lines;

    [Space] public float NewLineSize;

    [ButtonMethod]
    public void SetLineSize() => SetLineSize(NewLineSize);

    public void SetLineSize(float size)
    {
        for (int i = 0; i < Lines.Length; i++) { SetLineSize(i, size); }
    }

    public void SetLineSize(int i, float size)
    {
        RectTransform line = Lines[i];
        if (i % 2 == 0) line.sizeDelta = new(0, size);
        else line.sizeDelta = new(size, 0);
    }

    public void SetLineSizeTop(float size) => SetLineSize(0, size);

    public void SetLineSizeRight(float size) => SetLineSize(1, size);

    public void SetLineSizeBottom(float size) => SetLineSize(2, size);

    public void SetLineSizeLeft(float size) => SetLineSize(3, size);
}