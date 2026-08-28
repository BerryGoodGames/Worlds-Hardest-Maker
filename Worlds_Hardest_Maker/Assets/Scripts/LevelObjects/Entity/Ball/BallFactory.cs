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
    
    public BallController Create(Vector2 position, ISheet sheet)
    {
        Transform container = sheet is AnchorSheet anchorSheet ? anchorSheet.Container : ballContainer;
        
        GameObject ball = Object.Instantiate(
            ballPrefab,
            position, Quaternion.identity,
            container
        );
        
        diContainer.InjectGameObject(ball);
        
        return ball.GetComponentInChildren<BallController>();
    }
}