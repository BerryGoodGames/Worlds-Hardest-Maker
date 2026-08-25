using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BallFactory : ILevelObjectFactory<BallController>
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
    
    public BallController Create(ManagerParameters args)
    {
        Transform container = args.Sheet == null ? ballContainer : args.Sheet.AttachmentContainer;
        
        GameObject ball = Object.Instantiate(
            ballPrefab,
            args.Position, Quaternion.identity,
            container
        );
        
        diContainer.InjectGameObject(ball);
        
        return ball.GetComponentInChildren<BallController>();
    }
}