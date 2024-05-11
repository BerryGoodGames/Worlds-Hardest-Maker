using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class EntityController : LevelObjectController
{
    [SerializeField] [InitializationField] [UsedImplicitly] private bool isAttachable = true;
    [ConditionalField(nameof(isAttachable))] [InitializationField] public Transform AttachmentHolder;
    
    [ReadOnly] public AnchorController Sheet;
    
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