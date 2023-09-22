using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    public abstract Data GetData();

    public virtual Vector2 GetPosition() => transform.position;

    public virtual void Delete() => Destroy(transform.parent.gameObject);
}