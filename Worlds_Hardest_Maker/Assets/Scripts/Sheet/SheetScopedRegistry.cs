using System;
using System.Collections.Generic;

public class SheetScopedRegistry<T> where T : LevelObjectController
{
    private readonly Dictionary<ISheet, List<T>> buckets = new();

    public IReadOnlyList<T> Get(ISheet sheet)
    {
        return buckets.TryGetValue(sheet, out List<T> list) ? list : Array.Empty<T>();
    }

    public void AddItem(ISheet sheet, T item)
    {
        if (!buckets.TryGetValue(sheet, out List<T> list))
        {
            list = new();
            buckets[sheet] = list;
        }
        list.Add(item);
    }

    public bool RemoveItem(ISheet sheet, T item)
    {
        return buckets.TryGetValue(sheet, out List<T> list) && list.Remove(item);
    }
}