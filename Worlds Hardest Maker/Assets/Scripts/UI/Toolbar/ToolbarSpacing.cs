using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarSpacing : MonoBehaviour
{
    public Canvas canvas;
    public int margin;
    public int OffsetTop;
    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

    [Space]
    public float toolbarHeight;

    private void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        rectTransform = canvas.GetComponent<RectTransform>();

        float width = rectTransform.rect.width;
        int count = transform.childCount;

        float itemWidth = gridLayout.cellSize.x;
        float itemWidthTotal = itemWidth * count;

        // gridLayout.spacing = new((width - itemWidthTotal) / (count - 1) - (margin * 2 / (count - 1)), 0);
        gridLayout.spacing = new((width - itemWidthTotal - 2 * margin) / (count - 1), 0);

        gridLayout.padding.left = margin;
    }
    public void UpdateSize()
    {
        // set height of toolbar and scale values
        float height = toolbarHeight;

        RectTransform rt = transform.parent.GetComponent<RectTransform>();
        rt.sizeDelta = new(0, height);

        // scale tools
        foreach (Transform tool in transform)
        {
            tool.localScale = new(height / 100, height / 100);
        }

        GetComponent<GridLayoutGroup>().padding.top = (int)(0.55f * height - 30);

        Transform background = transform.parent.GetChild(0);
        RectTransform bgrt = background.GetComponent<RectTransform>();
        bgrt.sizeDelta = new(0, height + 200);
    }
}


