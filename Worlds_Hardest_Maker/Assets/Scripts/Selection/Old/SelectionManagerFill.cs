using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using VContainer.Unity;

public partial class SelectionManager
{
    public List<Vector2> GetFillRange()
    {
        return CurrentFillRange;
    }

    public List<Vector2> CurrentFillRange => selectionStateService.Start == null || selectionStateService.End == null ? null : GetCurrentFillRange();
    
    public List<Vector2> GetCurrentFillRange()
    {
        if (selectionStateService.Start == null || selectionStateService.End == null) return null;
        return SelectionGeometry.GetFillRange((Vector2)selectionStateService.Start, (Vector2)selectionStateService.End);
    }
    
    public void FillSelectedArea()
    {
        FillArea(CurrentFillRange, LevelSessionEditManager.Instance.CurrentEditMode);
        
        ClearSelection();
    }
    
    public void FillAreaWithFields(List<Vector2> positions, FieldMode mode)
    {
        // set rotation
        int rotation = mode.IsRotatable
            ? LevelSessionEditManager.Instance.EditRotation
            : 0;
        
        // find bounds
        (Vector2Int lowest, Vector2Int highest) = SelectionGeometry.GetBoundsMatrix(positions);
        
        // check if its 1 wide
        if (lowest.x == highest.x || lowest.y == highest.y)
        {
            foreach (Vector2 pos in positions)
            {
                ManagerParameters args = new()
                {
                    Position = pos.ConvertToMatrix(),
                    FieldMode = mode,
                    Rotation = rotation,
                };
                
                ((IManager<FieldController>)FieldManager.Instance).Set(args);
            }
            
            return;
        }
        
        AdaptAreaToFieldType(lowest, highest, mode);
        
        foreach (Vector2 pos in positions)
        {
            // set field at pos
            GameObject field = Instantiate(
                mode.Prefab, pos, Quaternion.Euler(0, 0, rotation),
                ReferenceManager.Instance.FieldContainer
            );
            
            diContainer.InjectGameObject(field);
            
            FieldController fieldController = field.GetComponent<FieldController>();
            fieldController.FieldMode = mode;
            
            if (field.TryGetComponent(out ColorCalibration calibrator)) calibrator.Apply(SettingsManager.Instance.OneColorSafeFields);
            
            if (field.TryGetComponent(out FieldOutline foComp)) foComp.UpdateOnStart = false;
        }
        
        // remove player if at changed pos
        if (!mode.IsStartFieldForPlayer)
        {
            PlayerController player = PlayerManager.Instance.Player;
            
            if (player != null && player.transform.position.IsBetween(lowest.ToVector2(), highest.ToVector2())) Destroy(player.gameObject);
        }
        
        FieldManager.UpdateOutlinesInArea(mode.HasOutline, lowest, highest);
    }
    
    public void FillArea(List<Vector2> positions, EditMode editMode)
    {
        if (positions.Count == 0) return;
        
        if (editMode.Attributes.IsField)
        {
            FillAreaWithFields(positions, (FieldMode)editMode);
            return;
        }
        
        DeleteArea(positions);
        
        foreach (Vector2 pos in positions) PlaceManager.Instance.Place(editMode, pos);
        
        FieldManager.UpdateOutlinesInArea(false, positions[0].Floor(), positions.Last().Ceil());
    }
    
    public void FillArea(Vector2 start, Vector2 end, EditMode editMode) => FillArea(SelectionGeometry.GetFillRange(start, end), editMode);
    
    private void AdaptAreaToFieldType(Vector2 lowestPos, Vector2 highestPos, FieldMode mode)
    {
        // clear fields in area
        int fieldLayer = LayerManager.Instance.Layers.Field;
        int fieldCount = ReferenceManager.Instance.FieldContainer.childCount;
        Collider2D[] fieldHits = new Collider2D[fieldCount];
        _ = Physics2D.OverlapAreaNonAlloc(lowestPos, highestPos, fieldHits, fieldLayer);
        
        foreach (Collider2D fieldHit in fieldHits)
        {
            if (fieldHit == null) continue;
            
            Destroy(fieldHit.gameObject);
        }
        
        // clear coins + keys
        int entityLayer = LayerManager.Instance.Layers.Entity;
        
        bool clearCoins = CoinManager.CannotPlaceFields.Contains(mode);
        bool clearKeys = KeyManager.CannotPlaceFields.Contains(mode);
        
        if (!clearCoins && !clearKeys) return;
        
        Collider2D[] entityHits = Physics2D.OverlapAreaAll(lowestPos, highestPos, entityLayer);
        
        foreach (Collider2D hit in entityHits)
        {
            if (hit == null ||
                (!clearCoins && !hit.CompareTag("Key")) ||
                (!clearKeys && !hit.CompareTag("Coin")) ||
                (!hit.CompareTag("Coin") && !hit.CompareTag("Key"))) continue;
            
            Destroy(hit.gameObject);
        }
    }
}