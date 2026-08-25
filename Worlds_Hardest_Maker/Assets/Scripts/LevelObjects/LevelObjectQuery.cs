using UnityEngine;

public class LevelObjectQuery<T> : ILevelObjectQuery<T> where T : LevelObjectController
{
    public T Find(Vector2 position, AnchorController sheet)
    {
        throw new System.NotImplementedException();
    }

    public bool Exists(Vector2 position, AnchorController sheet)
    {
        throw new System.NotImplementedException();
    }
}