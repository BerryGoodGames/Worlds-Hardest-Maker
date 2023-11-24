using UnityEngine;

public abstract class LevelObjectManager : MonoBehaviour
{
    protected abstract GameObject Prefab { get; }
    protected abstract Transform Container { get; }
    protected virtual int Layer => LayerManager.Instance.Layers.Entity;

    public abstract void Set();
}