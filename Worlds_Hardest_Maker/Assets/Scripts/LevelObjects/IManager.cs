using UnityEngine;

public interface IManager<out T> where T : LevelObjectController
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