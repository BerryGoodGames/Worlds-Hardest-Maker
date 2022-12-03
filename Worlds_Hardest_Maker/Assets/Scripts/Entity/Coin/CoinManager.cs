using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public static List<FieldType> CantPlaceFields = new(new FieldType[]{
        FieldType.WALL_FIELD,
        FieldType.RED_KEY_DOOR_FIELD,
        FieldType.BLUE_KEY_DOOR_FIELD,
        FieldType.GREEN_KEY_DOOR_FIELD,
        FieldType.YELLOW_KEY_DOOR_FIELD,
        FieldType.GRAY_KEY_DOOR_FIELD
    });
    [HideInInspector] public int TotalCoins { get; set; } = 0;

    [PunRPC]
    public void SetCoin(float mx, float my)
    {
        if (CanPlace(mx, my))
        {
            Vector2 pos = new(mx, my);

            TotalCoins++;
            GameObject coin = Instantiate(PrefabManager.Instance.Coin, pos, Quaternion.identity, ReferenceManager.Instance.CoinContainer);
                    
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", GameManager.Instance.Playing);
        }
    }

    public void SetCoin(Vector2 pos)
    {
        SetCoin(pos.x, pos.y);
    }
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
        foreach(Collider2D hit in hits)
        {
            if(hit.GetComponent<CoinController>() != null)
            {
                return hit.transform.parent.gameObject;
            }
        }
        return null;
    }
    public static GameObject GetCoin(Vector2 pos)
    {
        return GetCoin(pos.x, pos.y);
    }

    public static bool IsCoinThere(float mx, float my)
    {
        return GetCoin(mx, my) != null;
    }

    public static bool CanPlace(float mx, float my)
    {
        // conditions: no coin there, doesnt intersect with any walls etc, no player there
        return !IsCoinThere(mx, my) && !FieldManager.IntersectingAnyFieldsAtPos(mx, my, CantPlaceFields.ToArray()) && !PlayerManager.IsPlayerThere(mx, my);
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnPlay += () => Instance.TotalCoins = ReferenceManager.Instance.CoinContainer.transform.childCount;
    }
}
