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

    public void Attach(LevelObjectController target, AnchorController anchor, bool forceNewParent = true)
    {
        Transform attachTransform = target.AttachmentTarget;
        AnchorAttachment attachment = attachTransform.gameObject.GetOrAddComponent<AnchorAttachment>();

        diContainer.Inject(attachment);

        attachment.Anchor = anchor;
        if (forceNewParent) attachTransform.SetParent(anchor.AttachmentContainer);
        anchor.RegisterAttachment(attachment);
    }

    public void Detach(LevelObjectController target, Transform globalFallbackContainer)
    {
        Transform attachTransform = target.AttachmentTarget;
        if (!attachTransform.TryGetComponent(out AnchorAttachment attachment)) return;

        attachment.Anchor.UnregisterAttachment(attachment);
        attachment.ReturnToOriginalLayer();
        Object.Destroy(attachment);
        attachTransform.SetParent(globalFallbackContainer);
    }

    public bool IsAttachedTo(LevelObjectController target, ISheet sheet)
    {
        return target.AttachmentTarget.TryGetComponent(out AnchorAttachment attachment)
            ? sheet is AnchorSheet a && a.Anchor == attachment.Anchor
            : sheet.IsGlobal;
    }
}