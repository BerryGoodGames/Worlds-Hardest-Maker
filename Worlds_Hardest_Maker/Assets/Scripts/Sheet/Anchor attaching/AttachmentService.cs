using MyBox;
using UnityEngine;
using VContainer;

public class AttachmentService : IAttachmentService
{
    private readonly IObjectResolver diContainer;
    
    public AttachmentService(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }

    public void Attach(LevelObjectController target, AnchorController anchor)
    {
        AnchorAttachment attachment = target.gameObject.GetOrAddComponent<AnchorAttachment>();
        
        diContainer.Inject(attachment);

        attachment.Anchor = anchor;
        target.transform.SetParent(anchor.AttachmentContainer);
        anchor.RegisterAttachment(attachment);
    }

    public void Detach(LevelObjectController target, Transform globalFallbackContainer)
    {
        if (!target.TryGetComponent(out AnchorAttachment attachment)) return;

        attachment.Anchor.UnregisterAttachment(attachment);
        attachment.ReturnToOriginalLayer();
        Object.Destroy(attachment);
        target.transform.SetParent(globalFallbackContainer);
    }

    public bool IsAttachedTo(LevelObjectController target, ISheet sheet)
    {
        return target.TryGetComponent(out AnchorAttachment attachment)
            ? sheet is AnchorSheet a && a.Anchor == attachment.Anchor
            : sheet.IsGlobal;
    }
}