using MyBox;
using UnityEngine;

/// <summary>
///     Lets camera scroll to random direction (or with specified angle) with specified speed
///     Attach to main camera
/// </summary>
public class CameraScrolling : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool randomDirection;
    [SerializeField] [ConditionalField(nameof(randomDirection), true)] private float scrollDirAngle;
    private Vector2 dir;

    private void Awake()
    {
        if (randomDirection)
        {
            // generate random unit vector
            float random = Random.Range(0f, 260f);
            dir = new(Mathf.Cos(random), Mathf.Sin(random));
        }
        else
            dir = new(Mathf.Cos(scrollDirAngle * Mathf.PI / 180), Mathf.Sin(scrollDirAngle * Mathf.PI / 180));
    }

    private void FixedUpdate() => transform.position += (Vector3)dir * speed;
}