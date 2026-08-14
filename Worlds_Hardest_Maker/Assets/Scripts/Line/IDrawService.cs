using System.Collections.Generic;
using UnityEngine;

public interface IDrawService
{
    public float Weight { get; }
    public Color Fill { get; }
    public bool RoundedCorners { get; }
    public int LayerID { get; }
    public int OrderInLayer { get; }

    public LineRenderer DrawRect(float x, float y, float width, float height, bool alignCenter, Transform parent);
    public LineRenderer DrawCircle(Vector2 origin, float radius, Transform parent);

    public LineRenderer DrawCircle(float x, float y, float radius, Transform parent)
    {
        return DrawCircle(new(x, y), radius, parent);
    }
    
    public LineRenderer DrawLine(float x1, float y1, float x2, float y2, Transform parent = null)
    {
        return DrawLine(new(x1, y1), new(x2, y2), parent);
    }

    public LineRenderer DrawLine(Vector2 point1, Vector2 point2, Transform parent);

    public LineRenderer DrawDashedLine(Vector2 start, Vector2 end, float width, float spacing, Transform parent);

    public (Vector2 arrowVertex1, Vector2 arrowVertex2, Vector2 arrowCenter) GetArrowHeadPoints(Vector2 start, Vector2 end);
    
    public List<Vector2> GetCirclePoints(Vector2 origin, float radius, int accuracy);

    public void SetFill(float r, float g, float b);

    public void SetFill(Color color);

    public void SetWeight(float setWeight);

    public void SetRoundedCorners(bool set);

    public void SetLayerID(int id);
    public void SetLayerName(string name);

    public void SetOrderInLayer(int order);
}