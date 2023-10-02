using MyBox;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// custom fitter I mean like what else
///
/// Niko sucks
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
    /// Checks for any changes within the anchor blocks and updates size to fit the anchor blocks on the y-axis
    /// </summary>
    [ButtonMethod]
    public void UpdateSize(bool checkChanged = true)
    {
        if (checkChanged && !ChildrenChanged()) return;

        float y;

        if (top)
        {
            y = minimumHeight - bottomPadding;

            // get maximum y of all children
            foreach (RectTransform child in transform)
            {
                Vector2 scale = child.sizeDelta;

                Vector2 position = child.anchoredPosition;
                float thisMaxY = position.y + scale.y;

                if (thisMaxY > y) y = thisMaxY;
            }
        }
        else
        {
            y = -minimumHeight + bottomPadding;

            // get minimum y of all children
            foreach (RectTransform child in transform)
            {
                Vector2 scale = child.sizeDelta;

                Vector2 position = child.anchoredPosition;
                float thisMinY = position.y - scale.y;

                if (thisMinY < y) y = thisMinY;
            }
        }

        y -= bottomPadding;

        if (hasLayoutElement)
        {
            layoutElement.minHeight = Mathf.Abs(y);
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)rt.parent);
        }
        else rt.sizeDelta = new(rt.sizeDelta.x, -y);
    }
}