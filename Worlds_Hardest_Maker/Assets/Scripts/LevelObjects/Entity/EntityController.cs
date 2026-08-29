using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class EntityController : LevelObjectController
{
    [FormerlySerializedAs("isAttachable")] [InitializationField] public bool IsAttachable = true;
    [EnableIf(nameof(IsAttachable))] [InitializationField] public Transform AttachmentHolder;
    
    public override Transform AttachmentTarget => AttachmentHolder != null ? AttachmentHolder : transform;
    
    [MyBox.ReadOnly] public ISheet Sheet;
    
    public override void Delete()
    {
        if ((IsAttached && Sheet is AnchorSheet anchorSheet && anchorSheet.Anchor.IsAttaching)
            || (!IsAttached && !AnchorAttachManager.Instance.InAttachMode)) base.Delete();
    }
    
    protected virtual void Start()
    {
        if (!EditMode.AnchorSheetAvailable) return;
        
        Sheet = SheetUtils.ResolveFor(AttachmentHolder);
        IsAttached = Sheet is AnchorSheet;
    }
    
    public static bool TryGetController(Component component, out EntityController entityController) =>
        component.TryGetComponent(out entityController)
        || (entityController = component.GetComponentInChildren<EntityController>()) != null
        || (entityController = component.GetComponentInParent<EntityController>()) != null;
}