using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MCoin : MonoBehaviour
{
    public static MCoin Instance { get; private set; }

    public static List<MField.FieldType> CantPlaceFields = new(new MField.FieldType[]{
        MField.FieldType.WALL_FIELD,
        MField.FieldType.RED_KEY_DOOR_FIELD,
        MField.FieldType.BLUE_KEY_DOOR_FIELD,
        MField.FieldType.GREEN_KEY_DOOR_FIELD,
        MField.FieldType.YELLOW_KEY_DOOR_FIELD,
        MField.FieldType.GRAY_KEY_DOOR_FIELD
    });

    [PunRPC]
    public void SetCoin(float mx, float my)
    {
        if (CanPlace(mx, my))
        {
            Vector2 pos = new(mx, my);

            MGame.Instance.TotalCoins++;
            GameObject coin = Instantiate(MGame.Instance.Coin, pos, Quaternion.identity, MGame.Instance.CoinContainer.transform);
                    
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", MGame.Instance.Playing);
        }
    }
    [PunRPC]
    public void RemoveCoin(float mx, float my)
    {
        foreach (Transform coin in MGame.Instance.CoinContainer.transform)
        {
            if (coin.GetChild(0).GetComponent<CCoin>().coinPosition == new Vector2(mx, my))
            {
                Destroy(coin.gameObject);

                GameObject currentPlayer = MPlayer.GetPlayer();
                if (currentPlayer != null) currentPlayer.GetComponent<CPlayer>().UncollectCoinAtPos(new(mx, my));

                MGame.Instance.TotalCoins = MGame.Instance.CoinContainer.transform.childCount - 1;
            }
        }
    }
    public static GameObject GetCoin(float mx, float my)
    {
        GameObject container = MGame.Instance.CoinContainer;
        foreach (Transform coin in container.transform)
        {
            CCoin controller = coin.GetChild(0).GetComponent<CCoin>();
            if (controller.coinPosition == new Vector2(mx, my))
            {
                return coin.gameObject;
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
        return !IsCoinThere(mx, my) && !MField.IntersectingAnyFieldsAtPos(mx, my, CantPlaceFields.ToArray()) && !MPlayer.IsPlayerThere(mx, my);
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
