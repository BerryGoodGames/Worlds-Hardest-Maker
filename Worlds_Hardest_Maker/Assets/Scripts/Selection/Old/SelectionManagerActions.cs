using System.Collections.Generic;
using UnityEngine;

public partial class SelectionManager
{
    public void OnDeleteClicked()
    {
        DeleteArea(selectionAreaProvider.GetArea());
        selectionStateService.ClearSelection();
    }
    
    // TODO: extract delete logic to service IAreaEraser
    private void DeleteArea(SelectionArea area)
    {
        IReadOnlyList<Vector2> positions = area.Positions;
        
        // get everything in area
        if (positions.Count == 0) return;
        
        Collider2D[] hits = areaQueryService.QueryArea(area, LayerManager.Instance.Layers.LevelObjectMask);
        
        // DESTROY IT MUHAHAHAHAHAHHAHAHAHAHAHAHAHAHA
        foreach (Collider2D collider in hits)
        {
            if (collider.CompareTag("AnchorObject"))
            {
                collider.GetComponent<AnchorController>().Delete();
                continue;
            }
            
            Destroy(collider.gameObject);
        }
        
        PlayerController player = PlayerManager.Instance.Player;
        
        if (player != null
            && !FieldManager.Instance.IsPosCoveredWithFieldTypeInSheet(
                player.transform.position, PlaceManager.GetCurrentSheet(), EditModeManager.Instance.AllPlayerStartFieldModes.ToArray()
            ))
        {
            PlayerManager.Instance.RemoveAtPos(player.transform.position);
        }
        
        FieldManager.UpdateOutlinesInArea(false, area);
    }
    
    public void OnCopyClicked()
    {
        SelectionArea selectedArea = selectionAreaProvider.GetArea();
        Vector2 lowestPos = selectedArea.First();
        Vector2 highestPos = selectedArea.Last();
        
        CopyManager.Instance.Copy(lowestPos, highestPos);
        
        selectionStateService.ClearSelection();
    }
    
    public void OnCutClicked()
    {
        OnCopyClicked();
        OnDeleteClicked();
    }
    
    public void OnCancelClicked()
    {
        selectionStateService.CancelSelection();
    }
}