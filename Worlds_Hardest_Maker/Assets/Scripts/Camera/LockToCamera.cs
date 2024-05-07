using MyBox;
using UnityEngine;

public class LockToCamera : MonoBehaviour
{
    [SerializeField] [InitializationField] private bool lockX;
    [SerializeField] [InitializationField] private bool lockY;
    [SerializeField] [InitializationField] private bool deactivate;
    
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
        
        if (deactivate) gameObject.SetActive(false);
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