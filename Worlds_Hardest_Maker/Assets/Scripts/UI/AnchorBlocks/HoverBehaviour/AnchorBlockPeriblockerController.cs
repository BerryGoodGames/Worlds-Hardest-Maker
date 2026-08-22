using MyBox;
using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class AnchorBlockPeriblockerController : MonoBehaviour
{
    [HideInInspector] public MouseOverUIRect MouseOverUIRect;

    [SerializeField] [InitializationField] [MustBeAssigned] private ChainController mainChainController;    
    [SerializeField] [InitializationField] [MustBeAssigned] private AnchorBlockPreviewController anchorBlockPreview;
    
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
        
        ((RectTransform)transform).sizeDelta = new(width, height);
    }
    
    public void UpdateSize(int stringIndex)
    {
        if (stringIndex == -1)
        {
            UpdateSize(null);
            return;
        }
        
        AnchorBlockController hoveredBlock = mainChainController.GetAnchorBlockByChainIndex(stringIndex);
        UpdateSize(hoveredBlock);
    }
    
    public void UpdateSize()
    {
        int anchorBlockIndex = AnchorBlockManager.Instance.HoveredBlockIndex;
        
        if (anchorBlockIndex >= mainChainController.transform.childCount - 2)
        {
            UpdateSize(null);
            return;
        }
        
        UpdateSize(anchorBlockIndex + 1);
    }
    
    public void OnUnhover()
    {
        // check if any hoverable object is hovered (anchor blocks or anchor connector or periblocker)
        if (AnchorBlockManager.Instance.IsAnyBlockHovered() 
            || AnchorBlockManager.Instance.IsConnectorHovered 
            || AnchorBlockManager.Instance.IsPeriblockerHovered) return;
        
        // disable preview
        AnchorBlockManager.Instance.HoveredBlockIndex = -1;
        anchorBlockPreview.Deactivate();
    }
    
    public void Init()
    {
        MouseOverUIRect = GetComponent<MouseOverUIRect>();
        MouseOverUIRect.OnUnhovered += () => AnchorBlockManager.Instance.ExecutePeriblockerOnUnhover = true;
    }
}