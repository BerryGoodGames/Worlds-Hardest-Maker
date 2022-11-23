using UnityEngine;

public class UIRestrict : MonoBehaviour
{
    public float left;
    public float right;
    public float top;
    public float bottom;

    private Vector2 canvasSize;

    private void Awake()
    {
        canvasSize = GameManager.Instance.Canvas.GetComponent<RectTransform>().rect.size;
    }

    private void LateUpdate()
    {
        RectTransform rt = GetComponent<RectTransform>();

        Vector2 pos = rt.localPosition;
        Vector2 pivot = rt.pivot;
        Vector2 size = rt.rect.size;

        //float x = Mathf.Clamp(pos.x, pivot.x * size.x + left, canvasSize.x - (1 - pivot.x) * size.x - right);
        //float y = Mathf.Clamp(pos.y, pivot.y * size.y + bottom, canvasSize.y - (1 - pivot.y) * size.y - top);
        float x = Mathf.Clamp(pos.x, pivot.x * size.x - canvasSize.x * 0.5f + left, canvasSize.x * 0.5f - (1 - pivot.x) * size.x - right);
        float y = Mathf.Clamp(pos.y, pivot.y * size.y - canvasSize.y * 0.5f + bottom, canvasSize.y * 0.5f - (1 - pivot.y) * size.y - top);

        rt.localPosition = new(x, y);
    }
}
