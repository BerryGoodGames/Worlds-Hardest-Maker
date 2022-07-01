using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// rendering lines / circles: generating objects in container holding LineRenderers
/// attach to game manager
/// </summary>
public class LineManager : MonoBehaviour
{
    // Settings for drawing
    public static float weight = 0.11f;
    public static Color fill = new(0, 0, 0);
    public static bool roundedCorners = true;

    /// <summary>
    /// generate object containing a LineRenderer forming a rectangle
    /// </summary>
    /// <param name="x">x-coordinate of top right corner / center of rectangle</param>
    /// <param name="y">y-coordinate of top right corner / center of rectangle</param>
    /// <param name="w">width of rectangle</param>
    /// <param name="h">height of rectangle</param>
    /// <param name="sortingOrder">order in layer</param>
    /// <param name="alignCenter"></param>
    /// <param name="parent">parent the generated gameobject will be placed in, if nothing passed then DrawContainer</param>
    public static void DrawRect(float x, float y, float w, float h, int sortingOrder, bool alignCenter = false, Transform parent = null)
    {
        // generate object
        GameObject stroke = NewDrawObject("DrawRect", parent);

        LineRenderer rect = stroke.GetComponent<LineRenderer>();
        rect.positionCount = 5;
        rect.sortingOrder = sortingOrder;

        // get positions
        Vector2[] positions = {
            new(x, y),
            new(x + w, y),
            new(x + w, y + h),
            new(x, y + h),
            new(x, y)
        };

        // set positions
        for (int i = 0; i < positions.Length; i++)
        {
            if (alignCenter)
            {
                positions[i].x -= w / 2;
                positions[i].y -= h / 2;
            }
            rect.SetPosition(i, positions[i]);
        }
    }

    /// <summary>
    /// generate object containing a LineRenderer forming a circle
    /// </summary>
    /// <param name="x">x-coordinate of center</param>
    /// <param name="y">y-coordinate of center</param>
    /// <param name="radius">radius of circle</param>
    /// <param name="sortingOrder">order in layer</param>
    /// <param name="parent">parent the generated gameobject will be placed in, if nothing passed then DrawContainer</param>
    public static void DrawCircle(float x, float y, float radius, int sortingOrder, Transform parent = null)
    {
        // generate object
        GameObject stroke = NewDrawObject("DrawCircle", parent);

        LineRenderer circle = stroke.GetComponent<LineRenderer>();
        circle.sortingOrder = sortingOrder;

        // get points of circle
        int steps = 100;
        List<Vector2> points = GetCirclePoints(new(x, y), radius, steps);

        // set points
        circle.positionCount = steps + 1;
        for (int i = 0; i < points.Count; i++)
        {
            circle.SetPosition(i, points[i]);
        }
    }

    /// <summary>
    /// generate object containing a LineRenderer
    /// </summary>
    /// <param name="x1">x-coordinate of first point</param>
    /// <param name="y1">y-coordinate of second point</param>
    /// <param name="x2">x-coordinate of first point</param>
    /// <param name="y2">y-coordinate of second point</param>
    /// <param name="sortingOrder">order in layer</param>
    /// <param name="parent">parent the generated gameobject will be placed in, if nothing passed then DrawContainer</param>
    public static void DrawLine(float x1, float y1, float x2, float y2, int sortingOrder, Transform parent = null)
    {
        // generate object
        GameObject stroke = NewDrawObject("DrawLine", parent);

        LineRenderer line = stroke.GetComponent<LineRenderer>();
        line.sortingOrder = sortingOrder;
        line.positionCount = 2;

        // set points
        Vector3 start = new(x1, y1, 0);
        Vector3 end = new(x2, y2, 0);

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    /// <summary>
    /// generate object containing a LineRenderer
    /// </summary>
    /// <param name="pos1">first point</param>
    /// <param name="pos2">second point</param>
    /// <param name="sortingOrder">order in layer</param>
    /// <param name="parent">parent the generated gameobject will be placed in, if nothing passed then DrawContainer</param>
    public static void DrawLine(Vector2 pos1, Vector2 pos2, int sortingOrder, Transform parent = null)
    {
        if (parent == null)
        {
            parent = GameManager.Instance.DrawContainer.transform;
        }

        GameObject stroke = NewDrawObject("DrawLine", parent);

        LineRenderer line = stroke.GetComponent<LineRenderer>();
        line.sortingOrder = sortingOrder;
        line.positionCount = 2;

        Vector3 start = new(pos1.x, pos1.y, 0);
        Vector3 end = new(pos2.x, pos2.y, 0);

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    private static GameObject NewDrawObject(string name, Transform parent)
    {
        // DrawContainer is default container
        if (parent == null) parent = GameManager.Instance.DrawContainer.transform;

        GameObject stroke = new(){ name = name };
        stroke.transform.parent = parent;

        LineRenderer line = stroke.AddComponent<LineRenderer>();
        line.material = GameManager.Instance.LineMaterial;

        line.startWidth = weight;
        line.endWidth = weight;

        line.startColor = fill;
        line.endColor = fill;

        line.numCapVertices = roundedCorners ? 5 : 0;
        return stroke;
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

    public static void SetFill(float r, float g, float b)
    {
        fill = new(r, g, b);
    }
    public static void SetFill(Color color)
    {
        fill = color;
    }
    public static void SetWeight(float setWeight)
    {
        weight = setWeight;
    }
    public static void SetRoundedCorners(bool set)
    {
        roundedCorners = set;
    }
}
