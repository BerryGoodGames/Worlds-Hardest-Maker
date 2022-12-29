using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ToolOptionbar : MonoBehaviour
{
    public GameObject background;
    public GameObject hoveringHitbox;
    public GameObject options;
    public float size;
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

        anim.onSetVisible += EnableOptionbar;
        anim.onIsInvisible += DisableOptionbar;

        hh = hoveringHitbox.GetComponent(typeof(RectTransform)) as RectTransform;
        gridLayout = options.GetComponent<GridLayoutGroup>();
        toolCount = options.transform.childCount;

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
        if (!anim.IsVisible())
        {
            hh.sizeDelta = new(gridLayout.cellSize.x, gridLayout.cellSize.y);
            hh.localPosition = new(0, -1250);
            rtThis.localPosition = new(0, 1000);
        }
    }

    public void ScaleOptions()
    {
        foreach (Transform tool in options.transform)
        {
            tool.localScale = new(0.7f, 0.7f);
        }
    }

    public void UpdateHeight()
    {
        RectTransform rt = background.GetComponent(typeof(RectTransform)) as RectTransform;

        if (rt == null) return;

        if (toolCount == 0)
        {
            rt.sizeDelta = new(100, 100);
        }
        else
        {
            width = gridLayout.cellSize.x * size;
            height = (gridLayout.cellSize.y + gridLayout.spacing.y) * toolCount - gridLayout.spacing.y +
                     gridLayout.cellSize.y * (size - 1);

            rt.sizeDelta = new(width, height);
        }
    }
}