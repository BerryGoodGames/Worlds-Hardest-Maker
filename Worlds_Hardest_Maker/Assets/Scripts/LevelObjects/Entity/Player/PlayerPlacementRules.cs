using UnityEngine;

public class PlayerPlacementRules
{
    private readonly IPlayerQueryService playerQueryService;
    private readonly IFieldQueryService fieldQueryService;

    public PlayerPlacementRules(IPlayerQueryService playerQueryService, IFieldQueryService fieldQueryService)
    {
        this.playerQueryService = playerQueryService;
        this.fieldQueryService = fieldQueryService;
    }
    
    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet)
    {
        return !playerQueryService.IsThereInSheet(position, sheet) &&
               fieldQueryService.IsPosCoveredWithFieldTypeInSheet(position, sheet,
                   EditModeManager.Instance.AllPlayerStartFieldModes.ToArray());
    }
}