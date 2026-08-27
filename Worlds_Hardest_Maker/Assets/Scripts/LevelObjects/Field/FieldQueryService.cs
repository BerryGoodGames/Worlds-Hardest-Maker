using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldQueryService : ILevelObjectQuery<FieldController>
{
    private readonly IPositionQueryService positionQueryService;

    public FieldQueryService(IPositionQueryService positionQueryService)
    {
        this.positionQueryService = positionQueryService;
    }
    
    private List<FieldController> FindFieldsAtGridPosInSheet(Vector2 position, ISheet sheet)
    {
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
        };
        
        checkPoses = checkPoses.Distinct().ToArray();
        
        List<FieldController> res = new();
        foreach (Vector2Int checkPosition in checkPoses)
        {
            FieldController field = Find(checkPosition, sheet);
            if (field != null) res.Add(field);
        }
        
        return res;
    }
    
    public bool IntersectingAnyFieldsAtPos(Vector2 position, ISheet sheet, params FieldMode[] t)
    {
        List<FieldMode> modes = t.ToList();
        
        List<FieldController> intersectingFields = FindFieldsAtGridPosInSheet(position, sheet);
        foreach (FieldController field in intersectingFields)
        {
            if (modes.Contains(field.FieldMode)) return true;
        }
        
        return false;
    }
    
    public bool IntersectingEveryFieldAtPos(Vector2 position, ISheet sheet, params FieldMode[] t)
    {
        List<FieldMode> types = t.ToList();
        List<FieldController> intersectingFields = FindFieldsAtGridPosInSheet(position, sheet);
        foreach (FieldController field in intersectingFields)
        {
            if (!types.Contains(field.FieldMode)) return false;
        }
        
        return true;
    }
    
    public bool IsPosCoveredWithFieldTypeInSheet(Vector2 position, ISheet sheet, IEnumerable<FieldMode> t)
    {
        List<FieldMode> types = t.ToList();
        List<FieldController> intersectingFields = FindFieldsAtGridPosInSheet(position, sheet);
        if (intersectingFields.Count == 0) return false;
        
        int expectedCount = position.GetIntersectionCount();
        
        foreach (FieldController field in intersectingFields)
        {
            if (expectedCount != intersectingFields.Count || !types.Contains(field.FieldMode)) return false;
        }
        
        return true;
    }

    public FieldController Find(Vector2 position, ISheet sheet)
    {
        FieldController inFieldLayer = positionQueryService.QueryPosition<FieldController>(position, 
            0.1f, 
            LayerManager.Instance.Layers.Field, 
            sheet);
        
        if (inFieldLayer != null) return inFieldLayer;

        return positionQueryService.QueryPosition<FieldController>(position, 0.1f, LayerManager.Instance.Layers.Void,
            sheet);
    }

    public FieldController FindAny(Vector2 position)
    {
        FieldController inFieldLayer = positionQueryService.QueryPositionAny<FieldController>(position,
            0.1f,
            LayerManager.Instance.Layers.Field);

        if (inFieldLayer != null) return inFieldLayer;

        return positionQueryService.QueryPositionAny<FieldController>(position, 0.1f,
            LayerManager.Instance.Layers.Void);
    }

    public bool Exists(Vector2 position, ISheet sheet)
    {
        return Find(position, sheet) != null;
    }
}