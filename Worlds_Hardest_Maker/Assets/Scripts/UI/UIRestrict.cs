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
        print(GameManager.Instance.Canvas.GetComponent<RectTransform>().position.x);
    }

    private void LateUpdate()
    {
        RectTransform rt = GetComponent<RectTransform>();

        Vector2 pos = rt.position;
        Vector2 pivot = rt.pivot;
        Vector2 size = rt.rect.size;

        print(pos);
        //float x = Mathf.Clamp(pos.x, pivot.x * size.x + left, canvasSize.x - (1 - pivot.x) * size.x - right);
        //float y = Mathf.Clamp(pos.y, pivot.y * size.y + bottom, canvasSize.y - (1 - pivot.y) * size.y - top);
        float x = Mathf.Clamp(pos.x, left - pivot.x * size.x, canvasSize.x - (1 - pivot.x) * size.x - right);
        float y = Mathf.Clamp(pos.y, pivot.y * size.y + bottom, canvasSize.y - (1 - pivot.y) * size.y - top);

        rt.position = new(x, y);
    }
}
