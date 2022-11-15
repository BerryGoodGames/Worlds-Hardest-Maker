using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public static List<FieldManager.FieldType> CantPlaceFields = new(new FieldManager.FieldType[]{
        FieldManager.FieldType.WALL_FIELD,
        FieldManager.FieldType.RED_KEY_DOOR_FIELD,
        FieldManager.FieldType.BLUE_KEY_DOOR_FIELD,
        FieldManager.FieldType.GREEN_KEY_DOOR_FIELD,
        FieldManager.FieldType.YELLOW_KEY_DOOR_FIELD,
        FieldManager.FieldType.GRAY_KEY_DOOR_FIELD
    });

    [PunRPC]
    public void SetCoin(float mx, float my)
    {
        if (CanPlace(mx, my))
        {
            Vector2 pos = new(mx, my);

            GameManager.Instance.TotalCoins++;
            GameObject coin = Instantiate(GameManager.Instance.Coin, pos, Quaternion.identity, GameManager.Instance.CoinContainer.transform);
                    
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool("Playing", GameManager.Instance.Playing);
        }
    }
    [PunRPC]
    public void RemoveCoin(float mx, float my)
    {
        foreach (Transform coin in GameManager.Instance.CoinContainer.transform)
        {
            if (coin.GetChild(0).GetComponent<CoinController>().coinPosition == new Vector2(mx, my))
            {
                Destroy(coin.gameObject);

                GameObject currentPlayer = PlayerManager.GetPlayer();
                if (currentPlayer != null) currentPlayer.GetComponent<PlayerController>().UncollectCoinAtPos(new(mx, my));

                GameManager.Instance.TotalCoins = GameManager.Instance.CoinContainer.transform.childCount - 1;
            }
        }
    }
    public static GameObject GetCoin(float mx, float my)
    {
        GameObject container = GameManager.Instance.CoinContainer;
        foreach (Transform coin in container.transform)
        {
            CoinController controller = coin.GetChild(0).GetComponent<CoinController>();
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
        return !IsCoinThere(mx, my) && !FieldManager.IntersectingAnyFieldsAtPos(mx, my, CantPlaceFields.ToArray()) && !PlayerManager.IsPlayerThere(mx, my);
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
