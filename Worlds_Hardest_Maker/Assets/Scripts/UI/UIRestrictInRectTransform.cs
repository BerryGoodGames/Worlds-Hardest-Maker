using UnityEngine;

public class UIRestrictInRectTransform : MonoBehaviour
{
    private RectTransform thisRt;
    public RectTransform RectTransform;
    [SerializeField] private bool suppressWarning;

    private void Awake()
    {
        thisRt = GetComponent<RectTransform>();

        // disable if it doesnt have a rect transform
        if (RectTransform != null) return;
        if (!suppressWarning)
            Debug.LogWarning("There isn't a rect transform passed");
        enabled = false;
    }

    private void LateUpdate()
    {
        Vector2 pos = thisRt.localPosition;
        Vector2 pivot = thisRt.pivot;
        Vector2 size = thisRt.rect.size;

        Vector2 min = RectTransform.rect.min;
        Vector2 max = RectTransform.rect.max;
        // clamp new x and y between the rect transform
        float x = Mathf.Clamp(pos.x, min.x + size.x * pivot.x, max.x - size.x * (1 - pivot.x));
        float y = Mathf.Clamp(pos.y, min.y + size.y * pivot.y, max.y - size.y * (1 - pivot.y));

        thisRt.localPosition = new(x, y);
    }
}