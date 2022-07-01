using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages player duh
/// </summary>
public class PlayerManager : MonoBehaviour
{
    // list of fields which are safe for player
    public static readonly List<FieldManager.FieldType> SafeFields = new(new FieldManager.FieldType[]{
        FieldManager.FieldType.START_FIELD,
        FieldManager.FieldType.CHECKPOINT_FIELD,
        FieldManager.FieldType.START_AND_GOAL_FIELD
    });
    public static readonly List<FieldManager.FieldType> StartFields = new(new FieldManager.FieldType[]
    {
        FieldManager.FieldType.START_FIELD,
        FieldManager.FieldType.START_AND_GOAL_FIELD
    });

    public static void SetPlayer(int mx, int my, float speed = 3)
    {
        // check if field there exists
        GameObject field = FieldManager.GetField(mx, my);
        if (field != null)
        {
            // check if field at mx my is start field
            FieldManager.FieldType? typeAtPos = FieldManager.GetFieldType(field);
            if (typeAtPos == FieldManager.FieldType.START_FIELD || typeAtPos == FieldManager.FieldType.START_AND_GOAL_FIELD)
            {
                // clear area from players, coins and keys
                RemoveAllPlayers();
                GameManager.RemoveObjectInContainer(mx, my, GameManager.Instance.CoinContainer);
                GameManager.RemoveObjectInContainer(mx, my, GameManager.Instance.KeyContainer);

                // place player
                Vector2 pos = new(mx, my);
                GameObject player = GameManager.Instance.Player;
                GameObject newPlayer = Instantiate(player, pos, Quaternion.identity, GameManager.Instance.PlayerContainer.transform);
                PlayerController controller = newPlayer.GetComponent<PlayerController>();
                controller.SetSpeed(speed);

                GameManager.Instance.PlayerStartPos = pos;
            }
        }
    }

    public static void RemoveAllPlayers()
    {
        foreach(Transform player in GameManager.Instance.PlayerContainer.transform)
        {
            Destroy(player.GetComponent<AppendSlider>().GetSliderObject());
            Destroy(player.gameObject);
        }
        GameManager.Instance.PlayerStartPos = null;
    }
    public static void RemovePlayerAtPos(int mx, int my)
    {
        // remove player only if at pos
        foreach (Transform player in GameManager.Instance.PlayerContainer.transform)
        {
            if(player.position.x == mx && player.position.y == my)
            {
                Destroy(player.GetComponent<AppendSlider>().GetSliderObject());
                Destroy(player.gameObject);
                GameManager.Instance.PlayerStartPos = null;
            }
        }
    }
    public static GameObject GetCurrentPlayer()
    {
        Transform playerContainer = GameManager.Instance.PlayerContainer.transform;
        
        return playerContainer.childCount > 0 ? playerContainer.GetChild(0).gameObject : null;
    }
}
