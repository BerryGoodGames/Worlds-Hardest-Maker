using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDragDrop : MonoBehaviour
{
    [SerializeField] private bool active = true;

    private Vector2 offset;
    private Coroutine waitForSnap;
    private AnchorBlockController anchorBlockController;
    private UIRestrictInRectTransform restrict;

    private void Awake()
    {
        anchorBlockController = GetComponent<AnchorBlockController>();
        restrict = GetComponent<UIRestrictInRectTransform>();
    }

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

        AnchorBlockManager.DraggedBlock = anchorBlockController;
        AnchorBlockManager.DraggingBlock = true;

        offset = mousePos - (Vector2)transform.position;
    }

    private void OnEndDrag(Vector2 mousePos)
    {
        if (!active || !gameObject.activeInHierarchy) return;

        waitForSnap = StartCoroutine(WaitForSnap());

        ReferenceManager.Instance.AnchorBlockFitter.CheckForChanges();

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
        restrict.enabled = false;
    }

    private IEnumerator Drag()
    {
        if (!active) yield break;

        OnBeginDrag(Input.mousePosition);
        while (true)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                OnEndDrag(Input.mousePosition);
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                restrict.RectTransform = ReferenceManager.Instance.AnchorBlockStringContainer.GetComponent<RectTransform>();
                restrict.enabled = true;
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
        restrict.enabled = true;
    }
}