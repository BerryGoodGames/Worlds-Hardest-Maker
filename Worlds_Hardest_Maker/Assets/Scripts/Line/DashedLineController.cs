using MyBox;
using UnityEngine;

public class DashedLineController : MonoBehaviour
{
    private float spacing;
    private float width;
    
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
        CalculateDashes();
    }

    public void Initialize(float spacing, float width, Material dashedLineMaterial)
    {
        this.spacing = spacing;
        this.width = width;
        lineRenderer.material = dashedLineMaterial;
    }
    
    private void LateUpdate()
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
        float lineSpacing = spacing + width / 2;
        float lineAmount = totalArc.magnitude / lineSpacing;
        float lineWidth = width / lineSpacing;
        
        lineRenderer.material.SetFloat(amountID, lineAmount);
        lineRenderer.material.SetFloat(widthID, lineWidth);
    }
}