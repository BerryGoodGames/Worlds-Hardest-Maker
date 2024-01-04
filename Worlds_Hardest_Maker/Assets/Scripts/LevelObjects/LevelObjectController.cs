using UnityEngine;

public abstract class LevelObjectController : MonoBehaviour
{
    public abstract EditMode EditMode { get; }

    public abstract Data GetData();

    public virtual void Delete() => Destroy(gameObject);
}