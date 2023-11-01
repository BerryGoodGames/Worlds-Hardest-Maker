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
    public void SetCoin(float mx, float my)
    {
        if (!CanPlace(mx, my)) return;

        Vector2 pos = new(mx, my);

        TotalCoins++;
        GameObject coin = Instantiate(PrefabManager.Instance.Coin, pos, Quaternion.identity,
            ReferenceManager.Instance.CoinContainer);

        Animator anim = coin.GetComponent<Animator>();
        anim.SetBool(playing, EditModeManager.Instance.Playing);
    }

    public void SetCoin(Vector2 pos) => SetCoin(pos.x, pos.y);

    [PunRPC]
    public void RemoveCoin(float mx, float my)
    {
        Destroy(GetCoin(mx, my));

        GameObject currentPlayer = PlayerManager.GetPlayer();
        if (currentPlayer != null) currentPlayer.GetComponent<PlayerController>().UncollectCoinAtPos(new(mx, my));

        TotalCoins = ReferenceManager.Instance.CoinContainer.childCount - 1;
    }

    public static GameObject GetCoin(float mx, float my)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.1f, 128);
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<CoinController>() != null) return hit.gameObject;
        }

        return null;
    }

    public static GameObject GetCoin(Vector2 pos) => GetCoin(pos.x, pos.y);

    public static bool IsCoinThere(float mx, float my) => GetCoin(mx, my) != null;

    public static bool CanPlace(float mx, float my) =>
        // conditions: no coin there, doesn't intersect with any walls etc, no player there
        !IsCoinThere(mx, my) &&
        !FieldManager.IntersectingAnyFieldsAtPos(mx, my, CannotPlaceFields.ToArray()) &&
        !PlayerManager.IsPlayerThere(mx, my);

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    private void Start() =>
        EditModeManager.Instance.OnPlay += () =>
            Instance.TotalCoins = ReferenceManager.Instance.CoinContainer.transform.childCount;
}