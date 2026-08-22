using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class AnchorBlockViewFactory
{
    [Inject] private IObjectResolver diContainer;
    
    public void Create(Transform parent, 
        RectTransform chainContainer, 
        AnchorBlockConnectorController connectorController,
        AnchorBlockCatalog catalog,
        AnchorBlock block)
    {
        // create object
        GameObject prefab = catalog.GetPrefabByID(block.TypeID);
        GameObject anchorBlock = Object.Instantiate(prefab, parent);
        
        diContainer.InjectGameObject(anchorBlock);
        
        // set values in object
        block.Controller = anchorBlock.GetComponent<AnchorBlockController>();
        block.SetControllerValues(block.Controller);
        block.Controller.IsLocked = block.IsLocked;
        block.Controller.Block = block;
        
        // restrict
        UIRestrictInRectTransform restrict = anchorBlock.GetComponent<UIRestrictInRectTransform>();
        restrict.RectTransform = chainContainer;
        
        // rebuild
        RectTransform anchorBlockRectTransform = (RectTransform)anchorBlock.transform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(anchorBlockRectTransform);
        
        // move anchor connector
        connectorController.UpdateYAtEndOfFrame();
    }
}