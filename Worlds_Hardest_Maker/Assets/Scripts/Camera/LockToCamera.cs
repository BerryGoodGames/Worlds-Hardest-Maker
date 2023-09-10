using UnityEngine;

public class LockToCamera : MonoBehaviour
{
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;

    private Vector2 offset;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        if (cam != null) offset = transform.position - cam.transform.position;
    }

    private void LateUpdate() =>
        transform.position = new(
            lockX ? cam.transform.position.x + offset.x : transform.position.x,
            lockY ? cam.transform.position.y + offset.y : transform.position.y
        );
}