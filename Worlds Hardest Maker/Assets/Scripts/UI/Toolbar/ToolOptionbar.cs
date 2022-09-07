using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ToolOptionbar : MonoBehaviour
{
    public GameObject background;
    public GameObject hoveringHitbox;
    public GameObject options;
    public float size;
    [HideInInspector] public Animator anim;
    private RectTransform hh;
    private RectTransform rtThis;
    private GridLayoutGroup gridLayout;
    private int toolCount;
    private float width;
    private float height;

    private void Awake()
    {
        // REF
        rtThis = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();

        hh = hoveringHitbox.GetComponent(typeof(RectTransform)) as RectTransform;
        gridLayout = options.GetComponent<GridLayoutGroup>();
        toolCount = options.transform.childCount;

        UpdateHeight();
        ScaleOptions();
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
        if (toolCount == 0)
        {
            rt.sizeDelta = new(100, 100);
        } else
        {
            width = gridLayout.cellSize.x * size;
            height = (gridLayout.cellSize.y + gridLayout.spacing.y) * toolCount - gridLayout.spacing.y + gridLayout.cellSize.y * (size - 1);

            rt.sizeDelta = new(width, height);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ToolOptionbar))]
public class ToolOpionbarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ToolOptionbar script = (ToolOptionbar)target;

        if (GUILayout.Button("Scale options"))
        {
            script.ScaleOptions();
        }

        if(GUILayout.Button("Update Height"))
        {
            script.UpdateHeight();
        }
    }

}
#endif