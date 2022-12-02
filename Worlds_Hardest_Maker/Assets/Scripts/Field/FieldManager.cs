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
        WALL_FIELD, 
        START_FIELD, GOAL_FIELD, CHECKPOINT_FIELD, 
        ONE_WAY_FIELD, CONVEYOR,
        WATER, ICE,
        VOID,
        GRAY_KEY_DOOR_FIELD, RED_KEY_DOOR_FIELD, GREEN_KEY_DOOR_FIELD, BLUE_KEY_DOOR_FIELD, YELLOW_KEY_DOOR_FIELD
    }

    public static readonly List<FieldType> SolidFields = new(new FieldType[]
    {
        FieldType.WALL_FIELD,
        FieldType.ONE_WAY_FIELD,
        FieldType.GRAY_KEY_DOOR_FIELD,
        FieldType.RED_KEY_DOOR_FIELD,
        FieldType.GREEN_KEY_DOOR_FIELD,
        FieldType.BLUE_KEY_DOOR_FIELD,
        FieldType.YELLOW_KEY_DOOR_FIELD
    });

    public static readonly List<FieldType> RotatableFields = new(new FieldType[]
    {
        FieldType.ONE_WAY_FIELD,
        FieldType.CONVEYOR,
    });

    public static bool IsRotatable(GameManager.EditMode editMode)
    {
        FieldType? fieldType = (FieldType?)GameManager.TryConvertEnum<GameManager.EditMode, FieldType>(editMode);

        if (fieldType != null)
            return RotatableFields.Contains((FieldType)fieldType);
        else return false;
    }

    public static FieldType? GetFieldType(GameObject field)
    {
        if (field == null)
        {
            return null;
        }
        return field.tag.GetFieldType();
    }

    public static GameObject GetField(int mx, int my)
    {
        Collider2D[] collidedGameObjects = Physics2D.OverlapCircleAll(new(mx, my), 0.1f, 3072);
        foreach (Collider2D c in collidedGameObjects)
        {
            if (c.gameObject.IsField())
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
            string[] tags = { "StartField", "GoalField", "CheckpointField" };
            
            for(int i = 0; i < tags.Length; i++)
            {
                // TODO: similar code to GraphicsSettings.cs SetOneColorStartGoal
                if (field.CompareTag(tags[i]))
                {
                    if (GraphicsSettings.Instance.oneColorStartGoal)
                    {
                        field.GetComponent<SpriteRenderer>().color = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors[4];

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
            if (!PlayerManager.StartFields.Contains(type))
            {
                PlayerManager.Instance.RemovePlayerAtPosIntersect(mx, my);
            }

            if (CoinManager.CantPlaceFields.Contains(type))
            {
                // TODO: 9x bad performance than before
                // remove coin if wall is placed
                GameManager.RemoveObjectInContainerIntersect(mx, my, ReferenceManager.Instance.CoinContainer);
            }

            if (KeyManager.CantPlaceFields.Contains(type))
            {
                // TODO: 9x bad performance than before
                // remove key if wall is placed
                GameManager.RemoveObjectInContainerIntersect(mx, my, ReferenceManager.Instance.KeyContainer);
            }
        }
    }

    public void SetField(Vector2 pos, FieldType type, int rotation)
    {
        SetField((int)pos.x, (int)pos.y, type, rotation);
    }
    [PunRPC]
    public void SetField(int mx, int my, FieldType type)
    {
        SetField(mx, my, type, 0);
    }
    private static GameObject InstantiateField(Vector2 pos, FieldType type, int rotation)
    {
        GameObject res;
        GameObject prefab = type.GetPrefab();
        if (GameManager.Instance.Multiplayer)
        {
            res = PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.Euler(0, 0, rotation));
        } else
        {
            res = Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation), ReferenceManager.Instance.FieldContainer);
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

    #region Field intersection
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
