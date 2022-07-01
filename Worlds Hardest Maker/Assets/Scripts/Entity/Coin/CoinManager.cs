using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static List<FieldManager.FieldType> CantPlaceFields = new(new FieldManager.FieldType[]{
        FieldManager.FieldType.WALL_FIELD,
        FieldManager.FieldType.RED_KEY_DOOR_FIELD,
        FieldManager.FieldType.BLUE_KEY_DOOR_FIELD,
        FieldManager.FieldType.GREEN_KEY_DOOR_FIELD,
        FieldManager.FieldType.YELLOW_KEY_DOOR_FIELD,
        FieldManager.FieldType.GRAY_KEY_DOOR_FIELD
    });

    public static void SetCoin(int mx, int my)
    {
        Vector2 pos = new(mx, my);

        // place coin if no coin is there, the field isnt a cantplacefield or default, the player is not there
        if(!IsCoinThere(mx, my))
        {
            GameObject currentField = FieldManager.GetField(mx, my);
            FieldManager.FieldType? type = FieldManager.GetFieldType(currentField);
            
            if(type == null || !CantPlaceFields.Contains((FieldManager.FieldType)type))
            {
                GameObject player = PlayerManager.GetCurrentPlayer();
                if (player == null || (Vector2)player.transform.position != pos)
                {
                    Instantiate(GameManager.Instance.Coin, pos, Quaternion.identity, GameManager.Instance.CoinContainer.transform);
                }
            }
        }
    }
    public static void RemoveCoin(int mx, int my)
    {
        GameObject container = GameManager.Instance.CoinContainer;
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.25f);
        foreach (Collider2D hit in hits)
        {
            Transform parent = hit.transform.parent;
            if (parent.parent == container.transform)
            {
                Destroy(parent.gameObject);
            }
        }
    }
    public static GameObject GetCoin(int mx, int my)
    {
        GameObject container = GameManager.Instance.CoinContainer;
        foreach (Transform coin in container.transform)
        {
            if ((Vector2)coin.position == new Vector2(mx, my))
            {
                return coin.gameObject;
            }
        }
        return null;
    }
    public static bool IsCoinThere(int mx, int my)
    {
        return GetCoin(mx, my) != null;
    }
}
