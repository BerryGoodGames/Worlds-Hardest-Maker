using System.Collections.Generic;

public class LevelObjectRegistry<T> : ILevelObjectRegistry<T> where T : LevelObjectController
{
    public IReadOnlyList<T> All => items.AsReadOnly();

    private readonly List<T> items = new();
    
    public void Register(T instance)
    {
        items.Add(instance);
    }

    public void Unregister(T instance)
    {
        items.Remove(instance);
    }
}