using DG.Tweening;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AnchorFactory
{
    private readonly IObjectResolver diContainer;
    
    private AnchorParentController anchorPrefab;
    private Transform anchorContainer;

    public AnchorFactory(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }

    public void Initialize(AnchorParentController anchorPrefab, Transform anchorContainer)
    {
        this.anchorPrefab = anchorPrefab;
        this.anchorContainer = anchorContainer;
    }
    
    public AnchorController Create(Vector2 position)
    {
        AnchorController anchor = Object.Instantiate(
            anchorPrefab, position, Quaternion.identity,
            anchorContainer
        ).Child;
        
        diContainer.InjectGameObject(anchor.gameObject);
        
        anchor.AttachmentContainerSyncTransform.Sync();
        
        // default blocks
        anchor.AppendBlock(new SetSpeedBlock(true, 5, MovementUnit.UnitsPerSecond));
        anchor.AppendBlock(new SetRotationBlock(true, 1, RotationUnit.Iterations));
        anchor.AppendBlock(new SetDirectionBlock(true, true));
        anchor.AppendBlock(new SetEaseBlock(true, Ease.Linear));
        
        return anchor;
    }
}