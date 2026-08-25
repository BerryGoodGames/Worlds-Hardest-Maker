using JetBrains.Annotations;
using UnityEngine;

public interface ILevelObjectFactory<T> where T : LevelObjectController
{
    public T Create(Vector2 position, [CanBeNull] AnchorController sheet);
}