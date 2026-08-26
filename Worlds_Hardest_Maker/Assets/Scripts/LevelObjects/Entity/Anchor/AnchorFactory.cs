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
    
    public AnchorController Create()
    {
        AnchorController anchor = Object.Instantiate(
            anchorPrefab, Vector2.zero, Quaternion.identity,
            anchorContainer
        ).Child;
        
        diContainer.InjectGameObject(anchor.gameObject);
        
        return anchor;
    }
}