using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class AnchorBlockFitter : MonoBehaviour
{
    [SerializeField] private float bottomPadding;

    private int lastChildCount;
    private RectTransform[] children;
    private RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
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
        
        float minY = 0;
        

        foreach (RectTransform child in children)
        {
            Vector2 scale = child.sizeDelta;

            Vector2 position = child.anchoredPosition;
            float thisMinY = position.y - scale.y;



            if (thisMinY < minY)
                minY = thisMinY;
        }

        minY -= bottomPadding;

        rt.sizeDelta = new(rt.sizeDelta.x, -minY);
    }
}
