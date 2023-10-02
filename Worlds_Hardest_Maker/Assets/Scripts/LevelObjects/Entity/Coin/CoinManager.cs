using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public static List<FieldType> CannotPlaceFields = new(new[]
    {
        FieldType.WallField,
        FieldType.RedKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.YellowKeyDoorField,
        FieldType.GrayKeyDoorField
    });

    private static readonly int playing = Animator.StringToHash("Playing");

    public int TotalCoins { get; set; }

    [PunRPC]
    public void RemoveCoin(Vector2 position)
    {
        Destroy(GetCoin(position));

        GameObject currentPlayer = PlayerManager.GetPlayer();
        if (currentPlayer != null) currentPlayer.GetComponent<PlayerController>().UncollectCoinAtPos(position);

        TotalCoins = ReferenceManager.Instance.CoinContainer.childCount - 1;
    }

    public static GameObject GetCoin(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.1f, 128);
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<CoinController>() != null) return hit.transform.parent.gameObject;
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

    private void Start() =>
        EditModeManager.Instance.OnPlay += () =>
            Instance.TotalCoins = ReferenceManager.Instance.CoinContainer.transform.childCount;

    public void SetCoin(Vector2 worldPosition)
    {
        Vector2 matrixPosition = worldPosition.ConvertToGrid();

        if (!CanPlace(matrixPosition)) return;

        TotalCoins++;
        GameObject coin = Instantiate(PrefabManager.Instance.Coin, matrixPosition, Quaternion.identity,
            ReferenceManager.Instance.CoinContainer);

        Animator anim = coin.GetComponent<Animator>();
        anim.SetBool(playing, EditModeManager.Instance.Playing);
    }
}