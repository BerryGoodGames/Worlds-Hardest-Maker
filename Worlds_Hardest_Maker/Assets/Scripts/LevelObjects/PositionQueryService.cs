using System;
using UnityEngine;

public class PositionQueryService : IPositionQueryService
{
    public T QueryPosition<T>(Vector2 position, 
        float radius, 
        LayerMask layer, 
        string tag, 
        ISheet sheet,
        SheetUtils.SheetCheckingScope scope = SheetUtils.SheetCheckingScope.Self) where T : Component
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, layer);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(tag)) continue;
            if (!hit.TryGetComponent(out T obj)) continue;
            if (MatchesSheet(obj, sheet, scope)) return obj;
        }
    
        return null;
    }

    public T QueryPosition<T>(Vector2 position, 
        float radius, 
        LayerMask layer, 
        ISheet sheet,
        SheetUtils.SheetCheckingScope scope = SheetUtils.SheetCheckingScope.Self) where T : Component
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, layer);
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out T obj)) continue;
            if (MatchesSheet(obj, sheet, scope)) return obj;
        }
    
        return null;
    }

    public T QueryPositionAny<T>(Vector2 position, float radius, LayerMask layer) where T : Component
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, layer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out T controller)) return controller;
        }
        
        return null;
    }
    
    private static bool MatchesSheet<T>(T obj, ISheet sheet, SheetUtils.SheetCheckingScope scope) where T : Component
    {
        return scope switch
        {
            SheetUtils.SheetCheckingScope.Self => SheetUtils.Exists(obj, sheet),
            SheetUtils.SheetCheckingScope.Parent => SheetUtils.Exists(obj.transform.parent, sheet),
            SheetUtils.SheetCheckingScope.SelfOrParent => SheetUtils.Exists(obj, sheet) || SheetUtils.Exists(obj.transform.parent, sheet),
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };
    }
}