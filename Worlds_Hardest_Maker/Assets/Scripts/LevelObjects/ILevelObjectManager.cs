using JetBrains.Annotations;
using UnityEngine;

public interface ILevelObjectManager : ILevelObjectPlacer, ILevelObjectSerializer
{
    public bool Remove(Vector2 position, [CanBeNull] AnchorController sheet);
}