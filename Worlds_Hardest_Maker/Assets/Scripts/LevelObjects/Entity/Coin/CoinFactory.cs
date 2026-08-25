using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CoinFactory : ILevelObjectFactory<CoinController>
{
    private readonly IObjectResolver diContainer;
    
    private CoinController coinPrefab;
    private Transform coinContainer;

    public CoinFactory(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }

    public void Initialize(CoinController coinPrefab, Transform coinContainer)
    {
        this.coinPrefab = coinPrefab;
        this.coinContainer = coinContainer;
    }
    
    public CoinController Create(ManagerParameters args)
    {
        CoinController coin = Object.Instantiate(
            coinPrefab,
            args.Position, Quaternion.identity,
            args.Sheet == null ? coinContainer : args.Sheet.AttachmentContainer
        );
        
        diContainer.InjectGameObject(coin.gameObject);
        
        return coin;
    }
}