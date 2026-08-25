using UnityEngine;

public class PositionQueryService : IPositionQueryService
{
    public T QueryPosition<T>(Vector2 position, float radius, LayerMask layer, string tag, AnchorController sheet) where T : Component
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, layer);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(tag)) continue;
            if (!hit.TryGetComponent(out T obj)) continue;
            if (IManager.IsInSheet(obj, sheet) || IManager.IsInSheet(obj.transform.parent, sheet)) return obj;
        }
    
        return null;
    }

    public T QueryPosition<T>(Vector2 position, float radius, LayerMask layer, AnchorController sheet) where T : Component
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, layer);
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out T obj)) continue;
            if (IManager.IsInSheet(obj, sheet) || IManager.IsInSheet(obj.transform.parent, sheet)) return obj;
        }
    
        return null;
    }
}