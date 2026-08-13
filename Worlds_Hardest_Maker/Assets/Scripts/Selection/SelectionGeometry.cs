using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SelectionGeometry
{
    // get bounds of multiple points (in matrix)
    private static (Vector2 lowest, Vector2 highest) GetBounds(IReadOnlyList<Vector2> points)
    {
        if (points.Count == 0)
        {
            throw new ArgumentException("Could not give bounds because point list is empty.");
        }
        
        Vector2 lowest = points[0];
        Vector2 highest = points[0];
        
        foreach (Vector2 pos in points)
        {
            if (lowest.x > pos.x) lowest.x = pos.x;
            if (lowest.y > pos.y) lowest.y = pos.y;
            if (highest.x < pos.x) highest.x = pos.x;
            if (highest.y < pos.y) highest.y = pos.y;
        }
        
        return (lowest, highest);
    }

    public static (Vector2 lowest, Vector2 highest) GetBounds(params Vector2[] points) => GetBounds(points.ToList());

    public static (Vector2 lowest, Vector2 highest) GetBoundsGrid(IReadOnlyList<Vector2> points)
    {
        return GetBounds(points);
    }

    public static (Vector2 lowest, Vector2 highest) GetBoundsGrid(params Vector2[] points) => GetBoundsGrid(points.ToList());
    
    public static (Vector2Int lowest, Vector2Int highest) GetBoundsMatrix(IReadOnlyList<Vector2> points)
    {
        (Vector2 lowest, Vector2 highest) = GetBounds(points);
        return (Vector2Int.CeilToInt(lowest), Vector2Int.FloorToInt(highest));
    }

    public static (Vector2Int lowest, Vector2Int highest) GetBoundsMatrix(params Vector2[] points) => GetBoundsMatrix(points.ToList());

    public static SelectionArea GetFillArea(Vector2 p1, Vector2 p2)
    {
        WorldPositionType positionType = LevelSessionEditManager.Instance.CurrentEditMode.GetWorldPositionType();
        return GetFillArea(p1, p2, positionType);
    }

    public static SelectionArea GetFillArea(Vector2 p1, Vector2 p2, WorldPositionType worldPositionType)
    {
        const float matrixIncrement = 1;
        const float gridIncrement = 0.5f;
        
        bool inMatrix = worldPositionType is WorldPositionType.Matrix;
        
        // find bounds
        (Vector2 lowest, Vector2 highest) = inMatrix ? GetBoundsMatrix(p1, p2) : GetBoundsGrid(p1, p2);
        
        // collect every pos in range
        float increment = inMatrix ? matrixIncrement : gridIncrement;
        List<Vector2> res = new();
        for (float x = lowest.x; x <= highest.x; x += increment)
        {
            for (float y = lowest.y; y <= highest.y; y += increment) res.Add(new(x, y));
        }
        
        return new(res);
    }
}