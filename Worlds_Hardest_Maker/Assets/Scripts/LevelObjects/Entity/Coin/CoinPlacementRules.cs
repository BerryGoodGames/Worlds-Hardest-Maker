using UnityEngine;

public class CoinPlacementRules
{
    private readonly ICoinQueryService coinQueryService;
    private readonly IPlayerQueryService playerQueryService;

    public CoinPlacementRules(ICoinQueryService coinQueryService, IPlayerQueryService playerQueryService)
    {
        this.coinQueryService = coinQueryService;
        this.playerQueryService = playerQueryService;
    }
    
    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet)
    {
        return !coinQueryService.IsThereInSheet(position, sheet) && !playerQueryService.IsThereInSheet(position, sheet);
    }
}