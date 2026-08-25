using UnityEngine;

public class PlayerPlacementRules
{
    private readonly ILevelObjectQuery<PlayerController> playerQueryService;
    private readonly IFieldQueryService fieldQueryService;

    public PlayerPlacementRules(ILevelObjectQuery<PlayerController> playerQueryService, IFieldQueryService fieldQueryService)
    {
        this.playerQueryService = playerQueryService;
        this.fieldQueryService = fieldQueryService;
    }
    
    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet)
    {
        return !playerQueryService.Exists(position, sheet) &&
               fieldQueryService.IsPosCoveredWithFieldTypeInSheet(position, sheet,
                   EditModeManager.Instance.AllPlayerStartFieldModes.ToArray());
    }
}