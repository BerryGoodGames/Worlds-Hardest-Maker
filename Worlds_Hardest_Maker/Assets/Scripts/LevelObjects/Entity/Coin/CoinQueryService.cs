using UnityEngine;

public class CoinQueryService : ILevelObjectQuery<CoinController>
{
    private readonly IPositionQueryService positionQueryService;

    public CoinQueryService(IPositionQueryService positionQueryService)
    {
        this.positionQueryService = positionQueryService;
    }

    public CoinController Find(Vector2 position, ISheet sheet)
    {
        return positionQueryService.QueryPosition<CoinController>(position,
            0.1f,
            LayerManager.Instance.Layers.Entity,
            "Coin",
            sheet);
    }

    public CoinController FindAny(Vector2 position)
    {
        return positionQueryService.QueryPositionAny<CoinController>(position, 0.1f,
            LayerManager.Instance.Layers.Entity);
    }

    public bool Exists(Vector2 position, ISheet sheet)
    {
        return Find(position, sheet) != null;
    }
}