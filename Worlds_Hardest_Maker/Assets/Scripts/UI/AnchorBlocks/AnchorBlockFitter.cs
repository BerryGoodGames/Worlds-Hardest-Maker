using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class AnchorBlockFitter : MonoBehaviour
{
    [SerializeField] private float bottomPadding;
    [SerializeField] private float minimumHeight;

    private int lastChildCount;
    private RectTransform[] children;
    private RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!EditorApplication.isPlaying)
            CheckForChanges();
    }
#endif

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

    public void CheckForChanges()
    {
        if (!ChildrenChanged()) return;

        UpdateChildrenArray();

        float minY = -minimumHeight + bottomPadding;

        // get minimum y of all children
        foreach (RectTransform child in children)
        {
            Vector2 scale = child.sizeDelta;

            Vector2 position = child.anchoredPosition;
            float thisMinY = position.y - scale.y;

            if (thisMinY < minY)
                minY = thisMinY;
        }

        minY -= bottomPadding;

        rt.sizeDelta = new(rt.sizeDelta.x, -minY);
    }

    public void UpdateChildrenArray()
    {
        children = this.GetComponentsInDirectChildren<RectTransform>();
    }
}