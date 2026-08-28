using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CoinFactory
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
    
    public CoinController Create(Vector2 position, ISheet sheet)
    {
        CoinController coin = Object.Instantiate(
            coinPrefab,
            position, Quaternion.identity,
            sheet is AnchorSheet anchorSheet ? anchorSheet.Container : coinContainer
        );
        
        diContainer.InjectGameObject(coin.gameObject);
        
        return coin;
    }
}