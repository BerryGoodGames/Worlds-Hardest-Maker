using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

[Serializable]
public class AreaFillService : IAreaFillService
{
    [Inject] private IObjectResolver diContainer;
    [Inject] private IAreaQueryService areaQueryService;
    [Inject] private IAreaErasureService areaErasureService;
    
    public void FillAreaWithFields(SelectionArea area, FieldMode mode, Transform fieldContainer, Transform playerContainer)
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
        
        AdaptAreaToFieldType(area, mode);
        
        foreach (Vector2 pos in positions)
        {
            // TODO: extract field instantiation logic to factory
            // set field at pos
            GameObject field = Object.Instantiate(
                mode.Prefab, pos, Quaternion.Euler(0, 0, rotation),
                fieldContainer
            );
            
            diContainer.InjectGameObject(field);
            
            FieldController fieldController = field.GetComponent<FieldController>();
            fieldController.Initialize(playerContainer);
            fieldController.FieldMode = mode;
            
            if (field.TryGetComponent(out ColorCalibration calibrator)) calibrator.Apply(SettingsManager.Instance.OneColorSafeFields);
            
            if (field.TryGetComponent(out FieldOutline foComp)) foComp.UpdateOnStart = false;
        }
        
        // remove player if at changed pos
        if (!mode.IsStartFieldForPlayer)
        {
            PlayerController player = PlayerManager.Instance.Player;
            
            if (player != null && player.transform.position.IsBetween(lowest.ToVector2(), highest.ToVector2())) Object.Destroy(player.gameObject);
        }
        
        FieldManager.UpdateOutlinesInArea(mode.HasOutline, area);
    }

    public void FillArea(SelectionArea area, EditMode editMode, Transform fieldContainer, Transform playerContainer)
    {
        IReadOnlyList<Vector2> positions = area.Positions;
        
        if (positions.Count == 0) return;
        
        // TODO: somehow use strategy pattern to avoid this if statement
        if (editMode.Attributes.IsField)
        {
            FillAreaWithFields(area, (FieldMode)editMode, fieldContainer, playerContainer);
            return;
        }
        
        areaErasureService.EraseArea(area);
        
        foreach (Vector2 pos in positions) PlaceManager.Instance.Place(editMode, pos);
        
        FieldManager.UpdateOutlinesInArea(false, area);
    }

    public void AdaptAreaToFieldType(SelectionArea area, FieldMode mode)
    {
        // clear fields in area
        int fieldLayer = LayerManager.Instance.Layers.Field;

        Collider2D[] fieldHits = areaQueryService.QueryArea(area, fieldLayer);

        foreach (Collider2D fieldHit in fieldHits)
        {
            if (fieldHit == null) continue;

            Object.Destroy(fieldHit.gameObject);
        }

        // clear coins + keys
        int entityLayer = LayerManager.Instance.Layers.Entity;

        bool clearCoins = CoinManager.CannotPlaceFields.Contains(mode);
        bool clearKeys = KeyManager.CannotPlaceFields.Contains(mode);

        if (!clearCoins && !clearKeys) return;

        Collider2D[] entityHits = areaQueryService.QueryArea(area, entityLayer);

        HashSet<string> tagsToClear = new();
        if (clearCoins) tagsToClear.Add("Coin");
        if (clearKeys) tagsToClear.Add("Key");

        if (tagsToClear.Count == 0) return;

        foreach (Collider2D hit in entityHits)
        {
            if (hit == null) continue;
            if (!tagsToClear.Contains(hit.tag)) continue;

            Object.Destroy(hit.gameObject);
        }
    }
}