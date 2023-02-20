using System;
using System.Collections;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDragDrop : MonoBehaviour
{
    [SerializeField] private bool active = true;

    private Vector2 offset;

    private void OnDrag(Vector2 mousePos)
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

    private void OnBeginDrag(Vector2 mousePos)
    {
        if (!active || !gameObject.activeInHierarchy) return;
        
        AnchorBlockManager.DraggedBlock = GetComponent<AnchorBlockController>();
        AnchorBlockManager.DraggingBlock = true;

        offset = mousePos - (Vector2)transform.position;
    }

    private void OnEndDrag(Vector2 mousePos)
    {
        if (!active) return;

        AnchorBlockManager.DraggedBlock = null;
        AnchorBlockManager.DraggingBlock = false;
    }

    #region Events
    public void OnDragEvent(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        OnDrag(pointerData.position);
    }

    public void OnBeginEvent(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        OnBeginDrag(pointerData.position);
    }
    #endregion

    public void BeginDrag()
    {
        StartCoroutine(Drag());
    }

    private IEnumerator Drag()
    {
        if(!active) yield break;

        OnBeginDrag(Input.mousePosition);
        while (true)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                // waiting for one frame in case it has to be moved to a block string
                yield return null;
                OnEndDrag(Input.mousePosition);
                yield break;
            }
            OnDrag(Input.mousePosition);
            yield return null;
        }
    }
}