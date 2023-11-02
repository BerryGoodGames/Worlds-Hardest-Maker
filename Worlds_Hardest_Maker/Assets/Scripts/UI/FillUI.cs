using System;
using MyBox;
using UnityEngine;

public class FillUI : MonoBehaviour
{
    [MustBeAssigned] [InitializationField] [SerializeField] private RectTransform content;
    [MustBeAssigned] [InitializationField] [SerializeField] private RectTransform fill;

    private float prevOffsetMin = 0;
    
    private void Update()
    {
        if (Math.Abs(content.offsetMin.y - prevOffsetMin) < 0.1f) return;

        prevOffsetMin = content.offsetMin.y;
        
        content.hasChanged = false;
        
        fill.offsetMax = new(fill.offsetMax.x, content.offsetMin.y);
    }
}
