using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface IManager
{
    public bool CorrespondsToEditMode(EditMode compare);
    
    public static bool IsInSheet(Component controller, AnchorController sheet)
    {
        bool globalSheet = sheet == null;
        
        if (!controller.TryGetComponent(out EntityController entityController))
        {
            Debug.LogWarning("Could not find entity controller when trying to check if entity is in sheet");
            return false;
        }
        
        bool hasAttachment = entityController.AttachmentHolder.TryGetComponent(out AnchorAttachment attachment);
        return (globalSheet && !hasAttachment) || (hasAttachment && !globalSheet && attachment.Anchor == sheet);
    }
}

public interface IManager<out T> : IManager where T : LevelObjectController
{
    public T Set(ManagerParameters args) => SetInSheet(ManagerParameters.GetCurrentSheetParams(args));
    public T SetInSheet(ManagerParameters args);
    
    public T Get(Vector2 position) => GetInSheet(position, PlaceManager.GetCurrentSheet());
    public T GetInSheet(Vector2 position, [CanBeNull] AnchorController sheet);
    
    public T Instantiate(ManagerParameters args) => InstantiateInSheet(ManagerParameters.GetCurrentSheetParams(args));
    public T InstantiateInSheet(ManagerParameters args);
    
    public bool IsThere(Vector2 position) => Get(position) != null;
    public bool IsThereInSheet(Vector2 position, [CanBeNull] AnchorController sheet) => GetInSheet(position, sheet) != null;
    
    public List<Data> Serialize(List<Data> levelData);
}

public struct ManagerParameters
{
    public Vector2 Position { get; set; }
    public FieldMode FieldMode { get; set; }
    public int Rotation { get; set; }
    public KeyColor KeyColor { get; set; }
    public bool SurroundWithStartFields { get; set; }
    public AnchorController Sheet { get; set; }
    
    public static ManagerParameters GetCurrentSheetParams(ManagerParameters args)
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