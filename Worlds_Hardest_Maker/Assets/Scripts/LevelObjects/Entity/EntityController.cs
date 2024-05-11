using JetBrains.Annotations;
using MyBox;
using NaughtyAttributes;
using UnityEngine;

public abstract class EntityController : LevelObjectController
{
    [SerializeField] [InitializationField] [UsedImplicitly] private bool isAttachable = true;
    [EnableIf(nameof(isAttachable))] [InitializationField] public Transform AttachmentHolder;
    
    [MyBox.ReadOnly] public AnchorController Sheet;
    
    public override void Delete()
    {
        if ((IsAttached && Sheet.IsAttaching)
            || (!IsAttached && !AnchorAttachManager.Instance.InAttachMode)) base.Delete();
    }
    
    protected virtual void Start()
    {
        if (!EditMode.AnchorAvailable) return;
        
        AnchorAttachment attachment = AttachmentHolder.GetComponent<AnchorAttachment>();
        IsAttached = attachment != null;
        
        if (IsAttached) Sheet = attachment.Anchor;
    }
}