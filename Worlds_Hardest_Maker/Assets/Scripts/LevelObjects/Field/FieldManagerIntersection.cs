using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public partial class FieldManager
{
    public bool IntersectingAnyFieldsAtPos(Vector2 position, [CanBeNull] AnchorController sheet, params FieldMode[] t)
    {
        List<FieldMode> modes = t.ToList();
        
        List<FieldController> intersectingFields = GetFieldsAtGridPosInSheet(position, sheet);
        foreach (FieldController field in intersectingFields)
        {
            if (modes.Contains(field.FieldMode)) return true;
        }
        
        return false;
    }
    
    public bool IntersectingEveryFieldAtPos(Vector2 position, [CanBeNull] AnchorController sheet, params FieldMode[] t)
    {
        List<FieldMode> types = t.ToList();
        List<FieldController> intersectingFields = GetFieldsAtGridPosInSheet(position, sheet);
        foreach (FieldController field in intersectingFields)
        {
            if (!types.Contains(field.FieldMode)) return false;
        }
        
        return true;
    }
    
    public bool IsPosCoveredWithFieldTypeInSheet(Vector2 position, [CanBeNull] AnchorController sheet, params FieldMode[] t)
    {
        List<FieldMode> types = t.ToList();
        List<FieldController> intersectingFields = GetFieldsAtGridPosInSheet(position, sheet);
        if (intersectingFields.Count == 0) return false;
        
        int expectedCount = position.GetIntersectionCount();
        
        foreach (FieldController field in intersectingFields)
        {
            if (expectedCount != intersectingFields.Count || !types.Contains(field.FieldMode)) return false;
        }
        
        return true;
    }
}