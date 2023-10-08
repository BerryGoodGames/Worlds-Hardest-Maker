using MyBox;
using UnityEngine;

public class UIRestrict : MonoBehaviour
{
    public float Left, Right, Top, Bottom;
    [Space] public bool CustomRestrictContainer;
    [ConditionalField(nameof(CustomRestrictContainer))] public RectTransform RestrictContainer;

    private Vector2 canvasSize;
    private RectTransform rt;

    private void Awake()
    {
        canvasSize = (CustomRestrictContainer ? RestrictContainer : (RectTransform)ReferenceManager.Instance.Canvas.transform).rect.size;
        rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        Vector2 pos = rt.localPosition;
        Vector2 pivot = rt.pivot;
        Vector2 size = rt.rect.size;

        float x = Mathf.Clamp(pos.x, pivot.x * size.x - canvasSize.x * 0.5f + Left,
            canvasSize.x * 0.5f - (1 - pivot.x) * size.x - Right);
        float y = Mathf.Clamp(pos.y, pivot.y * size.y - canvasSize.y * 0.5f + Bottom,
            canvasSize.y * 0.5f - (1 - pivot.y) * size.y - Top);

        rt.localPosition = new(x, y);
    }
}