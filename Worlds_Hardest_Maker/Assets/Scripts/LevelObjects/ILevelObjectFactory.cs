public interface ILevelObjectFactory<T> where T : LevelObjectController
{
    // TODO: remove ManagerParameters
    public T Create(ManagerParameters args);
    // public T Create(Vector2 position, [CanBeNull] AnchorController sheet);
}