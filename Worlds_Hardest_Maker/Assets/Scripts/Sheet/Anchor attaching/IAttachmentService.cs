using UnityEngine;

public interface IAttachmentService
{
    void Attach(LevelObjectController target, AnchorController anchor);
    void Detach(LevelObjectController target, Transform globalFallbackContainer);
    bool IsAttachedTo(LevelObjectController target, ISheet sheet);
}