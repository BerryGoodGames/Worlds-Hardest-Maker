using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [UsedImplicitly] public static readonly List<FieldMode> CannotPlaceFields = new();

    [ReadOnly] public List<CoinController> Coins = new();
    [ReadOnly] public List<CoinController> CollectedCoins = new();

    private int TotalCoins => Coins.Count;

    public int CoinsNeededFinal =>
        Mathf.Min(LevelSettings.Instance.IsCoinsNeededLimited ? LevelSettings.Instance.CoinsNeeded : TotalCoins, TotalCoins);

    private static readonly int playing = Animator.StringToHash("Playing");


    public void RemoveCoin(Vector2 position)
    {
        Destroy(GetCoin(position));

        PlayerController currentPlayer = PlayerManager.Instance.Player;
        if (currentPlayer != null) UncollectCoinAtPos(position);
    }

    public static CoinController GetCoin(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out CoinController c)) return c;
        }

        return null;
    }
    
    public static CoinController GetCoinInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Coin")) continue;
            if (!hit.TryGetComponent(out CoinController coin)) continue;
            if (IsCoinInSheet(coin, sheet)) return coin;
        }

        return null;
    }

    public static bool IsCoinInSheet(CoinController coin, [CanBeNull] AnchorController sheet)
    {
        bool hasAttachment = coin.TryGetComponent(out AnchorAttachment attachment);
        
        // shorthand to:
        bool globalSheet = sheet == null;
        if (hasAttachment && globalSheet) return false;
        if (hasAttachment && attachment.Anchor != sheet) return false;
        if (!hasAttachment && !globalSheet) return false;
        return true;

        // return (globalSheet && !hasAttachment) || (hasAttachment && !globalSheet && attachment.Anchor == sheet);
    }

    public static bool IsCoinThere(Vector2 position) => GetCoin(position) != null;
    public static bool IsCoinThereInSheet(Vector2 position, [CanBeNull] AnchorController sheet) => GetCoinInSheet(position, sheet) != null;

    public static bool CanPlace(Vector2 position) => CanPlace(position, PlaceManager.GetCurrentSheet());

    public static bool CanPlace(Vector2 position, [CanBeNull] AnchorController sheet) =>
        // conditions: no coin there, doesn't intersect with any walls etc, no player there
        !IsCoinThereInSheet(position, sheet)
        && !FieldManager.IntersectingAnyFieldsAtPos(position, CannotPlaceFields.ToArray())
        && !PlayerManager.IsPlayerThere(position);

    public static CoinController SetCoinInSheet(Vector2 worldPosition, [CanBeNull] AnchorController sheet)
    {
        Transform container = AnchorAttachManager.Instance.InAttachMode
            ? AnchorAttachManager.GetCurrentAnchorContainer()
            : ReferenceManager.Instance.CoinContainer;
        
        Vector2 matrixPosition = worldPosition.ConvertToGrid();

        if (!CanPlace(matrixPosition, sheet)) return null;

        CoinController coin = Instantiate(
            PrefabManager.Instance.Coin, 
            matrixPosition, Quaternion.identity,
            container
        );

        coin.Animator.SetBool(playing, LevelSessionEditManager.Instance.Playing);
        
        PlaceManager.AttachToSheet(coin.gameObject, sheet);

        return coin;
    }

    public static CoinController SetCoin(Vector2 worldPosition) => SetCoinInSheet(worldPosition, PlaceManager.GetCurrentSheet());

    public void UncollectCoinAtPos(Vector2 position)
    {
        for (int i = CollectedCoins.Count - 1; i >= 0; i--)
        {
            CoinController c = CollectedCoins[i];
            if (c.CoinPosition == position) CollectedCoins.Remove(c);
        }
    }

    public bool AllCoinsCollected() => CollectedCoins.Count >= CoinsNeededFinal;

    public void ActivateAnimations() => Coins.ForEach(coin => coin.ActivateAnimation());

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}