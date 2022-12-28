using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowEntity : MonoBehaviour
{
    [HideInInspector] public GameObject entity;
    [HideInInspector] public Vector2 offset;

    private Transform target;
    private RectTransform rt;
    private Camera cam;

    private void Start()
    {
        target = entity.transform;
        rt = GetComponent<RectTransform>();
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        rt.position = cam.WorldToScreenPoint((Vector2)target.position + offset);

        float scl = 10.0f / cam.orthographicSize;
        rt.localScale = new(scl, scl);
    }
}
