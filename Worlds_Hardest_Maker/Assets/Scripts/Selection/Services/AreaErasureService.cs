using System.Collections.Generic;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

public class AreaErasureService : IAreaErasureService
{
    [Inject] private IAreaQueryService areaQueryService;
    [Inject] private FieldQueryService fieldQueryService;
    
    public void EraseArea(SelectionArea area)
    {
        IReadOnlyList<Vector2> positions = area.Positions;
        
        // get everything in area
        if (positions.Count == 0) return;
        
        Collider2D[] hits = areaQueryService.QueryArea(area, LayerManager.Instance.Layers.LevelObjectMask, PlaceManager.GetCurrentSheet());
        
        // DESTROY IT MUHAHAHAHAHAHHAHAHAHAHAHAHAHAHA
        foreach (Collider2D collider in hits)
        {
            // if (collider.CompareTag("AnchorObject"))
            // {
            //     collider.GetComponent<AnchorController>().Delete();
            //     continue;
            // }
            //
            // Object.Destroy(collider.gameObject);

            // TODO: test this
            if(LevelObjectController.TryGetController(collider, out LevelObjectController controller))
            {
                controller.Delete();
            }
            else
            {
                Debug.LogWarning("Found hit with no leve object component, ignoring hit");
            }
        }
        
        PlayerController player = PlayerManager.Instance.Player;
        ISheet currentSheet = PlaceManager.GetCurrentSheet();
        IEnumerable<FieldMode> startFieldModes = EditModeManager.Instance.AllPlayerStartFieldModes;
        
        if (player != null
            && !fieldQueryService.IsPosCoveredWithFieldTypeInSheet(player.transform.position, currentSheet, startFieldModes))
        {
            PlayerManager.Instance.RemoveAtPos(player.transform.position);
        }
        
        FieldManager.Instance.UpdateOutlinesInArea(false, area);
    }
}