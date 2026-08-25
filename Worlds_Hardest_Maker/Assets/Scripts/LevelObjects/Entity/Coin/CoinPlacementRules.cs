using UnityEngine;

public class CoinPlacementRules
{
    private readonly ILevelObjectQuery<CoinController> coinQueryService;
    private readonly ILevelObjectQuery<PlayerController> playerQueryService;

    public CoinPlacementRules(ILevelObjectQuery<CoinController> coinQueryService, ILevelObjectQuery<PlayerController> playerQueryService)
    {
        this.coinQueryService = coinQueryService;
        this.playerQueryService = playerQueryService;
    }
    
    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet)
    {
        return !coinQueryService.Exists(position, sheet) && !playerQueryService.Exists(position, sheet);
    }
}