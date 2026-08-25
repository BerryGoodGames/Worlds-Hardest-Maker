using JetBrains.Annotations;
using UnityEngine;

public interface ILevelObjectQuery
{
    public static bool Exists(Component controller, AnchorController sheet)
    {
        bool globalSheet = sheet == null;
        
        bool hasEntityController = EntityController.TryGetController(controller, out EntityController entityController);
        
        if (hasEntityController && !entityController.IsAttachable) return globalSheet;
        
        bool hasAttachment = (hasEntityController ? entityController.AttachmentHolder : controller).TryGetComponent(out AnchorAttachment attachment);
        return (globalSheet && !hasAttachment) || (hasAttachment && !globalSheet && attachment.Anchor == sheet);
    }
}

public interface ILevelObjectQuery<T> where T : LevelObjectController
{
    [CanBeNull] T Find(Vector2 position, [CanBeNull] AnchorController sheet);
    bool Exists(Vector2 position, [CanBeNull] AnchorController sheet);
}