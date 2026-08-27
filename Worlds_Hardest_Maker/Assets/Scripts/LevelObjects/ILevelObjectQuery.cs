using JetBrains.Annotations;
using UnityEngine;

public interface ILevelObjectQuery<T> where T : LevelObjectController
{
    [CanBeNull] T Find(Vector2 position, [CanBeNull] AnchorController sheet);
    bool Exists(Vector2 position, [CanBeNull] AnchorController sheet);
}