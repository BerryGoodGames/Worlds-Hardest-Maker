using UnityEngine;

public static class SheetUtils
{
    public enum SheetCheckingScope
    {
        Self, Parent, SelfOrParent
    }
    
    public static bool Exists(Component controller, ISheet sheet)
    {
        bool globalSheet = sheet.IsGlobal;
        
        bool hasEntityController = EntityController.TryGetController(controller, out EntityController entityController);
        
        if (hasEntityController && !entityController.IsAttachable) return globalSheet;
        
        bool hasAttachment = (hasEntityController ? entityController.AttachmentHolder : controller).TryGetComponent(out AnchorAttachment attachment);

        return (globalSheet && !hasAttachment) || (hasAttachment && sheet is AnchorSheet anchorSheet && anchorSheet.Anchor == attachment.Anchor);
    }
    
    public static ISheet ResolveFor(Component component)
    {
        return component.TryGetComponent(out AnchorAttachment attachment)
            ? attachment.Anchor.OwnSheet
            : GlobalSheet.Instance;
    }
}