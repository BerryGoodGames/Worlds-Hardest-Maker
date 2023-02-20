using System;
using System.Collections;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDragDrop : MonoBehaviour
{
    [SerializeField] private bool active = true;

    private Vector2 offset;

    public void OnDrag(Vector2 mousePos)
    {
        if (!active) return;

        Canvas canvas = ReferenceManager.Instance.Canvas;
        

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            mousePos,
            canvas.worldCamera,
            out Vector2 position
        );

        transform.position = canvas.transform.TransformPoint(position) - (Vector3)offset;
    }

    public void OnDragEvent(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        OnDrag(pointerData.position);
    }

    public void OnBeginDrag(Vector2 mousePos)
    {
        if (!active) return;

        offset = mousePos - (Vector2)transform.position;
    }

    public void OnBeginEvent(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        OnBeginDrag(pointerData.position);
    }

    public void BeginDrag()
    {
        StartCoroutine(Drag());
    }

    public IEnumerator Drag()
    {
        while (true)
        {
            print("1");
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) yield break;
            print("2");
            OnDrag(Input.mousePosition);
            yield return null;
        }
    }
}