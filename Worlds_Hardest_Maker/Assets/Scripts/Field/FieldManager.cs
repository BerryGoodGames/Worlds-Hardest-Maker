using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

// class for global functions
// no active activities
public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance { get; private set; }

    public enum FieldType
    {
        WALL_FIELD, START_FIELD, GOAL_FIELD, START_AND_GOAL_FIELD, CHECKPOINT_FIELD, ONE_WAY_FIELD, GRAY_KEY_DOOR_FIELD, RED_KEY_DOOR_FIELD, GREEN_KEY_DOOR_FIELD, BLUE_KEY_DOOR_FIELD, YELLOW_KEY_DOOR_FIELD
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

    [PunRPC]
    public void RemoveField(int mx, int my, bool updateOutlines)
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
    [PunRPC]
    public void RemoveField(int mx, int my)
    {
        RemoveField(mx, my, false);
    }

    [PunRPC]
    public void SetField(int mx, int my, FieldType type, int rotation)
    {
        if (GetField(mx, my) == null || GetFieldType(GetField(mx, my)) != type)
        {
            // remove any field at pos
            RemoveField(mx, my, true);

            // place field according to edit mode
            Vector2 pos = new(mx, my);
            GameObject field = InstantiateField(pos, type, rotation);

            // REF
            string[] tags = { "StartField", "GoalField", "StartAndGoalField", "CheckpointField" };
            
            for(int i = 0; i < tags.Length; i++)
            {
                // TODO: similar code to GraphicsSettings.cs SetOneColorStartGoal
                if (field.CompareTag(tags[i]))
                {
                    if (GraphicsSettings.Instance.oneColorStartGoal)
                    {
                        field.GetComponent<SpriteRenderer>().color = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors[5];

                        if (field.TryGetComponent(out Animator anim))
                        {
                            anim.enabled = false;
                        }
                    }
                    else
                    {
                        // set colorful colors to start, goal, checkpoints and startgoal fields
                        List<Color> colors = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors;

                        SpriteRenderer renderer = field.GetComponent<SpriteRenderer>();
                        if (field.CompareTag(tags[i]))
                        {
                            renderer.color = colors[i];

                            if (field.TryGetComponent(out Animator anim))
                            {
                                anim.enabled = true;
                            }
                        }
                    }
                }
            }

            // remove player if at changed pos
            if (type != FieldType.START_FIELD && type != FieldType.START_AND_GOAL_FIELD)
            {
                PlayerManager.Instance.RemovePlayerAtPosIntersect(mx, my);
            }

            if (CoinManager.CantPlaceFields.Contains(type))
            {
                // TODO: 9x bad performance than before
                // remove coin if wall is placed
                GameManager.RemoveObjectInContainerIntersect(mx, my, GameManager.Instance.CoinContainer);
            }

            if (KeyManager.CantPlaceFields.Contains(type))
            {
                // TODO: 9x bad performance than before
                // remove key if wall is placed
                GameManager.RemoveObjectInContainerIntersect(mx, my, GameManager.Instance.KeyContainer);
            }
        }
    }
    [PunRPC]
    public void SetField(int mx, int my, FieldType type)
    {
        SetField(mx, my, type, 0);
    }
    private static GameObject InstantiateField(Vector2 pos, FieldType type, int rotation)
    {
        GameObject res;
        GameObject prefab = GetPrefabByType(type);
        if (GameManager.Instance.Multiplayer)
        {
            res = PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.Euler(0, 0, rotation));
        } else
        {
            res = Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation), GameManager.Instance.FieldContainer.transform);
        }
        return res;
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

    public static List<GameObject> GetFieldsAtPos(float mx, float my)
    {
        (int x, int y)[] checkPoses =
        {
            (Mathf.FloorToInt(mx), Mathf.FloorToInt(my)),
            (Mathf.CeilToInt(mx), Mathf.FloorToInt(my)),
            (Mathf.FloorToInt(mx), Mathf.CeilToInt(my)),
            (Mathf.CeilToInt(mx), Mathf.CeilToInt(my))
        };

        checkPoses = checkPoses.Distinct().ToArray();

        List<GameObject> res = new();
        foreach (var (x, y) in checkPoses)
        {
            GameObject field = GetField(x, y);
            if(field != null) res.Add(field);
        }

        return res;
    }

    #region FIELD INTERSECTION
    public static bool IntersectingAnyFieldsAtPos(float mx, float my, params FieldType[] t)
    {
        List<FieldType> types = t.ToList();

        List<GameObject> intersectingFields = GetFieldsAtPos(mx, my);
        foreach(GameObject field in intersectingFields)
        {
            if (types.Contains((FieldType)GetFieldType(field))) return true;
        }
        return false;
    }

    public static bool IntersectingEveryFieldAtPos(float mx, float my, params FieldType[] t)
    {
        List<FieldType> types = t.ToList();
        List<GameObject> intersectingFields = GetFieldsAtPos(mx, my);
        foreach (GameObject field in intersectingFields)
        {
            if (!types.Contains((FieldType)GetFieldType(field))) return false;
        }
        return true;
    }

    public static bool IsPosCoveredWithFieldType(float mx, float my, params FieldType[] t)
    {
        List<FieldType> types = t.ToList();
        List<GameObject> intersectingFields = GetFieldsAtPos(mx, my);
        if (intersectingFields.Count == 0) return false;

        int expectedCount = IntersectionCountAtPos(mx, my);
        
        foreach (GameObject field in intersectingFields)
        {
            if (expectedCount != intersectingFields.Count || !types.Contains((FieldType)GetFieldType(field))) return false;
        }
        return true;
    }

    public static int IntersectionCountAtPos(float mx, float my)
    {
        (int x, int y)[] checkPoses =
        {
            (Mathf.FloorToInt(mx), Mathf.FloorToInt(my)),
            (Mathf.CeilToInt(mx), Mathf.FloorToInt(my)),
            (Mathf.FloorToInt(mx), Mathf.CeilToInt(my)),
            (Mathf.CeilToInt(mx), Mathf.CeilToInt(my))
        };

        return checkPoses.Distinct().ToArray().Length;
    }
    #endregion

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
