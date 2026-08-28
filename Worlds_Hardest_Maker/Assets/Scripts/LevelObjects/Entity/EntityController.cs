using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class EntityController : LevelObjectController
{
    [FormerlySerializedAs("isAttachable")] [InitializationField] public bool IsAttachable = true;
    [EnableIf(nameof(IsAttachable))] [InitializationField] public Transform AttachmentHolder;
    
    [MyBox.ReadOnly] public ISheet Sheet;
    
    public override void Delete()
    {
        if ((IsAttached && Sheet is AnchorSheet anchorSheet && anchorSheet.Anchor.IsAttaching)
            || (!IsAttached && !AnchorAttachManager.Instance.InAttachMode)) base.Delete();
    }
    
    protected virtual void Start()
    {
        if (!EditMode.AnchorSheetAvailable) return;
        
        AnchorAttachment attachment = AttachmentHolder.GetComponent<AnchorAttachment>();
        IsAttached = attachment != null;
        
        // TODO refactor: registering?
        if (IsAttached) Sheet = new AnchorSheet(attachment.Anchor);
        else Sheet = GlobalSheet.Instance;
    }
    
    public static bool TryGetController(Component component, out EntityController entityController) =>
        component.TryGetComponent(out entityController)
        || (entityController = component.GetComponentInChildren<EntityController>()) != null
        || (entityController = component.GetComponentInParent<EntityController>()) != null;
}