using JetBrains.Annotations;
using UnityEngine;

public interface IManager
{
    public static bool IsInSheet(Component controller, AnchorController sheet)
    {
        bool globalSheet = sheet == null;
        
        bool hasEntityController = EntityController.TryGetController(controller, out EntityController entityController);
        
        if (hasEntityController && !entityController.IsAttachable) return globalSheet;
        
        bool hasAttachment = (hasEntityController ? entityController.AttachmentHolder : controller).TryGetComponent(out AnchorAttachment attachment);
        return (globalSheet && !hasAttachment) || (hasAttachment && !globalSheet && attachment.Anchor == sheet);
    }
}

public interface IManager<out T> : IManager where T : LevelObjectController
{
    public T Set(ManagerParameters args) => SetInSheet(ManagerParameters.FromCurrentSheet(args));
    public T SetInSheet(ManagerParameters args);
    
    public T InstantiateInSheet(ManagerParameters args);
}

public struct ManagerParameters
{
    public Vector2 Position { get; set; }
    public FieldMode FieldMode { get; set; }
    public int Rotation { get; set; }
    public KeyColor KeyColor { get; set; }
    public bool SurroundWithStartFields { get; set; }
    public AnchorController Sheet { get; set; }
    
    public static ManagerParameters FromCurrentSheet(ManagerParameters args)
    {
        args.Sheet = PlaceManager.GetCurrentSheet();
        return args;
    }
}