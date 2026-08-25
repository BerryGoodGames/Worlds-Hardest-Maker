using UnityEngine;

public class CoinQueryService : ILevelObjectQuery<CoinController>
{
    private readonly IPositionQueryService positionQueryService;

    public CoinQueryService(IPositionQueryService positionQueryService)
    {
        this.positionQueryService = positionQueryService;
    }

    public CoinController Find(Vector2 position, AnchorController sheet)
    {
        return positionQueryService.QueryPosition<CoinController>(position,
            0.1f,
            LayerManager.Instance.Layers.Entity,
            "Coin",
            sheet);
    }

    public bool Exists(Vector2 position, AnchorController sheet)
    {
        return Find(position, sheet) != null;
    }
}