using JetBrains.Annotations;
using UnityEngine;

public interface ILevelObjectManager : ILevelObjectPlacer, ILevelObjectSerializer
{
    public LevelObjectController Query(Vector2 position, [CanBeNull] AnchorController sheet);
    public bool Remove(Vector2 position, [CanBeNull] AnchorController sheet);
}