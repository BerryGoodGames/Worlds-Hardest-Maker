using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class SizeFitter : MonoBehaviour
{
    [SerializeField] private bool width = true;
    [SerializeField] private bool height = true;

    [Space]
    [Header("Padding")]
    [SerializeField] private float left;
    [SerializeField] private float right;
    [SerializeField] private float top;
    [SerializeField] private float bottom;

    private int lastChildCount;
    private RectTransform[] children;
    private RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        CheckForChanges();
    }
#if UNITY_EDITOR
    private void Update()
    {
        if(!EditorApplication.isPlaying)
            CheckForChanges();
    }
#endif

    private bool ChildrenChanged()
    {
        if (transform.childCount != lastChildCount)
        {
            lastChildCount = transform.childCount;
            return true;
        }

        foreach (RectTransform child in children)
        {
            if (!child.hasChanged) continue;
            child.hasChanged = false;
            return true;
        }

        return false;
    }

    public void CheckForChanges()
    {
        if (!ChildrenChanged()) return;

        children = this.GetComponentsInDirectChildren<RectTransform>();

        float maxX, maxY;
        Vector3 localPosition = rt.anchoredPosition;
        float minX = maxX = localPosition.x;
        float minY = maxY = localPosition.y;

        foreach (RectTransform child in children)
        {
            Vector2 scale = child.sizeDelta;

            Vector2 position = child.anchoredPosition;
            float tempMinX = position.x - (scale.x / 2);
            float tempMaxX = position.x + (scale.x / 2);
            float tempMinY = position.y - (scale.y / 2);
            float tempMaxY = position.y + (scale.y / 2);

            if (tempMinX < minX)
                minX = tempMinX;
            if (tempMaxX > maxX)
                maxX = tempMaxX;

            if (tempMinY < minY)
                minY = tempMinY;
            if (tempMaxY > maxY)
                maxY = tempMaxY;
        }

        maxX += right;
        maxY += top;
        minX -= left;
        minY -= bottom;

        rt.sizeDelta = new(width? maxX - minX : rt.sizeDelta.x, height? maxY - minY : rt.sizeDelta.y);
    }
}
