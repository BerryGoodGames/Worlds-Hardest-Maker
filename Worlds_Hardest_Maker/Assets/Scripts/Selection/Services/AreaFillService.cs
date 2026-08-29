using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using WorldsHardestMaker.Selection;
using Object = UnityEngine.Object;

[Serializable]
public class AreaFillService : IAreaFillService
{
    [Inject] private IObjectResolver diContainer;
    [Inject] private IAreaQueryService areaQueryService;
    [Inject] private IAreaErasureService areaErasureService;
    [Inject] private FieldFactory fieldFactory;
    
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
                FieldManager.Instance.CreateNew(pos.ConvertToMatrix(),
                    rotation,
                    PlaceManager.GetCurrentSheet(),
                    mode);
            }
            
            return;
        }
        
        AdaptAreaToFieldType(area, mode);
        
        foreach (Vector2 pos in positions)
        {
            FieldController newField = fieldFactory.Create(pos, rotation, GlobalSheet.Instance, mode);
            
            if (newField.TryGetComponent(out ColorCalibration calibrator)) calibrator.Apply(SettingsManager.Instance.OneColorSafeFields);
            
            if (newField.TryGetComponent(out FieldOutline foComp)) foComp.UpdateOnStart = false;
        }
        
        // remove player if at changed pos
        if (!mode.IsStartFieldForPlayer)
        {
            PlayerController player = PlayerManager.Instance.Player;
            
            if (player != null && player.transform.position.IsBetween(lowest.ToVector2(), highest.ToVector2())) Object.Destroy(player.gameObject);
        }
        
        FieldManager.Instance.UpdateOutlinesInArea(mode.HasOutline, area);
    }

    public void FillArea(SelectionArea area, EditMode editMode, Transform fieldContainer, Transform playerContainer)
    {
        IReadOnlyList<Vector2> positions = area.Positions;
        
        if (positions.Count == 0) return;
        
        // TODO: somehow use strategy pattern to avoid this if statement
        if (editMode is FieldMode fieldMode)
        {
            FillAreaWithFields(area, fieldMode, fieldContainer, playerContainer);
            return;
        }
        
        areaErasureService.EraseArea(area);
        
        foreach (Vector2 pos in positions) PlaceManager.Instance.Place(editMode, pos);
        
        FieldManager.Instance.UpdateOutlinesInArea(false, area);
    }

    public void AdaptAreaToFieldType(SelectionArea area, FieldMode mode)
    {
        // clear fields in area
        int fieldLayer = LayerManager.Instance.Layers.Field;

        Collider2D[] fieldHits = areaQueryService.QueryArea(area, fieldLayer, PlaceManager.GetCurrentSheet());

        foreach (Collider2D fieldHit in fieldHits)
        {
            if (fieldHit == null) continue;
            
            Object.Destroy(fieldHit.gameObject);
        }
    }
}