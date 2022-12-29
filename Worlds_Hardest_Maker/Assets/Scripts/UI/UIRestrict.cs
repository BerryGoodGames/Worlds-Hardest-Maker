using UnityEngine;

public class UIRestrict : MonoBehaviour
{
    public float left;
    public float right;
    public float top;
    public float bottom;

    private Vector2 canvasSize;
    private RectTransform rt;

    private void Awake()
    {
        canvasSize = ReferenceManager.Instance.canvas.GetComponent<RectTransform>().rect.size;
        rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        Vector2 pos = rt.localPosition;
        Vector2 pivot = rt.pivot;
        Vector2 size = rt.rect.size;

        float x = Mathf.Clamp(pos.x, pivot.x * size.x - canvasSize.x * 0.5f + left,
            canvasSize.x * 0.5f - (1 - pivot.x) * size.x - right);
        float y = Mathf.Clamp(pos.y, pivot.y * size.y - canvasSize.y * 0.5f + bottom,
            canvasSize.y * 0.5f - (1 - pivot.y) * size.y - top);

        rt.localPosition = new(x, y);
    }
}