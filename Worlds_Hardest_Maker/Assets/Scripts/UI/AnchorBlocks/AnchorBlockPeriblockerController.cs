using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class AnchorBlockPeriblockerController : MonoBehaviour
{
    [HideInInspector] public MouseOverUIRect MouseOverUIRect;

    /// <summary>
    ///     Resizes itself to <c>anchorBlock</c>
    /// </summary>
    /// <param name="anchorBlock">The anchor block it should resize to, if <c>null</c> passed then sets width and height to 0</param>
    public void UpdateSize(AnchorBlockController anchorBlock)
    {
        (float width, float height) = (0, 0);

        if (anchorBlock != null)
        {
            Rect anchorBlockRect = ((RectTransform)anchorBlock.transform).rect;

            (width, height) = (anchorBlockRect.width, anchorBlockRect.height);
        }

        ((RectTransform)transform).sizeDelta = new Vector2(width, height);
    }

    public void UpdateSize(int stringIndex)
    {
        if (stringIndex == -1)
        {
            UpdateSize(null);
            return;
        }

        ChainController mainChain = ReferenceManager.Instance.MainChainController;
        AnchorBlockController hoveredBlock = mainChain.GetAnchorBlockByChainIndex(stringIndex);
        UpdateSize(hoveredBlock);
    }

    public void UpdateSize()
    {
        Transform mainChain = ReferenceManager.Instance.MainChainController.transform;
        int anchorBlockIndex = AnchorBlockManager.Instance.HoveredBlockIndex;

        if (anchorBlockIndex >= mainChain.childCount - 2)
        {
            UpdateSize(null);
            return;
        }

        UpdateSize(anchorBlockIndex + 1);
    }

    public void OnUnhover()
    {
        // check if any hoverable object is hovered (anchor blocks or anchor connector or periblocker)
        if (AnchorBlockManager.IsAnyBlockHovered() || AnchorBlockManager.Instance.IsConnectorHovered ||
            AnchorBlockManager.Instance.IsPeriblockerHovered) return;

        // disable preview
        AnchorBlockManager.Instance.HoveredBlockIndex = -1;
        ReferenceManager.Instance.AnchorBlockPreview.Deactivate();
    }

    public void Init()
    {
        MouseOverUIRect = GetComponent<MouseOverUIRect>();
        MouseOverUIRect.OnUnhovered += () => AnchorBlockManager.Instance.ExecutePeriblockerOnUnhover = true;
    }
}