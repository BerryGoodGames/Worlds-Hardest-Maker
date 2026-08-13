using UnityEngine;

public class AreaQuery : IAreaQueryService
{
    public Collider2D[] QueryArea(SelectionArea area, LayerMask layer)
    {
        return Physics2D.OverlapAreaAll(area.Lowest, area.Highest, layer);
    }
}