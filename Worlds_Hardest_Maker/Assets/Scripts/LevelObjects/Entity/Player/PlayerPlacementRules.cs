using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacementRules
{
    private readonly ILevelObjectQuery<PlayerController> playerQueryService;
    private readonly FieldQueryService fieldQueryService;

    public PlayerPlacementRules(ILevelObjectQuery<PlayerController> playerQueryService, FieldQueryService fieldQueryService)
    {
        this.playerQueryService = playerQueryService;
        this.fieldQueryService = fieldQueryService;
    }
    
    public bool CanPlaceInSheet(Vector2 position, ISheet sheet)
    {
        return !playerQueryService.Exists(position, sheet) &&
               fieldQueryService.IsPosCoveredWithFieldTypeInSheet(position, sheet, EditModeManager.Instance.AllPlayerStartFieldModes);
    }
    
    public IEnumerable<Vector2Int> GetAutoPlacedStartFieldPositions(Vector2 position, ISheet sheet)
    {
        if (playerQueryService.Exists(position, sheet)) return Array.Empty<Vector2Int>();
        if (CanPlaceInSheet(position, sheet)) return Array.Empty<Vector2Int>();

        return position.GetIntersectionPositions();
    }
}