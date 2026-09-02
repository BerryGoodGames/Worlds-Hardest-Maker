using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class AnchorManager : ILevelObjectPlacer, ILevelObjectSerializer
{
    public AnchorController CreateNew(Vector2 position, ISheet sheet)
    {
        if (anchorQueryService.Exists(position, sheet)) return null;
        
        AnchorController anchor = anchorFactory.Create(position);
        
        // TODO: need to add bucket to sheet scoped registry?
        // BallManager.Instance.BallListSheets.Add(anchor, new());
        
        return anchor;
    }
    
    public void Remove(AnchorController anchor)
    {
        // deselect anchor first, if selected
        if (SelectedAnchor != null)
        {
            if (SelectedAnchor == anchor) DeselectAnchor();
        }
        
        // TODO: need to remove bucket from sheet scoped registry?
        // BallManager.Instance.BallListSheets.Remove(anchor);
        
        // destroy anchor
        Destroy(anchor.transform.parent.gameObject);
        
        audioService.Play(PlaceManager.Instance.GetSfx(EditModeManager.Delete));
    }

    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Anchor;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        
        LevelObjectController result = CreateNew(gridPosition, PlaceManager.Instance.GetCurrentSheet());
        
        Select((AnchorController)result);
        LastSelectClick = Time.time;

        return PlacementResult.FromController(result);
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();
        foreach (Transform anchor in anchorContainer)
        {
            AnchorData anchorData = new(anchor.GetComponent<AnchorParentController>().Child);
            levelData.Add(anchorData);
        }
        
        return levelData;
    }
}