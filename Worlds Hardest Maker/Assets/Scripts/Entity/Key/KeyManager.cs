using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static readonly List<GameManager.EditMode> KeyModes = new(new GameManager.EditMode[]
    {
        GameManager.EditMode.GRAY_KEY,
        GameManager.EditMode.RED_KEY,
        GameManager.EditMode.BLUE_KEY,
        GameManager.EditMode.GREEN_KEY,
        GameManager.EditMode.YELLOW_KEY
    });
    public static readonly List<GameManager.EditMode> KeyDoorModes = new(new GameManager.EditMode[]
    {
        GameManager.EditMode.GRAY_KEY_DOOR_FIELD,
        GameManager.EditMode.RED_KEY_DOOR_FIELD,
        GameManager.EditMode.BLUE_KEY_DOOR_FIELD,
        GameManager.EditMode.GREEN_KEY_DOOR_FIELD,
        GameManager.EditMode.YELLOW_KEY_DOOR_FIELD
    });
    public static readonly List<FieldManager.FieldType> KeyDoorTypes = new(new FieldManager.FieldType[]
    {
        FieldManager.FieldType.GRAY_KEY_DOOR_FIELD,
        FieldManager.FieldType.RED_KEY_DOOR_FIELD,
        FieldManager.FieldType.BLUE_KEY_DOOR_FIELD,
        FieldManager.FieldType.GREEN_KEY_DOOR_FIELD,
        FieldManager.FieldType.YELLOW_KEY_DOOR_FIELD
    });

    public static List<FieldManager.FieldType> CantPlaceFields = new(new FieldManager.FieldType[]{
        FieldManager.FieldType.WALL_FIELD,
        FieldManager.FieldType.GRAY_KEY_DOOR_FIELD,
        FieldManager.FieldType.RED_KEY_DOOR_FIELD,
        FieldManager.FieldType.BLUE_KEY_DOOR_FIELD,
        FieldManager.FieldType.GREEN_KEY_DOOR_FIELD,
        FieldManager.FieldType.YELLOW_KEY_DOOR_FIELD
    });

    public static void SetKey(int mx, int my, FieldManager.KeyDoorColor color)
    {
        Vector2 pos = new(mx, my);
        // place key if no key is there, the field is a canplacefield or default, the player is not there
        if(!IsKeyThere(mx, my, color))
        {
            GameObject currentField = FieldManager.GetField(mx, my);
            FieldManager.FieldType? type = FieldManager.GetFieldType(currentField);
            
            if(type == null || !CantPlaceFields.Contains((FieldManager.FieldType)type))
            {
                GameObject player = PlayerManager.GetCurrentPlayer();
                if (player == null || (Vector2)player.transform.position != pos)
                {
                    RemoveKey(mx, my);
                    if(color == FieldManager.KeyDoorColor.GRAY) {
                        Instantiate(GameManager.Instance.GrayKey, pos, Quaternion.identity, GameManager.Instance.KeyContainer.transform);
                    }
                    else if (color == FieldManager.KeyDoorColor.RED) {
                        Instantiate(GameManager.Instance.RedKey, pos, Quaternion.identity, GameManager.Instance.KeyContainer.transform);
                    }
                    else if (color == FieldManager.KeyDoorColor.GREEN)
                    {
                        Instantiate(GameManager.Instance.GreenKey, pos, Quaternion.identity, GameManager.Instance.KeyContainer.transform);
                    }
                    else if (color == FieldManager.KeyDoorColor.BLUE)
                    {
                        Instantiate(GameManager.Instance.BlueKey, pos, Quaternion.identity, GameManager.Instance.KeyContainer.transform);
                    }
                    else if (color == FieldManager.KeyDoorColor.YELLOW)
                    {
                        Instantiate(GameManager.Instance.YellowKey, pos, Quaternion.identity, GameManager.Instance.KeyContainer.transform);
                    }
                }
            }
        }
    }
    public static void RemoveKey(int mx, int my)
    {
        GameObject container = GameManager.Instance.KeyContainer;

        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.25f);

        foreach(Collider2D hit in hits)
        {
            Transform parent = hit.transform.parent;
            if(parent.parent == container.transform)
            {
                Destroy(parent.gameObject);
            }
        }
    }
    public static GameObject GetKey(int mx, int my)
    {
        GameObject container = GameManager.Instance.KeyContainer;
        foreach (Transform key in container.transform)
        {
            if ((Vector2)key.position == new Vector2(mx, my))
            {
                return key.gameObject;
            }
        }
        return null;
    }
    public static bool IsKeyThere(int mx, int my, FieldManager.KeyDoorColor color)
    {
        GameObject key = GetKey(mx, my);
        return key != null && key.transform.GetChild(0).GetComponent<KeyController>().color == color;
    }
}
