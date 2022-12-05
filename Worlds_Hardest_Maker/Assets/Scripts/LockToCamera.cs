using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockToCamera : MonoBehaviour
{
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;

    private Vector2 offset;

    private void Awake()
    {
        offset = transform.position - Camera.main.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = new(
            lockX ? Camera.main.transform.position.x + offset.x : transform.position.x,
            lockY ? Camera.main.transform.position.y + offset.y : transform.position.y
        );
    }
}
