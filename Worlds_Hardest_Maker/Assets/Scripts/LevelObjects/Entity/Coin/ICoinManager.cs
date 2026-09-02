using System.Collections.Generic;

public interface ICoinManager
{
    // TODO: extract (also keys)
    public IReadOnlyList<CoinController> CollectedCoins { get; }
    public int CoinsNeededFinal { get; }
    
    public void CollectCoin(CoinController coin);
    public void UncollectCoin(CoinController coin);
    public bool AllCoinsCollected();
    public void RemoveCollectedCoinNulls();
    public void ClearCollectedCoins();
}