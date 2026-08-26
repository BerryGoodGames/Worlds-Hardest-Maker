using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class AnchorManager : ILevelObjectPlacer, ILevelObjectSerializer
{
    public AnchorController SetInSheet(ManagerParameters args)
    {
        if (anchorQueryService.Exists(args.Position, args.Sheet)) return null;
        
        AnchorController anchor = anchorFactory.Create(args);
        anchor.transform.position = args.Position;
        anchor.AttachmentContainerSyncTransform.Sync();
        
        // default blocks
        anchor.AppendBlock(new SetSpeedBlock(true, 5, MovementUnit.UnitsPerSecond));
        anchor.AppendBlock(new SetRotationBlock(true, 1, RotationUnit.Iterations));
        anchor.AppendBlock(new SetDirectionBlock(true, true));
        anchor.AppendBlock(new SetEaseBlock(true, Ease.Linear));
        
        BallManager.Instance.BallListSheets.Add(anchor, new());
        
        return anchor;
    }
    
    public void Remove(AnchorController anchor)
    {
        // deselect anchor first, if selected
        if (SelectedAnchor != null)
        {
            if (SelectedAnchor == anchor) DeselectAnchor();
        }
        
        BallManager.Instance.BallListSheets.Remove(anchor);
        
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
        ManagerParameters args = ManagerParameters.FromCurrentSheet(new()
        {
            Position = gridPosition
        });
        
        LevelObjectController result = SetInSheet(args);
        
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