using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public abstract partial class AnchorBlockController
{
    [HideInInspector] public MouseOverUIRect MouseOverUI;

    private void HoveringStart()
    {
        MouseOverUI = GetComponent<MouseOverUIRect>();

        MouseOverUI.OnHovered = () => AnchorBlockManager.Instance.ExecuteBlockOnHover = this;
        MouseOverUI.OnUnhovered = () => AnchorBlockManager.Instance.ExecuteBlockOnUnhover = this;
    }

    public void OnHover()
    {
        if (IsSource) return;
        if (!IsInChain()) return;

        int thisIndex = GetChainIndex();

        if (AnchorBlockManager.Instance.HoveredBlockIndex == thisIndex)
        {
            // case: player dragged from preview to this block -> set the preview for the block after this one
            AnchorBlockManager.Instance.HoveredBlockIndex++;
        }
        else
            AnchorBlockManager.Instance.HoveredBlockIndex = thisIndex;

        // activate preview only if locked
        if (!IsLocked) ReferenceManager.Instance.AnchorBlockPreview.Activate();
    }

    public void OnUnhover()
    {
        if (IsSource) return;
        if (!IsInChain()) return;

        if (AnchorBlockManager.Instance.IsPeriblockerHovered) return;
        if (AnchorBlockManager.Instance.IsPreviewHovered) return;

        AnchorBlockManager.Instance.HoveredBlockIndex = -1;
    }
}