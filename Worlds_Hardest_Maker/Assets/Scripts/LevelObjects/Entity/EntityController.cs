using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    public virtual Vector2 Position => transform.position;

    public abstract EditMode EditMode { get; }

    public abstract Data GetData();

    public virtual void Delete() => Destroy(gameObject);
}