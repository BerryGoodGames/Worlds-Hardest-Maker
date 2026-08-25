public readonly struct PlacementResult
{
    public bool Successful { get; }
    public LevelObjectController PlacedObject { get; }

    public PlacementResult(bool successful, LevelObjectController placedObject)
    {
        Successful = successful;
        PlacedObject = placedObject;
    }
    
    public static readonly PlacementResult Failed = new(false, null);
    public static PlacementResult Succeeded(LevelObjectController placedObject) => new(true, placedObject);
    public static PlacementResult FromController(LevelObjectController placedObject)
    {
        if (placedObject == null) return Failed;
        return Succeeded(placedObject);
    }
    
    public static implicit operator bool(PlacementResult result) => result.Successful;
}