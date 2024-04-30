using MyBox;
using UnityEngine;

public abstract class EntityController : LevelObjectController
{
    [SerializeField] [InitializationField] private bool isAttachable = true;
    [ConditionalField(nameof(isAttachable))] [InitializationField] public Transform AttachmentHolder;
    
    private bool isAttached;
    private AnchorController sheet;
    
    public override void Delete()
    {
        if (isAttached && sheet.IsAttaching) base.Delete();
        if (!isAttached && !AnchorAttachManager.Instance.InAttachMode) base.Delete();
    }

    protected virtual void Start()
    {
        if (!EditMode.AnchorAvailable) return;
        
        AnchorAttachment attachment = AttachmentHolder.GetComponent<AnchorAttachment>();
        isAttached = attachment != null;
        
        if (isAttached)
        {
            sheet = attachment.Anchor;
        }
    }
}