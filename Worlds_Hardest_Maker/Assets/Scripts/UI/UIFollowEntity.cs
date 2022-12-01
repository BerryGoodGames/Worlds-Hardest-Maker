using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowEntity : MonoBehaviour
{
    [HideInInspector] public GameObject entity;
    [HideInInspector] public Vector2 offset;

    private void LateUpdate()
    {
        Camera cam = Camera.main;

        Transform target = entity.GetComponent<Transform>();

        RectTransform rt = GetComponent<RectTransform>();
        rt.position = cam.WorldToScreenPoint((Vector2)target.position + offset);

        float scl = 10.0f / cam.orthographicSize;
        rt.localScale = new(scl, scl);
    }
}
