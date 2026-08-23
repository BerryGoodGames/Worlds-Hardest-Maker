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
    
    public T Get(Vector2 position) => GetInSheet(position, PlaceManager.GetCurrentSheet());
    public T GetInSheet(Vector2 position, [CanBeNull] AnchorController sheet);
    
    public T Instantiate(ManagerParameters args) => InstantiateInSheet(ManagerParameters.FromCurrentSheet(args));
    public T InstantiateInSheet(ManagerParameters args);
    
    public bool IsThere(Vector2 position) => Get(position) != null;
    public bool IsThereInSheet(Vector2 position, [CanBeNull] AnchorController sheet) => GetInSheet(position, sheet) != null;
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

public interface IManagerSelectable
{
    public void Select(Vector2 position);
}

public interface IManagerPlaceRestrictable
{
    public bool CanPlace(Vector2 position);
    public bool CanPlaceInSheet(Vector2 position, [CanBeNull] AnchorController sheet);
}