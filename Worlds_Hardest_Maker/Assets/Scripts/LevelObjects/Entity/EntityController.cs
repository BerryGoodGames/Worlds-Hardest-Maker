using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class EntityController : LevelObjectController
{
    [FormerlySerializedAs("isAttachable")] [InitializationField] public bool IsAttachable = true;
    [EnableIf(nameof(IsAttachable))] [InitializationField] public Transform AttachmentHolder;
    
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
    
    public static bool TryGetController(Component component, out EntityController entityController) =>
        component.TryGetComponent(out entityController)
        || (entityController = component.GetComponentInChildren<EntityController>()) != null
        || (entityController = component.GetComponentInParent<EntityController>()) != null;
}