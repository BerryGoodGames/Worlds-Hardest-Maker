using UnityEngine;

public class BallQueryService : ILevelObjectQuery<BallController>
{
    private readonly IPositionQueryService positionQueryService;

    public BallQueryService(IPositionQueryService positionQueryService)
    {
        this.positionQueryService = positionQueryService;
    }
    
    public BallController Find(Vector2 position, AnchorController sheet)
    {
        return positionQueryService.QueryPosition<BallController>(position,
            0.01f,
            LayerManager.Instance.Layers.Entity,
            "BallObject",
            sheet);
    }

    public bool Exists(Vector2 position, AnchorController sheet)
    {
        return Find(position, sheet) != null;
    }
}