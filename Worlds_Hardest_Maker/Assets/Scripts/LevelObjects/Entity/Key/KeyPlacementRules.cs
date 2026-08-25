using JetBrains.Annotations;
using UnityEngine;

public class KeyPlacementRules
{
    private readonly IKeyQueryService keyQueryService;
    private readonly IPlayerQueryService playerQueryService;
    
    public KeyPlacementRules(IKeyQueryService keyQueryService, IPlayerQueryService playerQueryService)
    {
        this.keyQueryService = keyQueryService;
        this.playerQueryService = playerQueryService;
    }
    
    public bool CanPlaceInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        return !playerQueryService.IsThereInSheet(position, sheet) && !keyQueryService.IsThereInSheet(position, sheet);
    }
}