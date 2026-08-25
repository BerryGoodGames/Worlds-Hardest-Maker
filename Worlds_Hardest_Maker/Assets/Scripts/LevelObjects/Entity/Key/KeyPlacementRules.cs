using JetBrains.Annotations;
using UnityEngine;

public class KeyPlacementRules
{
    private readonly IPlayerQueryService playerQueryService;
    private readonly IKeyQueryService keyQueryService;
    
    public bool CanPlaceInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        return !playerQueryService.IsThereInSheet(position, sheet) && !keyQueryService.IsThereInSheet(position, sheet);
    }
}