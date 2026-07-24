using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using VContainer.Unity;

public partial class SelectionManager
{
    public static List<Vector2> GetFillRange(Vector2 p1, Vector2 p2)
    {
        bool inMatrix = LevelSessionEditManager.Instance.CurrentEditMode.GetWorldPositionType() is WorldPositionType.Matrix;
        
        // find bounds
        (Vector2 lowest, Vector2 highest) = inMatrix ? GetBoundsMatrix(p1, p2) : GetBounds(p1, p2);
        
        // collect every pos in range
        float increment = inMatrix ? 1 : 0.5f;
        List<Vector2> res = new();
        for (float x = lowest.x; x <= highest.x; x += increment)
        {
            for (float y = lowest.y; y <= highest.y; y += increment) res.Add(new(x, y));
        }
        
        return res;
    }
    
    public static List<Vector2> GetCurrentFillRange()
    {
        if (SelectionStart == null || SelectionEnd == null) return null;
        return GetFillRange((Vector2)SelectionStart, (Vector2)SelectionEnd);
    }
    
    public void FillSelectedArea()
    {
        if (!Selecting) return;
        
        FillArea(CurrentSelectionRange, LevelSessionEditManager.Instance.CurrentEditMode);
        ResetPreview();
        Selecting = false;
        selectionOptions.gameObject.SetActive(false);
    }
    
    public void FillAreaWithFields(List<Vector2> poses, FieldMode mode)
    {
        // set rotation
        int rotation = mode.IsRotatable
            ? LevelSessionEditManager.Instance.EditRotation
            : 0;
        
        // find bounds
        (Vector2Int lowest, Vector2Int highest) = GetBoundsMatrix(poses);
        
        // check if its 1 wide
        if (lowest.x == highest.x || lowest.y == highest.y)
        {
            foreach (Vector2 pos in poses)
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
        
        foreach (Vector2 pos in poses)
        {
            // set field at pos
            GameObject field = Instantiate(
                mode.Prefab, pos, Quaternion.Euler(0, 0, rotation),
                ReferenceManager.Instance.FieldContainer
            );
            
            IObjectResolver.InjectGameObject(field);
            
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
    
    public void FillArea(List<Vector2> poses, EditMode editMode)
    {
        if (poses.Count == 0) return;
        
        if (editMode.Attributes.IsField)
        {
            FillAreaWithFields(poses, (FieldMode)editMode);
            return;
        }
        
        DeleteArea(poses);
        
        foreach (Vector2 pos in poses) PlaceManager.Instance.Place(editMode, pos);
        
        FieldManager.UpdateOutlinesInArea(false, poses[0].Floor(), poses.Last().Ceil());
    }
    
    public void FillArea(Vector2 start, Vector2 end, EditMode editMode) => FillArea(GetFillRange(start, end), editMode);
    
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