using UnityEngine;

public interface IAreaQueryService
{
    public Collider2D[] QueryArea(SelectionArea area, LayerMask layer);
}