using UnityEngine;
using UnityEngine.Serialization;

public class ToolbarSizing : MonoBehaviour
{
    [FormerlySerializedAs("canvas")] public Canvas Canvas;

    [FormerlySerializedAs("toolbarHeight")] [Space]
    public float ToolbarHeight;

    public void UpdateSize()
    {
        // set height of toolbarContainer and scale values
        RectTransform rt = transform.parent.GetComponent<RectTransform>();
        rt.sizeDelta = new(0, ToolbarHeight);

        // scale Tools
        foreach (Transform tool in transform)
        {
            tool.localScale = new(ToolbarHeight / 100, ToolbarHeight / 100);
        }

        Transform background = transform.parent.GetChild(0);
        RectTransform backgroundRectTransform = background.GetComponent<RectTransform>();
        backgroundRectTransform.sizeDelta = new(0, ToolbarHeight + 200);
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