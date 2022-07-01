using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

// class for global functions
// no active activities
public class FieldManager : MonoBehaviour
{
    public enum FieldType
    {
        WALL_FIELD, START_FIELD, GOAL_FIELD, START_AND_GOAL_FIELD, CHECKPOINT_FIELD, ONE_WAY_FIELD, GRAY_KEY_DOOR_FIELD, RED_KEY_DOOR_FIELD, GREEN_KEY_DOOR_FIELD, BLUE_KEY_DOOR_FIELD, YELLOW_KEY_DOOR_FIELD
    }
    public enum KeyDoorColor
    {
        GRAY, RED, GREEN, BLUE, YELLOW
    }

    public static bool IsEditModeFieldType(GameManager.EditMode mode)
    {
        // list of all field types
        List<GameManager.EditMode> fieldModes = new()
        {
            GameManager.EditMode.WALL_FIELD,
            GameManager.EditMode.START_FIELD,
            GameManager.EditMode.GOAL_FIELD,
            GameManager.EditMode.START_AND_GOAL_FIELD,
            GameManager.EditMode.CHECKPOINT_FIELD,
            GameManager.EditMode.ONE_WAY_FIELD,
            GameManager.EditMode.GRAY_KEY_DOOR_FIELD,
            GameManager.EditMode.RED_KEY_DOOR_FIELD,
            GameManager.EditMode.GREEN_KEY_DOOR_FIELD,
            GameManager.EditMode.BLUE_KEY_DOOR_FIELD,
            GameManager.EditMode.YELLOW_KEY_DOOR_FIELD
        };

        return fieldModes.Contains(mode);
    }

    public static bool IsField(GameObject field)
    {
        return GetFieldTypeByTag(field.tag) != (FieldType)(-1);
    }

    public static GameObject GetPrefabByType(FieldType type)
    {
        // return prefab according to type
        return new GameObject[] {
            GameManager.Instance.WallField,
            GameManager.Instance.StartField,
            GameManager.Instance.GoalField,
            GameManager.Instance.StartAndGoalField,
            GameManager.Instance.CheckpointField,
            GameManager.Instance.OneWayField,
            GameManager.Instance.GrayKeyDoorField,
            GameManager.Instance.RedKeyDoorField,
            GameManager.Instance.GreenKeyDoorField,
            GameManager.Instance.BlueKeyDoorField,
            GameManager.Instance.YellowKeyDoorField,
        }[(int)type];
    }

    public static FieldType GetFieldTypeByTag(string tag)
    {
        List<string> tags = new()
        {
            "WallField",
            "StartField",
            "GoalField",
            "StartAndGoalField",
            "CheckpointField",
            "OneWayField",
            "KeyDoorField",
            "RedKeyDoorField",
            "GreenKeyDoorField",
            "BlueKeyDoorField",
            "YellowKeyDoorField"
        };

        return (FieldType)tags.IndexOf(tag);
    }

    public static FieldType? GetFieldType(GameObject field)
    {
        if (field == null)
        {
            return null;
        }
        return GetFieldTypeByTag(field.tag);
    }

    public static GameObject GetField(int mx, int my)
    {
        Collider2D[] collidedGameObjects = Physics2D.OverlapCircleAll(new(mx, my), 0.1f);
        foreach (Collider2D c in collidedGameObjects)
        {
            if (IsField(c.gameObject))
            {
                return c.gameObject;
            }
        }
        return null;
    }
    public static GameObject GetField(Vector2 pos)
    {
        return GetField((int)pos.x, (int)pos.y);
    }

    public static void RemoveField(int mx, int my, bool updateOutlines = false)
    {
        GameObject field = GetField(mx, my);

        if(field != null) DestroyImmediate(field);

        if (updateOutlines)
        {
            // update outlines beside removed field
            foreach (GameObject neighbour in GetNeighbours(mx, my))
            {
                if (neighbour.TryGetComponent(out FieldOutline comp))
                {
                    comp.UpdateOutline();
                }
            }
        }
    }

    public static void SetField(int mx, int my, FieldType type, int rotation = 0)
    {
        if (GetField(mx, my) == null || GetFieldType(GetField(mx, my)) != type)
        {
            // remove any field at pos
            RemoveField(mx, my, true);

            // place field according to edit mode
            Vector2 pos = new(mx, my);
            GameObject prefab = GetPrefabByType(type);
            Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation), GameManager.Instance.FieldContainer.transform);

            // remove player if at changed pos
            if (type != FieldType.START_FIELD && type != FieldType.START_AND_GOAL_FIELD)
            {
                PlayerManager.RemovePlayerAtPos(mx, my);
            }

            if (CoinManager.CantPlaceFields.Contains(type))
            {
                // remove coin if wall is placed
                GameManager.RemoveObjectInContainer(mx, my, GameManager.Instance.CoinContainer);
            }

            if (KeyManager.CantPlaceFields.Contains(type))
            {
                // remove key if wall is placed
                GameManager.RemoveObjectInContainer(mx, my, GameManager.Instance.KeyContainer);
            }
        }
    }

    public static List<GameObject> GetNeighbours(GameObject field)
    {
        return GetNeighbours((int)field.transform.position.x, (int)field.transform.position.y);
    }
    public static List<GameObject> GetNeighbours(int mx, int my)
    {
        List<GameObject> neighbours = new();
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        for (int d = 0; d < dx.Length; d++)
        {
            GameObject neighbour = GetField(dx[d] + mx, dy[d] + my);
            if (neighbour != null)
            {
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }

}
