using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BallFactory
{
    private readonly IObjectResolver diContainer;
    
    private GameObject ballPrefab;
    private Transform ballContainer;

    public BallFactory(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }

    public void Initialize(GameObject ballPrefab, Transform ballContainer)
    {
        this.ballPrefab = ballPrefab;
        this.ballContainer = ballContainer;
    }
    
    public BallController Create(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        Transform container = sheet == null ? ballContainer : sheet.AttachmentContainer;
        
        GameObject ball = Object.Instantiate(
            ballPrefab,
            position, Quaternion.identity,
            container
        );
        
        diContainer.InjectGameObject(ball);
        
        return ball.GetComponentInChildren<BallController>();
    }
}