using UnityEngine;

public class ToolbarSizing : MonoBehaviour
{
    public Canvas canvas;

    [Space] public float toolbarHeight;

    public void UpdateSize()
    {
        // set height of toolbarContainer and scale values
        RectTransform rt = transform.parent.GetComponent<RectTransform>();
        rt.sizeDelta = new(0, toolbarHeight);

        // scale Tools
        foreach (Transform tool in transform)
        {
            tool.localScale = new(toolbarHeight / 100, toolbarHeight / 100);
        }

        Transform background = transform.parent.GetChild(0);
        RectTransform backgroundRectTransform = background.GetComponent<RectTransform>();
        backgroundRectTransform.sizeDelta = new(0, toolbarHeight + 200);
    }

    public void ScaleOptionsInOptionbars()
    {
        ToolOptionbar[] optionbars = FindObjectsOfType<ToolOptionbar>();
        foreach (ToolOptionbar optionbar in optionbars)
        {
            optionbar.ScaleOptions();
        }
    }

    public void UpdateOptionbarHeights()
    {
        ToolOptionbar[] optionbars = FindObjectsOfType<ToolOptionbar>();
        foreach (ToolOptionbar optionbar in optionbars)
        {
            optionbar.UpdateHeight();
        }
    }
}