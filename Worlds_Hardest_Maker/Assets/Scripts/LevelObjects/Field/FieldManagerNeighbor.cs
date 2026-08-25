using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public partial class FieldManager
{
    public List<FieldController> GetNeighbors(GameObject field)
    {
        Vector2Int position = Vector2Int.RoundToInt(field.transform.position);
        return GetNeighbors(position);
    }
    
    public List<FieldController> GetNeighbors(Vector2 position)
    {
        Vector2Int[] deltas = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, };
        
        List<FieldController> neighbors = new();
        
        foreach (Vector2Int d in deltas)
        {
            FieldController neighbor = Get(position + d);
            if (neighbor != null) neighbors.Add(neighbor);
        }
        
        return neighbors;
    }
    
    public List<FieldController> GetNeighborsInSheet(GameObject field, [CanBeNull] AnchorController sheet)
    {
        Vector2Int position = Vector2Int.RoundToInt(field.transform.position);
        return GetNeighborsInSheet(position, sheet);
    }
    
    public List<FieldController> GetNeighborsInSheet(Vector2Int position, [CanBeNull] AnchorController sheet)
    {
        Vector2Int[] deltas = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, };
        
        List<FieldController> neighbors = new();
        
        foreach (Vector2Int d in deltas)
        {
            FieldController neighbor = fieldQueryService.Find(position + d, sheet);
            if (neighbor != null) neighbors.Add(neighbor);
        }
        
        return neighbors;
    }
}