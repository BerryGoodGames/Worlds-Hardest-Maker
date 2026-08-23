using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VContainer.Unity;

public partial class AnchorManager : IManager<AnchorController>, ILevelObjectManager, ILevelObjectSerializer
{
    public AnchorController SetInSheet(ManagerParameters args)
    {
        if (((IManager<AnchorController>)this).IsThereInSheet(args.Position, args.Sheet)) return null;
        
        AnchorController anchor = InstantiateInSheet(args);
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
    
    public AnchorController GetInSheet(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f);
        
        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent.CompareTag("Anchor")) return hit.gameObject.GetComponent<AnchorController>();
        }
        
        return null;
    }
    
    public void Remove(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, 128);
        
        foreach (Collider2D hit in hits)
        {
            // check tag
            if (!hit.transform.parent.CompareTag("Anchor")) continue;
            
            Remove(hit.GetComponent<AnchorController>());
            break;
        }
    }
    
    public void Remove(AnchorController anchor)
    {
        // deselect anchor first, if selected
        if (Instance.SelectedAnchor != null)
        {
            if (Instance.SelectedAnchor == anchor) Instance.DeselectAnchor();
        }
        
        BallManager.Instance.BallListSheets.Remove(anchor);
        
        // destroy anchor
        Destroy(anchor.transform.parent.gameObject);
        
        audioService.Play(PlaceManager.Instance.GetSfx(EditModeManager.Delete));
    }
    
    public AnchorController InstantiateInSheet(ManagerParameters args)
    {
        AnchorController anchor = Instantiate(
            anchorPrefab, Vector2.zero, Quaternion.identity,
            anchorContainer
        ).Child;
        
        diContainer.InjectGameObject(anchor.gameObject);
        
        return anchor;
    }

    public LevelObjectController PlaceLevelObject(ManagerParameters args) => SetInSheet(args);

    public List<Data> Serialize(List<Data> levelData)
    {
        foreach (Transform anchor in anchorContainer)
        {
            AnchorData anchorData = new(anchor.GetComponent<AnchorParentController>().Child);
            levelData.Add(anchorData);
        }
        
        return levelData;
    }
    
    public bool CorrespondsToEditMode(EditMode compare) => compare == EditModeManager.Anchor;
    
    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Anchor;
    }

    public bool Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        ManagerParameters args = ManagerParameters.FromCurrentSheet(new()
        {
            Position = gridPosition
        });
        
        LevelObjectController result = SetInSheet(args);
        
        Select((AnchorController)result);
        LastSelectClick = Time.time;
        
        return result;
    }

    public LevelObjectController Query(Vector2 position, AnchorController sheet)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(Vector2 position, AnchorController sheet)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<Data> Serialize()
    {
        throw new System.NotImplementedException();
    }
}