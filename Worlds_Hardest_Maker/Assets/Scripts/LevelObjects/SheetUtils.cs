using UnityEngine;

public static class SheetUtils
{
    public enum SheetCheckingScope
    {
        Self, Parent, SelfOrParent
    }
    
    public static bool Exists(Component controller, AnchorController sheet)
    {
        bool globalSheet = sheet == null;
        
        bool hasEntityController = EntityController.TryGetController(controller, out EntityController entityController);
        
        if (hasEntityController && !entityController.IsAttachable) return globalSheet;
        
        bool hasAttachment = (hasEntityController ? entityController.AttachmentHolder : controller).TryGetComponent(out AnchorAttachment attachment);

        return (globalSheet && !hasAttachment) || (hasAttachment && !globalSheet && attachment.Anchor == sheet);
    }
}