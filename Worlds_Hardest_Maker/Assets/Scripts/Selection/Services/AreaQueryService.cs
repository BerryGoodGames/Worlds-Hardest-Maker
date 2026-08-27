using System.Linq;
using UnityEngine;
using WorldsHardestMaker.Selection;

public class AreaQueryService : IAreaQueryService
{
    public Collider2D[] QueryArea(SelectionArea area, LayerMask layer, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapAreaAll(area.Lowest, area.Highest, layer);

        return hits.Where(hit => SheetUtils.Exists(hit, sheet)).ToArray();
    }
}