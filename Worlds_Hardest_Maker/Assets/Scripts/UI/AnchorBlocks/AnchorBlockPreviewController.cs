using MyBox;
using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class AnchorBlockPreviewController : MonoBehaviour
{
    [ReadOnly] public MouseOverUIRect MouseOverUIRect;

    public AnchorBlockPeriblockerController Periblocker;

    private bool activated;

    public void Activate()
    {
        if (!AnchorBlockManager.Instance.DraggingBlock) return;

        Periblocker.UpdateSize();

        UpdateSize();
        UpdateSiblingIndex();
        gameObject.SetActive(true);

        activated = true;
    }

    public void Deactivate()
    {
        if (!activated) return;

        gameObject.SetActive(false);

        activated = false;
    }

    /// <summary>
    ///     Resizes itself to the block currently dragged
    /// </summary>
    public void UpdateSize()
    {
        AnchorBlockController draggedBlock = AnchorBlockManager.Instance.DraggedBlock;
        Rect draggedBlockRect = ((RectTransform)draggedBlock.transform).rect;

        (float width, float height) = (draggedBlockRect.width, draggedBlockRect.height);

        ((RectTransform)transform).sizeDelta = new Vector2(width, height);
    }

    public void UpdateSiblingIndex() => transform.SetSiblingIndex(AnchorBlockManager.Instance.HoveredBlockIndex + 1);

    public void OnUnhover()
    {
        if (AnchorBlockManager.IsConnectorHovered) return;
        if (AnchorBlockManager.IsPeriblockerHovered) return;
        if (AnchorBlockManager.IsBlockHovered(GetChainIndex())) return;

        Deactivate();
        AnchorBlockManager.Instance.HoveredBlockIndex = -1;
    }

    private int GetChainIndex() => transform.GetSiblingIndex() - 1;

    private void Update()
    {
        // if (!AnchorBlockManager.Instance.DraggingBlock && gameObject.activeSelf) Deactivate();

        if (AnchorBlockManager.IsPreviewHovered && AnchorBlockManager.Instance.HoveredBlockIndex == -1)
        {
            // correct mistake of overriding index to -1
            AnchorBlockManager.Instance.HoveredBlockIndex = GetChainIndex();
        }
    }

    private void LateUpdate()
    {
        if (activated && !AnchorBlockManager.IsPreviewHovered &&
            !AnchorBlockManager.IsConnectorHovered &&
            !AnchorBlockManager.IsPeriblockerHovered && !AnchorBlockManager.IsAnyBlockHovered()) Deactivate();
    }

    private void Awake()
    {
        MouseOverUIRect = GetComponent<MouseOverUIRect>();
        MouseOverUIRect.OnUnhovered += () => AnchorBlockManager.Instance.ExecutePreviewOnUnhover = true;

        Periblocker.Init();

        gameObject.SetActive(false);
    }
}