using UnityEngine;

public abstract class LevelObjectManager : MonoBehaviour, IPlaceable
{
    protected abstract GameObject Prefab { get; }
    protected abstract Transform Container { get; }
    protected virtual int Layer => LayerManager.Instance.Layers.Entity;

    public virtual void Place(Vector2 worldPosition) =>
        Instantiate(Prefab, worldPosition, Quaternion.identity, Container);
}