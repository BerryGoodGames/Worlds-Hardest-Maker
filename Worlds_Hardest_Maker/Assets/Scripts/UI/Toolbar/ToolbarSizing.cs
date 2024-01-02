using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class ToolbarSizing : MonoBehaviour
{
    public Canvas Canvas;

    [Space] public float ToolbarHeight;

    [MustBeAssigned] [SerializeField] private CustomFitter fitter;

    [ButtonMethod]
    public void UpdateSize()
    {
        // set height of toolbarContainer and scale values
        RectTransform parentRt = (RectTransform)transform.parent;
        parentRt.sizeDelta = new(0, ToolbarHeight);

        // scale Tools
        // foreach (Transform tool in transform) tool.localScale = new(ToolbarHeight / 100, ToolbarHeight / 100);

        Transform background = parentRt.GetChild(0);
        RectTransform backgroundRt = background.GetComponent<RectTransform>();
        backgroundRt.sizeDelta = new(0, ToolbarHeight + 200);

        fitter.UpdateSize(false);
    }

    // [ButtonMethod]
    // public void ScaleOptionsInEveryOptionbar()
    // {
    //     ToolOptionbar[] optionbars = FindObjectsOfType<ToolOptionbar>();
    //     foreach (ToolOptionbar optionbar in optionbars) optionbar.ScaleOptions();
    // }

    [ButtonMethod] [UsedImplicitly]
    public void UpdateEveryOptionbarHeight()
    {
        ToolOptionbar[] optionbars = FindObjectsOfType<ToolOptionbar>();
        foreach (ToolOptionbar optionbar in optionbars) optionbar.UpdateHeight();
    }
}