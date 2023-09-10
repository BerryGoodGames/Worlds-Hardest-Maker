using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockDragDrop : MonoBehaviour
{
    [SerializeField] private bool active = true;

    private Vector2 offset;
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

        AnchorBlockManager.Instance.DraggedBlock = anchorBlockController;
        AnchorBlockManager.Instance.DraggingBlock = true;

        anchorBlockController.TrimFromCurrentChain();

        offset = mousePos - (Vector2)transform.position;

        // update anchor connector y size
        RectTransform anchorConnectorRt =
            (RectTransform)ReferenceManager.Instance.AnchorBlockConnectorController.transform;
        anchorConnectorRt.sizeDelta = new(anchorConnectorRt.sizeDelta.x,
            ((RectTransform)AnchorBlockManager.Instance.DraggedBlock.transform).rect.height);
    }

    private void OnEndDrag()
    {
        if (!active || !gameObject.activeInHierarchy) return;

        AnchorBlockManager.CheckConnectorInsert();
        AnchorBlockManager.CheckBlockInsert();

        MouseOverUIPointer mouseOverUI =
            ReferenceManager.Instance.AnchorBlockChainContainer.GetComponent<MouseOverUIPointer>();
        if (mouseOverUI.Over)
        {
            // push anchor block in container
            AnchorBlockManager.Instance.DraggedBlock.GetComponent<UIRestrictInRectTransform>().Restrict();
        }
        else
        {
            // discard anchor block
            anchorBlockController.Delete();
        }

        ReferenceManager.Instance.AnchorBlockFitter.CheckForChanges();

        // reset values
        AnchorBlockManager.Instance.DraggedBlock = null;
        AnchorBlockManager.Instance.DraggingBlock = false;
    }

    #region Events

    public void OnDragEvent(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        if (pointerData.button != PointerEventData.InputButton.Left) return;
        OnDrag(pointerData.position);
    }

    public void OnBeginEvent(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        if (pointerData.button != PointerEventData.InputButton.Left) return;
        OnBeginDrag(pointerData.position);
    }

    public void OnEndEvent(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        if (pointerData.button != PointerEventData.InputButton.Left) return;
        OnEndDrag();
    }

    #endregion

    public void BeginDrag() => StartCoroutine(Drag());

    private IEnumerator Drag()
    {
        if (!active) yield break;

        OnBeginDrag(Input.mousePosition);
        while (true)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                OnEndDrag();
                // restrict.RectTransform = ReferenceManager.Instance.AnchorBlockChainContainer;
                restrict.enabled = true;
                yield break;
            }

            OnDrag(Input.mousePosition);
            yield return null;
        }
    }
}