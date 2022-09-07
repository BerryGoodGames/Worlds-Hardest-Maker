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

        // dividing by scale of ball for some reason???? 
        float scl = (float)(6.5 / cam.orthographicSize / 0.64);
        rt.localScale = new(scl, scl);
    }
}
