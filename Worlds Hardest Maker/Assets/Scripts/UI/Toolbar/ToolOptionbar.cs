using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolOptionbar : MonoBehaviour
{
    public GameObject background;
    public GameObject hoveringHitbox;
    public GameObject options;
    public float size;
    [HideInInspector] public Animator anim;
    RectTransform hh;
    RectTransform rtThis;
    GridLayoutGroup gridLayout;
    int toolCount;
    float width;
    float height;

    private void Awake()
    {
        rtThis = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();

        RectTransform rt = background.GetComponent(typeof(RectTransform)) as RectTransform;
        hh = hoveringHitbox.GetComponent(typeof(RectTransform)) as RectTransform;
        gridLayout = options.GetComponent<GridLayoutGroup>();
        toolCount = options.transform.childCount;

        width = gridLayout.cellSize.x * size;
        height = (gridLayout.cellSize.y + gridLayout.spacing.y) * toolCount - gridLayout.spacing.y + gridLayout.cellSize.y * (size - 1);

        rt.sizeDelta = new(width, height);

        foreach (Transform tool in options.transform)
        {
            tool.localScale = new(0.7f, 0.7f);
            
        }

    }

    public void EnableOptionbar()
    {
        hh.sizeDelta = new(width, height + gridLayout.cellSize.y + gridLayout.spacing.y);
        hh.localPosition = new(0, (2 - toolCount) * (gridLayout.cellSize.y + gridLayout.spacing.y) / 2);
        rtThis.localPosition = new(0, -95);
    }

    public void DisableOptionbar()  
    {
        if (!anim.GetBool("Hovered"))
        {
            hh.sizeDelta = new(gridLayout.cellSize.x, gridLayout.cellSize.y);
            hh.localPosition = new(0, -1250);
            rtThis.localPosition = new(0, 1000);
        }
    }
}
