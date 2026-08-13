using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionArea
{
    public static readonly SelectionArea Empty = new(new List<Vector2>());
    
    public IReadOnlyList<Vector2> Positions { get; }
    
    public SelectionArea(IReadOnlyList<Vector2> positions)
    {
        Positions = positions;
    }

    public Vector2 First()
    {
        return Positions.Count == 0 ? throw new ArgumentException("Could not get first position in selection area, length was 0") : Positions[0];
    }

    public Vector2 Last()
    {
        return Positions.Count == 0 ? throw new ArgumentException("Could not get last position in selection area, length was 0") : Positions[^1];
    }
}