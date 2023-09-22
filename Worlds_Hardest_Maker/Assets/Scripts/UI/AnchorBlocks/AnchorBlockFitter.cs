using UnityEngine;

/// <summary>
/// Resizes anchor block string container to match the necessary size
/// Attach to container of anchor block strings
/// </summary>
[ExecuteInEditMode]
public class AnchorBlockFitter : MonoBehaviour
{
    [SerializeField] private float bottomPadding;
    [SerializeField] private float minimumHeight;

    private int lastChildCount;
    private RectTransform[] children;
    private RectTransform rt;

    private void Awake() => rt = GetComponent<RectTransform>();

    private bool ChildrenChanged()
    {
        // check for new child / one child less
        if (transform.childCount != lastChildCount)
        {
            lastChildCount = transform.childCount;
            return true;
        }

        // check if their scale/positions have changed
        foreach (RectTransform child in children)
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
    public void CheckForChanges()
    {
        UpdateChildrenArray();

        if (!ChildrenChanged()) return;

        float minY = -minimumHeight + bottomPadding;

        // get minimum y of all children
        foreach (RectTransform child in children)
        {
            Vector2 scale = child.sizeDelta;

            Vector2 position = child.anchoredPosition;
            float thisMinY = position.y - scale.y;

            if (thisMinY < minY) minY = thisMinY;
        }

        minY -= bottomPadding;

        rt.sizeDelta = new(rt.sizeDelta.x, -minY);
    }

    public void UpdateChildrenArray() => children = this.GetComponentsInDirectChildren<RectTransform>();
}