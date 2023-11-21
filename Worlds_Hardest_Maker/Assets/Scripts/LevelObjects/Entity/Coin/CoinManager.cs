using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using Photon.Pun;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [UsedImplicitly] public static readonly List<FieldType> CannotPlaceFields = new();

    private static readonly int playing = Animator.StringToHash("Playing");
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");

    [ReadOnly] public List<CoinController> Coins = new();
    public int TotalCoins => Coins.Count;

    [PunRPC]
    public void RemoveCoin(Vector2 position)
    {
        Destroy(GetCoin(position));

        PlayerController currentPlayer = PlayerManager.GetPlayer();
        if (currentPlayer != null) currentPlayer.UncollectCoinAtPos(position);
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
        !IsCoinThere(position) &&
        !FieldManager.IntersectingAnyFieldsAtPos(position, CannotPlaceFields.ToArray()) &&
        !PlayerManager.IsPlayerThere(position);

    public static CoinController SetCoin(Vector2 worldPosition)
    {
        Vector2 matrixPosition = worldPosition.ConvertToGrid();

        if (!CanPlace(matrixPosition)) return null;

        CoinController coin = Instantiate(
            PrefabManager.Instance.Coin, matrixPosition, Quaternion.identity,
            ReferenceManager.Instance.CoinContainer
        );

        coin.Animator.SetBool(playing, EditModeManager.Instance.Playing);

        return coin;
    }

    public void ResetStates()
    {
        // reset coins
        foreach (CoinController coin in Coins)
        {
            coin.PickedUp = false;

            coin.Animator.SetBool(playing, false);
            coin.Animator.SetBool(pickedUp, false);
        }
    }

    public void ActivateAnimations()
    {
        // activate coin animations
        foreach (CoinController coin in Coins)
        {
            coin.Animator.SetBool(playing, true);
            coin.Animator.SetBool(pickedUp, coin.PickedUp);
        }
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}