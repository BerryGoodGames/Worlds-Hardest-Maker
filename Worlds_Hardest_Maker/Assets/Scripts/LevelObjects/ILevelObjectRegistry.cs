using System.Collections.Generic;

public interface ILevelObjectRegistry<T> where T : LevelObjectController
{
    IReadOnlyList<T> All { get; }
    void Register(T instance);
    void Unregister(T instance);
}