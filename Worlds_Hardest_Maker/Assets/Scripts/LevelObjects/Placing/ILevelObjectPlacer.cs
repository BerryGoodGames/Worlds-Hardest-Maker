public interface ILevelObjectPlacer
{
    public bool CanHandle(EditMode editMode);
    public PlacementResult Place(PlacementRequest request);
}