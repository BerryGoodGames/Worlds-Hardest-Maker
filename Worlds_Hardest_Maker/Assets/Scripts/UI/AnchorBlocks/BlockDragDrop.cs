using System;
using System.Collections;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDragDrop : MonoBehaviour
{
    [SerializeField] private bool active = true;

    private Vector2 offset;
    private Coroutine waitForSnap;

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
        if (!active || !gameObject.activeInHierarchy) return;

        waitForSnap = StartCoroutine(WaitForSnap());

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

    public void OnEndEvent(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        OnEndDrag(pointerData.position);
    }
    #endregion

    public void BeginDrag()
    {
        StartCoroutine(Drag());
    }

    public void OnSnap()
    {
        StopCoroutine(waitForSnap);
    }

    private IEnumerator Drag()
    {
        if(!active) yield break;

        OnBeginDrag(Input.mousePosition);
        while (true)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                OnEndDrag(Input.mousePosition);
                yield break;
            }
            OnDrag(Input.mousePosition);
            yield return null;
        }
    }

    private IEnumerator WaitForSnap()
    {
        yield return new WaitForEndOfFrame();
        if (transform.parent.TryGetComponent(out StringController stringController))
        {
            Transform connectorContainer = stringController.ConnectorContainer;
            Destroy(connectorContainer.GetChild(transform.GetSiblingIndex() - 1).gameObject);
        }
        transform.SetParent(ReferenceManager.Instance.AnchorBlockStringContainer);
    }
}