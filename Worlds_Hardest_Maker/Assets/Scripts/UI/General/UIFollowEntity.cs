using UnityEngine;

public class UIFollowEntity : MonoBehaviour
{
    [HideInInspector] public GameObject Entity;

    [HideInInspector] public Vector2 Offset;

    private Transform target;
    private RectTransform rt;
    private Camera cam;

    private void Start()
    {
        target = Entity.transform;
        rt = GetComponent<RectTransform>();
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        rt.position = cam.WorldToScreenPoint((Vector2)target.position + Offset);

        float scl = 10.0f / cam.orthographicSize;
        rt.localScale = new(scl, scl);
    }
}