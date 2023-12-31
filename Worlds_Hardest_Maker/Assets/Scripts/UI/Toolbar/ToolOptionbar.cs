using MyBox;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ToolOptionbar : MonoBehaviour
{
    [SerializeField] private RectTransform toolPrefab;
    
    public GameObject Background;

    public GameObject HoveringHitbox;

    public GameObject Options;
    public float Size;
    private RectTransform hh;
    private RectTransform rtThis;
    private VerticalLayoutGroup verticalLayout;
    private AlphaTween anim;
    private int toolCount;
    private float width;
    private float height;

    private void Awake()
    {
        // REF
        rtThis = GetComponent<RectTransform>();
        anim = GetComponent<AlphaTween>();

        // anim.OnSetVisible += EnableOptionbar;
        // anim.OnIsInvisible += DisableOptionbar;

        hh = (RectTransform)HoveringHitbox.transform;
        verticalLayout = Options.GetComponent<VerticalLayoutGroup>();
        toolCount = Options.transform.childCount;

        UpdateHeight();
        ScaleOptions();

        // DisableOptionbar();
        EnableOptionbar();
    }

    public void EnableOptionbar()
    {
        Rect toolRect = toolPrefab.rect;
        
        hh.sizeDelta = new(width, height + toolRect.height + 0);
        hh.localPosition = new(0, (2 - toolCount) * (toolRect.height + 0) * 0.5f);
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
        foreach (Transform tool in Options.transform) tool.localScale = new(0.7f, 0.7f);
    }

    [ButtonMethod]
    public void UpdateHeight()
    {
        RectTransform rt = (RectTransform)Background.transform;

        if (rt == null) return;

        if (toolCount == 0)
        {
            rt.sizeDelta = ToolHeight * Vector2.one;
            return;
        }

        width = rt.rect.width;
        height = (gridLayout.cellSize.y + gridLayout.spacing.y) * toolCount
            - gridLayout.spacing.y + gridLayout.cellSize.y * (Size - 1);

        rt.sizeDelta = new(width, height);
    }
}