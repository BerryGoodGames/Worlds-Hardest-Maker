using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public abstract class AnchorBlock
{
    protected AnchorController Anchor;
    protected bool IsLocked;
    
    public AnchorBlockController Controller { get; set; }
    
    protected AnchorBlock(AnchorController anchor, bool isLocked)
    {
        Anchor = anchor;
        IsLocked = isLocked;
    }
    
    public abstract string TypeID { get; }
    
    public void CreateAnchorBlockObject(Transform parent, 
        RectTransform chainContainer, 
        AnchorBlockConnectorController connectorController,
        AnchorBlockCatalog catalog,
        IObjectResolver diContainer)
    {
        // create object
        GameObject prefab = catalog.GetPrefabByID(TypeID);
        GameObject anchorBlock = Object.Instantiate(prefab, parent);
        
        diContainer.InjectGameObject(anchorBlock);
        
        // set values in object
        Controller = anchorBlock.GetComponent<AnchorBlockController>();
        SetControllerValues(Controller);
        Controller.IsLocked = IsLocked;
        Controller.Block = this;
        
        // restrict
        UIRestrictInRectTransform restrict = anchorBlock.GetComponent<UIRestrictInRectTransform>();
        restrict.RectTransform = chainContainer;
        
        // rebuild
        RectTransform anchorBlockRectTransform = (RectTransform)anchorBlock.transform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(anchorBlockRectTransform);
        
        // move anchor connector
        connectorController.UpdateYAtEndOfFrame();
    }
    
    public abstract void Execute();
    protected abstract void SetControllerValues(AnchorBlockController c);
    public abstract AnchorBlockData GetData();
}