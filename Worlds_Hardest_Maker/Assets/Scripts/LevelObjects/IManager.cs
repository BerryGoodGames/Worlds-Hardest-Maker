using UnityEngine;

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