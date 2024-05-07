using System;
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
        if (cam != null)
        {
            // offset = transform.position - cam.transform.position;
            offset = cam.WorldToScreenPoint(transform.position);
        }
    }
    
    private void LateUpdate()
    {
        Vector3 screenPosition = cam.ScreenToWorldPoint(offset);
        transform.position = new(
            lockX ? screenPosition.x : transform.position.x,
            lockY ? screenPosition.y : transform.position.y
        );
    }
}