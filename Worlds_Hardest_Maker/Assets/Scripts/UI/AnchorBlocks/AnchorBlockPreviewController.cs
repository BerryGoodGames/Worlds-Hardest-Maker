using MyBox;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(MouseOverUIRect))]
public class AnchorBlockPreviewController : MonoBehaviour
{
    [ReadOnly] public MouseOverUIRect MouseOverUIRect;
    
    public AnchorBlockPeriblockerController Periblocker;
    
    private bool activated;
    
    private IAudioService audioService;
    
    [Inject]
    public void Construct(IAudioService audioService)
    {
        this.audioService = audioService;
    }

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
    
    public void UpdateSiblingIndex()
    {
        transform.SetSiblingIndex(AnchorBlockManager.Instance.HoveredBlockIndex + 1);
        audioService.Play("AnchorBlockBrowse");
    }
    
    public void OnUnhover()
    {
        if (AnchorBlockManager.Instance.IsConnectorHovered) return;
        if (AnchorBlockManager.Instance.IsPeriblockerHovered) return;
        if (AnchorBlockManager.Instance.IsBlockHovered(GetChainIndex())) return;
        
        Deactivate();
        AnchorBlockManager.Instance.HoveredBlockIndex = -1;
    }
    
    private int GetChainIndex() => transform.GetSiblingIndex() - 1;
    
    private void Update()
    {
        if (AnchorBlockManager.Instance.IsPreviewHovered && AnchorBlockManager.Instance.HoveredBlockIndex == -1)
            // correct mistake of overriding index to -1
            AnchorBlockManager.Instance.HoveredBlockIndex = GetChainIndex();
    }
    
    private void LateUpdate()
    {
        if (activated && !AnchorBlockManager.Instance.IsPreviewHovered &&
            !AnchorBlockManager.Instance.IsConnectorHovered &&
            !AnchorBlockManager.Instance.IsPeriblockerHovered && !AnchorBlockManager.Instance.IsAnyBlockHovered()) Deactivate();
    }
    
    private void Awake()
    {
        MouseOverUIRect = GetComponent<MouseOverUIRect>();
        MouseOverUIRect.OnUnhovered += () => AnchorBlockManager.Instance.ExecutePreviewOnUnhover = true;
        
        Periblocker.Init();
        
        gameObject.SetActive(false);
    }
}