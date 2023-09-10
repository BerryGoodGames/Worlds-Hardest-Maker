using MyBox;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ToolOptionbar : MonoBehaviour
{
    public GameObject Background;

    public GameObject HoveringHitbox;

    public GameObject Options;
    public float Size;
    private RectTransform hh;
    private RectTransform rtThis;
    private GridLayoutGroup gridLayout;
    private AlphaUITween anim;
    private int toolCount;
    private float width;
    private float height;

    private void Awake()
    {
        // REF
        rtThis = GetComponent<RectTransform>();
        anim = GetComponent<AlphaUITween>();

        anim.OnSetVisible += EnableOptionbar;
        anim.OnIsInvisible += DisableOptionbar;

        hh = HoveringHitbox.GetComponent(typeof(RectTransform)) as RectTransform;
        gridLayout = Options.GetComponent<GridLayoutGroup>();
        toolCount = Options.transform.childCount;

        UpdateHeight();
        ScaleOptions();

        DisableOptionbar();
    }

    public void EnableOptionbar()
    {
        hh.sizeDelta = new(width, height + gridLayout.cellSize.y + gridLayout.spacing.y);
        hh.localPosition = new(0, (2 - toolCount) * (gridLayout.cellSize.y + gridLayout.spacing.y) * 0.5f);
        rtThis.localPosition = new(0, -95);
    }

    public void DisableOptionbar()
    {
        if (anim.IsVisible) return;

        hh.sizeDelta = new(gridLayout.cellSize.x, gridLayout.cellSize.y);
        hh.localPosition = new(0, -1250);
        rtThis.localPosition = new(0, 1000);
    }

    [ButtonMethod]
    public void ScaleOptions()
    {
        foreach (Transform tool in Options.transform)
        {
            tool.localScale = new(0.7f, 0.7f);
        }
    }

    [ButtonMethod]
    public void UpdateHeight()
    {
        RectTransform rt = Background.GetComponent(typeof(RectTransform)) as RectTransform;

        if (rt == null) return;

        if (toolCount == 0)
            rt.sizeDelta = new(100, 100);
        else
        {
            width = gridLayout.cellSize.x * Size;
            height = (gridLayout.cellSize.y + gridLayout.spacing.y) * toolCount - gridLayout.spacing.y +
                     gridLayout.cellSize.y * (Size - 1);

            rt.sizeDelta = new(width, height);
        }
    }
}