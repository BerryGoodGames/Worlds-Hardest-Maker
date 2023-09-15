using System;
using System.Collections.Generic;
using LuLib.Vector;
using MyBox;
using Unity.Collections;
using UnityEngine;

/// <summary>
///     Renders lines / circles / rects: generates objects in container holding LineRenderers
///     <para>Attach to game manager</para>
/// </summary>
public class DrawManager : MonoBehaviour
{
    private static DrawManager instance;

    public static int DefaultLayerID;
    public static int OutlineLayerID;
    public static int BallLayerID;

    private void Awake()
    {
        DefaultLayerID = SortingLayer.NameToID(LayerManager.Instance.SortingLayers.Default);
        OutlineLayerID = SortingLayer.NameToID(LayerManager.Instance.SortingLayers.Outline);
        BallLayerID = SortingLayer.NameToID(LayerManager.Instance.SortingLayers.Ball);

        instance ??= this;
    }

    // Settings for drawing
    public static float Weight { get; private set; } = 0.11f;
    public static Color Fill { get; private set; } = new(0, 0, 0);
    public static bool RoundedCorners { get; private set; } = true;
    public static int LayerID { get; private set; } = DefaultLayerID;
    public static int OrderInLayer { get; private set; }

    /// <summary>
    ///     Generates object containing a LineRenderer forming a rectangle
    /// </summary>
    public static LineRenderer DrawRect(float x, float y, float width, float height, bool alignCenter = false,
        Transform parent = null)
    {
        // generate object
        LineRenderer rect = NewDrawObject("DrawRect", parent);
        rect.positionCount = 5;
        rect.sortingOrder = OrderInLayer;
        rect.sortingLayerID = LayerID;

        // get positions
        Vector2[] positions =
        {
            new(x, y),
            new(x + width, y),
            new(x + width, y + height),
            new(x, y + height),
            new(x, y)
        };

        // set positions
        for (int i = 0; i < positions.Length; i++)
        {
            if (alignCenter)
            {
                positions[i].x -= width * 0.5f;
                positions[i].y -= height * 0.5f;
            }

            rect.SetPosition(i, positions[i]);
        }

        return rect;
    }

    /// <summary>
    ///     Generates object containing a LineRenderer forming a circle
    /// </summary>
    public static LineRenderer DrawCircle(Vector2 origin, float radius, Transform parent = null)
    {
        // generate object
        LineRenderer circle = NewDrawObject("DrawCircle", parent);
        circle.sortingOrder = OrderInLayer;
        circle.sortingLayerID = LayerID;

        // get points of circle
        const int steps = 100;
        List<Vector2> points = GetCirclePoints(origin, radius, steps);

        // set points
        circle.positionCount = steps + 1;
        for (int i = 0; i < points.Count; i++)
        {
            circle.SetPosition(i, points[i]);
        }

        return circle;
    }

    /// <summary>
    ///     Generates object containing a LineRenderer forming a circle
    /// </summary>
    public static LineRenderer DrawCircle(float x, float y, float radius, Transform parent = null) =>
        DrawCircle(new(x, y), radius, parent);

    /// <summary>
    ///     Generates object containing a LineRenderer
    /// </summary>
    public static LineRenderer DrawLine(float x1, float y1, float x2, float y2, Transform parent = null) =>
        DrawLine(new(x1, y1), new(x2, y2), parent);


    /// <summary>
    ///     Generates object containing a LineRenderer
    /// </summary>
    public static LineRenderer DrawLine(Vector2 point1, Vector2 point2, Transform parent = null)
    {
        if (parent == null) parent = ReferenceManager.Instance.DrawContainer;

        // generate object
        LineRenderer line = NewDrawObject("DrawLine", parent);
        line.sortingOrder = OrderInLayer;
        line.sortingLayerID = LayerID;
        line.positionCount = 2;
        // line.numCapVertices = 0;

        line.SetPosition(0, point1);
        line.SetPosition(1, point2);

        return line;
    }

    public static LineRenderer DrawDashedLine(Vector2 start, Vector2 end, float width, float spacing, Transform parent = null)
    {
        if (parent == null) parent = ReferenceManager.Instance.DrawContainer;

        // generate object
        LineRenderer line = DrawLine(start, end, parent);

        DashedLineController dashedLineController = line.gameObject.AddComponent<DashedLineController>();

        dashedLineController.Width = width;
        dashedLineController.Spacing = spacing;

        return line;
    }

    public static (Vector2 arrowVertex1, Vector2 arrowVertex2, Vector2 arrowCenter) GetArrowHeadPoints(Vector2 start, Vector2 end)
    {
        const float headLineLength = 0.15f;
        Vector2 delta = end - start;
        Vector2 halfPoint = start + delta / 2;
        Vector2 offset = delta.normalized * (headLineLength / 2);
        Vector2 _start = halfPoint + offset;
        Vector2 endSideOffset = delta.normalized * Mathf.Sin(headLineLength);
        endSideOffset.Rotate(90);
        Vector2 _end = halfPoint - offset;

        Vector2 arrowVertex1 = _end + endSideOffset;
        Vector2 arrowVertex2 = _end - endSideOffset;
        Vector2 arrowCenter = _start;

        return (arrowVertex1, arrowVertex2, arrowCenter);
    }

    private static LineRenderer NewDrawObject(string name, Transform parent)
    {
        // DrawContainer is default container
        if (parent == null) parent = ReferenceManager.Instance.DrawContainer;

        GameObject stroke = new()
        {
            name = name,
            transform = { parent = parent }
        };

        LineRenderer line = stroke.AddComponent<LineRenderer>();
        line.material = MaterialManager.Instance.LineMaterial;

        line.startWidth = Weight;
        line.endWidth = Weight;

        line.startColor = Fill;
        line.endColor = Fill;

        line.numCapVertices = RoundedCorners ? 5 : 0;
        return line;
    }

    public static List<Vector2> GetCirclePoints(Vector2 origin, float radius, int accuracy)
    {
        List<Vector2> points = new();
        for (int currentStep = 0; currentStep <= accuracy; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / accuracy; // position relative to starting point
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;
            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float xPos = xScaled * radius + origin.x;
            float yPos = yScaled * radius + origin.y;

            Vector3 currentPosition = new(xPos, yPos, 0);
            points.Add(currentPosition);
        }

        return points;
    }

    public static void SetFill(float r, float g, float b) => Fill = new(r, g, b);

    public static void SetFill(Color color) => Fill = color;

    public static void SetWeight(float setWeight) => Weight = setWeight;

    public static void SetRoundedCorners(bool set) => RoundedCorners = set;

    public static void SetLayerID(int id) => LayerID = id;

    public static void SetOrderInLayer(int order) => OrderInLayer = order;
}