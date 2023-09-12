using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class DashedLineController : MonoBehaviour
{
    [Separator("Settings")] 
    public float Spacing;
    public float Width;

    private LineRenderer lineRenderer;

    private Vector2 point0;
    private Vector2 point1;

    private Vector2 prevPoint0;
    private Vector2 prevPoint1;
    private static readonly int amountID = Shader.PropertyToID("_amount");
    private static readonly int widthID = Shader.PropertyToID("_width");

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = ReferenceManager.Instance.DashedLineMaterial;
        CalculateDashes();
    }

    private void Update()
    {
        // check if points changed
        point0 = lineRenderer.GetPosition(0);
        point1 = lineRenderer.GetPosition(1);

        if (point0 == prevPoint0 && point1 == prevPoint1) return;

        CalculateDashes();

        // update previous points
        prevPoint0 = point0;
        prevPoint1 = point1;
    }

    [ButtonMethod]
    public void CalculateDashes()
    {
        Vector2 totalArc = point1 - point0;
        float lineSpacing = Spacing + Width / 2;
        float lineAmount = totalArc.magnitude / lineSpacing;
        float lineWidth = Width / lineSpacing;
        
        lineRenderer.material.SetFloat(amountID, lineAmount);
        lineRenderer.material.SetFloat(widthID, lineWidth);
    }
}
