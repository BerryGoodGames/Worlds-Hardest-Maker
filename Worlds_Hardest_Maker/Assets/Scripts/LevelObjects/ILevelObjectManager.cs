using JetBrains.Annotations;
using UnityEngine;

public interface ILevelObjectManager
{
    public bool CanHandle(EditMode editMode);

    public bool Place(PlacementRequest request);
    public LevelObjectController Query(Vector2 position, [CanBeNull] AnchorController sheet);
    public bool Remove(Vector2 position, [CanBeNull] AnchorController sheet);
}