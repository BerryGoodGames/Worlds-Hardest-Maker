using System.Collections.Generic;
using MyBox;
using Photon.Pun;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public static List<FieldType> CannotPlaceFields = new(
        new[]
        {
            FieldType.WallField,
            FieldType.RedKeyDoorField,
            FieldType.BlueKeyDoorField,
            FieldType.GreenKeyDoorField,
            FieldType.YellowKeyDoorField,
            FieldType.GrayKeyDoorField,
        }
    );

    private static readonly int playing = Animator.StringToHash("Playing");
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");

    [ReadOnly] public List<CoinController> Coins = new();
    public int TotalCoins => Coins.Count;

    [PunRPC]
    public void RemoveCoin(Vector2 position)
    {
        Destroy(GetCoin(position));

        GameObject currentPlayer = PlayerManager.GetPlayer();
        if (currentPlayer != null) currentPlayer.GetComponent<PlayerController>().UncollectCoinAtPos(position);
    }

    public static GameObject GetCoin(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<CoinController>() != null) return hit.gameObject;
        }

        return null;
    }

    public static bool IsCoinThere(Vector2 position) => GetCoin(position) != null;

    public static bool CanPlace(Vector2 position) =>
        // conditions: no coin there, doesn't intersect with any walls etc, no player there
        !IsCoinThere(position) &&
        !FieldManager.IntersectingAnyFieldsAtPos(position, CannotPlaceFields.ToArray()) &&
        !PlayerManager.IsPlayerThere(position);

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    public void SetCoin(Vector2 worldPosition)
    {
        Vector2 matrixPosition = worldPosition.ConvertToGrid();

        if (!CanPlace(matrixPosition)) return;

        CoinController coin = Instantiate(
            PrefabManager.Instance.Coin, matrixPosition, Quaternion.identity,
            ReferenceManager.Instance.CoinContainer
        );

        coin.Animator.SetBool(playing, EditModeManager.Instance.Playing);
    }

    public void Place(Vector2 worldPosition) { }

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
}