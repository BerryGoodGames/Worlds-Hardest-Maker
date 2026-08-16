using System.Collections;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class AnchorBlockConnectorController : MonoBehaviour
{
    [HideInInspector] public MouseOverUIRect MouseOverUIRect;

    [SerializeField] [InitializationField] [MustBeAssigned] private AnchorBlockPreviewController anchorBlockPreviewController;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform mainChain;
    
    private void Start()
    {
        MouseOverUIRect = GetComponent<MouseOverUIRect>();
        
        MouseOverUIRect.OnHovered += () => AnchorBlockManager.Instance.ExecuteConnectorOnHover = true;
        MouseOverUIRect.OnUnhovered += () => AnchorBlockManager.Instance.ExecuteConnectorOnUnhover = true;
    }
    
    public void OnHover()
    {
        AnchorBlockManager.Instance.HoveredBlockIndex = mainChain.childCount - 2;
        
        anchorBlockPreviewController.Activate();
    }
    
    public void OnUnhover()
    {
        AnchorBlockManager.Instance.HoveredBlockIndex = -1;
        anchorBlockPreviewController.Deactivate();
    }
    
    public void UpdateY()
    {
        // move anchor connector
        float anchorY = mainChain.localPosition.y;
        
        foreach (RectTransform anchorBlockInChain in mainChain)
        {
            // ignore preview object
            if (anchorBlockInChain.CompareTag("AnchorBlockPreview")) continue;
            
            float height = anchorBlockInChain.rect.height;
            
            anchorY -= height;
        }
        
        RectTransform rt = (RectTransform)transform;
        rt.localPosition = new(rt.localPosition.x, anchorY);
    }
    
    public void UpdateHeight(Vector2 mouseOffset)
    {
        // update anchor connector y size
        RectTransform rt = (RectTransform)transform;
        
        float blockHeight = ((RectTransform)AnchorBlockManager.Instance.DraggedBlock.transform).rect.height;
        float offsetY = mouseOffset.y;
        
        rt.sizeDelta = new(rt.sizeDelta.x, blockHeight * 1.5f - offsetY);
    }
    
    public void UpdateYAtEndOfFrame() => StartCoroutine(UpdateYCoroutine());
    
    private IEnumerator UpdateYCoroutine()
    {
        yield return new WaitForEndOfFrame();
        
        UpdateY();
    }
    
    private void LateUpdate()
    {
        if (MouseOverUIRect.Over && AnchorBlockManager.Instance.HoveredBlockIndex == -1)
        {
            AnchorBlockManager.Instance.HoveredBlockIndex = mainChain.childCount - 2;
        }
    }
}