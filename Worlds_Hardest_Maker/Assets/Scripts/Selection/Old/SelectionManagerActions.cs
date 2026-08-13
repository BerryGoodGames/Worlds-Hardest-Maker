using System.Collections.Generic;
using System.Linq;
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
        Vector2 lowestPos = positions[0];
        Vector2 highestPos = positions.Last();
        Vector2 castPos = (lowestPos + highestPos) * 0.5f;
        Vector2 castSize = highestPos - lowestPos;
        
        // TODO: extract physics query logic to a service
        Collider2D[] hits = Physics2D.OverlapBoxAll(castPos, castSize, 0, LayerManager.Instance.Layers.LevelObjectMask);
        
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
        
        FieldManager.UpdateOutlinesInArea(false, lowestPos, highestPos);
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