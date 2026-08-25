using JetBrains.Annotations;
using UnityEngine;

public class KeyPlacementRules
{
    private readonly ILevelObjectQuery<KeyController> keyQueryService;
    private readonly ILevelObjectQuery<PlayerController> playerQueryService;
    
    public KeyPlacementRules(ILevelObjectQuery<KeyController> keyQueryService, ILevelObjectQuery<PlayerController> playerQueryService)
    {
        this.keyQueryService = keyQueryService;
        this.playerQueryService = playerQueryService;
    }
    
    public bool CanPlaceInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        return !playerQueryService.Exists(position, sheet) && !keyQueryService.Exists(position, sheet);
    }
}