using System.Collections.Generic;
using UnityEngine;
using WorldsHardestMaker.Selection;

namespace WorldsHardestMaker.CopyPaste
{
    public class CopyDataFactory
    {
        private readonly IAreaQueryService areaQueryService;

        public CopyDataFactory(IAreaQueryService areaQueryService)
        {
            this.areaQueryService = areaQueryService;
        }
    
        public List<CopyData> FromArea(SelectionArea area)
        {
            List<CopyData> result = new();
        
            Collider2D[] hits = areaQueryService.QueryArea(area, LayerManager.Instance.Layers.LevelObjectMask, PlaceManager.Instance.GetCurrentSheet());
        
            List<Vector2> points = HitsToPoints(hits);
        
            (Vector2 lowest, Vector2 highest) = SelectionGeometry.GetBoundsMatrix(points);
        
            // center and size of actual controllers user selected
            Vector2 centerPosition = ((lowest + highest) / 2).Floor();
        
            foreach (Collider2D hit in hits)
            {
                if (hit == null) continue;
            
                if (!hit.TryGetComponent(out LevelObjectController levelObjectController))
                {
                    Debug.LogWarning($"Could not find level object controller on hit while copying: {hit.name}");
                    continue;
                }

                if (!levelObjectController.IsCopyableNow()) continue;
            
                Data data = levelObjectController.GetData();
            
                Vector2 position = levelObjectController.transform.position;
                Vector2 relativePos = position - centerPosition;
            
                CopyData copyData = new(data, relativePos);
            
                result.Add(copyData);
            }

            return result;
        }
    
        private static List<Vector2> HitsToPoints(Collider2D[] hits)
        {
            List<Vector2> points = new();
            foreach (Collider2D hit in hits)
            {
                if (hit == null) continue;
            
                points.Add(hit.transform.position);
            }
        
            return points;
        }
    }
}