using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDragDrop : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private RectTransform rt;

    private Vector2 offset;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Drag(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out Vector2 position
        );

        transform.position = canvas.transform.TransformPoint(position) - (Vector3)offset;
    }
    public void BeginDrag(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        offset = pointerData.position - (Vector2)transform.position;
    }
}