using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public abstract Data GetData();

    public virtual Vector2 GetPosition() => transform.position;
}