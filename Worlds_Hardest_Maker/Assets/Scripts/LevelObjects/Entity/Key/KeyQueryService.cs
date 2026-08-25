using UnityEngine;

public class KeyQueryService : ILevelObjectQuery<KeyController>
{
    private readonly IPositionQueryService positionQueryService;

    public KeyQueryService(IPositionQueryService positionQueryService)
    {
        this.positionQueryService = positionQueryService;
    }
    
    public KeyController Find(Vector2 position, AnchorController sheet)
    {
        return positionQueryService.QueryPosition<KeyController>(position, 0.01f, LayerManager.Instance.Layers.Entity,
            "Key", sheet);
    }

    public bool Exists(Vector2 position, AnchorController sheet)
    {
        return Find(position, sheet) != null;
    }
}