using MyBox;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     custom fitter I mean like what else
///     Niko sucks
/// </summary>
[ExecuteInEditMode]
public class CustomFitter : MonoBehaviour
{
    [SerializeField] private bool top;
    [SerializeField] private float bottomPadding;
    [SerializeField] private float minimumHeight;
    [SerializeField] private bool updateEachFrame;

    private int lastChildCount;
    private RectTransform rt;
    private LayoutElement layoutElement;
    private bool hasLayoutElement;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (TryGetComponent(out layoutElement)) hasLayoutElement = true;
    }

    private void Update()
    {
        if (!updateEachFrame) return;
        UpdateSize();
    }

    private bool ChildrenChanged()
    {
        // check for new child / one child less
        if (transform.childCount != lastChildCount)
        {
            lastChildCount = transform.childCount;
            return true;
        }

        // check if their scale/positions have changed
        foreach (RectTransform child in transform)
        {
            if (!child.hasChanged) continue;
            child.hasChanged = false;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Checks for any changes within the anchor blocks and updates size to fit the anchor blocks on the y-axis
    /// </summary>
    [ButtonMethod]
    public void UpdateSize(bool checkChanged = true)
    {
        if (rt == null) return;
        if (checkChanged && !ChildrenChanged()) return;

        float y = (top ? 1 : -1) * minimumHeight - bottomPadding;

        // get maximum / minimum y of all children
        foreach (RectTransform child in transform)
        {
            Vector2 scale = child.sizeDelta;

            Vector2 position = child.anchoredPosition;
            float thisMaxMinY = position.y + scale.y * (top ? 1 : -1);

            if ((top && thisMaxMinY > y)
                || (!top && thisMaxMinY < y)) y = thisMaxMinY;
        }

        y -= bottomPadding;

        if (hasLayoutElement)
        {
            layoutElement.minHeight = Mathf.Abs(y);
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)rt.parent);
            return;
        }
        
        rt.sizeDelta = new(rt.sizeDelta.x, -y);
    }
}