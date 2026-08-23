public interface ILevelObjectPlacer
{
    public bool CanHandle(EditMode editMode);
    public bool Place(PlacementRequest request);
}