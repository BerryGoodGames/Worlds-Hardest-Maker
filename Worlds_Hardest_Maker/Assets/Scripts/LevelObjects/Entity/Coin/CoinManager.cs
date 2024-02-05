using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [UsedImplicitly] public static readonly List<FieldMode> CannotPlaceFields = new();

    [SerializeField] [InitializationField] [MustBeAssigned] private NumberInput coinsNeededInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private Toggle isCoinsNeededLimitedInput;
    [Separator]
    [ReadOnly] public List<CoinController> Coins = new();
    [ReadOnly] public List<CoinController> CollectedCoins = new();

    private int TotalCoins => Coins.Count;
    private int coinsNeeded;
    private bool isCoinsNeededLimited;

    public int CoinsNeededFinal => Mathf.Min(isCoinsNeededLimited ? coinsNeeded : TotalCoins, TotalCoins);

    private static readonly int playing = Animator.StringToHash("Playing");
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");

    
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

    public static bool IsCoinThere(Vector2 position) => GetCoin(position) != null;

    public static bool CanPlace(Vector2 position) =>
        // conditions: no coin there, doesn't intersect with any walls etc, no player there
        !IsCoinThere(position)
        && !FieldManager.IntersectingAnyFieldsAtPos(position, CannotPlaceFields.ToArray())
        && !PlayerManager.IsPlayerThere(position);

    public static CoinController SetCoin(Vector2 worldPosition)
    {
        Vector2 matrixPosition = worldPosition.ConvertToGrid();

        if (!CanPlace(matrixPosition)) return null;

        CoinController coin = Instantiate(
            PrefabManager.Instance.Coin, matrixPosition, Quaternion.identity,
            ReferenceManager.Instance.CoinContainer
        );

        coin.Animator.SetBool(playing, LevelSessionEditManager.Instance.Playing);

        return coin;
    }
    
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

    private void SetCoinsNeeded(int value) => coinsNeeded = value;
    public void SetCoinsNeeded() => SetCoinsNeeded((int)coinsNeededInput.GetCurrentNumber());
    public void SetIsNeededCoinsLimited() => isCoinsNeededLimited = isCoinsNeededLimitedInput.isOn;

    private void Start()
    {
        SetCoinsNeeded();
        SetIsNeededCoinsLimited();
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}