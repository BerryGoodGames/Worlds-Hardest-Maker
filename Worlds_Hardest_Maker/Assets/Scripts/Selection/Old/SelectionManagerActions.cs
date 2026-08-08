using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SelectionManager
{
    public void OnDeleteClicked()
    {
        DeleteArea(CurrentFillRange);
        OnCancelClicked();
    }
    
    public static void DeleteArea(List<Vector2> poses)
    {
        // get everything in area
        if (poses.Count == 0) return;
        Vector2 lowestPos = poses[0];
        Vector2 highestPos = poses.Last();
        Vector2 castPos = (lowestPos + highestPos) * 0.5f;
        Vector2 castSize = highestPos - lowestPos;
        
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
            )) PlayerManager.Instance.RemoveAtPos(player.transform.position);
        
        FieldManager.UpdateOutlinesInArea(false, lowestPos, highestPos);
    }
    
    public void OnCopyClicked()
    {
        Vector2 lowestPos = CurrentFillRange[0];
        Vector2 highestPos = CurrentFillRange[^1];
        
        CopyManager.Instance.Copy(lowestPos, highestPos);
        
        OnCancelClicked();
    }
    
    public void OnCutClicked()
    {
        OnCopyClicked();
        OnDeleteClicked();
    }
    
    public void OnCancelClicked()
    {
        previewController.DestroyPreview();

        // reset selection marking
        if (selectionOutline != null) Destroy(selectionOutline);
        
        // hide selection menu
        selectionOptions.gameObject.SetActive(false);
        
        Selecting = false;
        
        MenuManager.Instance.BlockMenu = false;
    }
}