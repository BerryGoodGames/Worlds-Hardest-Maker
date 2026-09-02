using System.Collections.Generic;
using UnityEngine;

public class PlayerStartFieldResolver
{
    private readonly PlayerPlacementRules playerPlacementRules;
    private readonly IFieldManager fieldManager;
    private readonly IPlayerManager playerManager;
    
    public PlayerStartFieldResolver(PlayerPlacementRules playerPlacementRules, IFieldManager fieldManager, IPlayerManager playerManager)
    {
        this.playerPlacementRules = playerPlacementRules;
        this.fieldManager = fieldManager;
        this.playerManager = playerManager;
    }
    
    public void OnPlayerPlaced(Vector2 position, ISheet sheet)
    {
        IEnumerable<Vector2Int> positions = playerPlacementRules.GetAutoPlacedStartFieldPositions(position, sheet);
        foreach (Vector2Int pos in positions)
        {
            fieldManager.CreateNew(pos, 0, sheet, EditModeManager.Start);
        }
    }

    public void OnFieldPlaced(Vector2 position, ISheet sheet, FieldMode fieldMode)
    {
        if (!fieldMode.IsStartFieldForPlayer)
        {
            playerManager.RemoveAtPosIntersectInSheet(position, sheet);
        }
    }
}