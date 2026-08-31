using System.Collections.Generic;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

public class AreaErasureService : IAreaErasureService
{
    [Inject] private IAreaQueryService areaQueryService;
    [Inject] private FieldQueryService fieldQueryService;
    [Inject] private IFieldManager fieldManager;
    [Inject] private IPlayerManager playerManager;
    
    public void EraseArea(SelectionArea area)
    {
        IReadOnlyList<Vector2> positions = area.Positions;
        
        // get everything in area
        if (positions.Count == 0) return;
        
        Collider2D[] hits = areaQueryService.QueryArea(area, LayerManager.Instance.Layers.LevelObjectMask, PlaceManager.GetCurrentSheet());
        
        // DESTROY IT MUHAHAHAHAHAHHAHAHAHAHAHAHAHAHA
        foreach (Collider2D collider in hits)
        {
            if(LevelObjectController.TryGetController(collider, out LevelObjectController controller))
            {
                controller.Delete();
            }
            else
            {
                Debug.LogWarning("Found hit with no leve object component, ignoring hit");
            }
        }
        
        PlayerController player = playerManager.Player;
        ISheet currentSheet = PlaceManager.GetCurrentSheet();
        IEnumerable<FieldMode> startFieldModes = EditModeManager.Instance.AllPlayerStartFieldModes;
        
        if (player != null
            && !fieldQueryService.IsPosCoveredWithFieldTypeInSheet(player.transform.position, currentSheet, startFieldModes))
        {
            playerManager.RemoveAtPos(player.transform.position);
        }
        
        fieldManager.UpdateOutlinesInArea(false, area);
    }
}