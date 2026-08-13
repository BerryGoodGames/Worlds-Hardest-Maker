using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using VContainer.Unity;

public partial class SelectionManager
{
    // TODO: extract fill logic to service
    public void FillSelectedArea()
    {
        FillArea(selectionAreaProvider.GetArea(), LevelSessionEditManager.Instance.CurrentEditMode);
        
        selectionStateService.ClearSelection();
    }
    
    private void FillAreaWithFields(SelectionArea area, FieldMode mode)
    {
        IReadOnlyList<Vector2> positions = area.Positions;
        
        // set rotation
        int rotation = mode.IsRotatable ? LevelSessionEditManager.Instance.EditRotation : 0;
        
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
            // TODO: extract field instantiation logic to factory
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
    
    private void FillArea(SelectionArea area, EditMode editMode)
    {
        IReadOnlyList<Vector2> positions = area.Positions;
        
        if (positions.Count == 0) return;
        
        // TODO: somehow use strategy pattern to avoid this if statement
        if (editMode.Attributes.IsField)
        {
            FillAreaWithFields(area, (FieldMode)editMode);
            return;
        }
        
        DeleteArea(area);
        
        foreach (Vector2 pos in positions) PlaceManager.Instance.Place(editMode, pos);
        
        FieldManager.UpdateOutlinesInArea(false, positions[0].Floor(), positions.Last().Ceil());
    }
    
    public void FillArea(Vector2 start, Vector2 end, EditMode editMode) => FillArea(SelectionGeometry.GetFillArea(start, end), editMode);

    private void AdaptAreaToFieldType(Vector2 lowestPos, Vector2 highestPos, FieldMode mode)
    {
        // clear fields in area
        int fieldLayer = LayerManager.Instance.Layers.Field;
        int fieldCount = ReferenceManager.Instance.FieldContainer.childCount;

        // TODO: extract physics query logic to a service
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

        // TODO: extract physics query logic to a service
        Collider2D[] entityHits = Physics2D.OverlapAreaAll(lowestPos, highestPos, entityLayer);

        HashSet<string> tagsToClear = new();
        if (clearCoins) tagsToClear.Add("Coin");
        if (clearKeys) tagsToClear.Add("Key");

        if (tagsToClear.Count == 0) return;

        foreach (Collider2D hit in entityHits)
        {
            if (hit == null) continue;
            if (!tagsToClear.Contains(hit.tag)) continue;

            Destroy(hit.gameObject);
        }
    }
}