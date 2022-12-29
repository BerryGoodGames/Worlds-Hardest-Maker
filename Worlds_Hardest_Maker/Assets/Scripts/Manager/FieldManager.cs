using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// class for global functions
// no active activities
public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance { get; private set; }

    public static readonly List<FieldType> solidFields = new(new[]
    {
        FieldType.WALL_FIELD,
        FieldType.GRAY_KEY_DOOR_FIELD,
        FieldType.RED_KEY_DOOR_FIELD,
        FieldType.GREEN_KEY_DOOR_FIELD,
        FieldType.BLUE_KEY_DOOR_FIELD,
        FieldType.YELLOW_KEY_DOOR_FIELD
    });

    public static readonly List<FieldType> rotatableFields = new(new[]
    {
        FieldType.ONE_WAY_FIELD,
        FieldType.CONVEYOR
    });

    public static bool IsRotatable(EditMode editMode)
    {
        FieldType? fieldType = (FieldType?)Utils.TryConvertEnum<EditMode, FieldType>(editMode);

        return fieldType != null && rotatableFields.Contains((FieldType)fieldType);
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

        if (field != null) DestroyImmediate(field);

        if (!updateOutlines) return;

        // update outlines beside removed field
        foreach (GameObject neighbor in GetNeighbors(mx, my))
        {
            if (neighbor.TryGetComponent(out FieldOutline comp))
            {
                comp.UpdateOutline();
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
        if (GetField(mx, my) != null && GetFieldType(GetField(mx, my)) == type) return;

        // remove any field at pos
        RemoveField(mx, my, true);

        // place field according to edit mode
        Vector2 pos = new(mx, my);
        GameObject field = InstantiateField(pos, type, rotation);

        ApplyStartGoalCheckpointFieldColor(ref field);

        // remove player if at changed pos
        if (!PlayerManager.startFields.Contains(type))
        {
            PlayerManager.Instance.RemovePlayerAtPosIntersect(mx, my);
        }

        if (CoinManager.cannotPlaceFields.Contains(type))
        {
            // TODO: 9x bad performance than before
            // remove coin if wall is placed
            GameManager.RemoveObjectInContainerIntersect(mx, my, ReferenceManager.Instance.coinContainer);
        }

        if (KeyManager.cannotPlaceFields.Contains(type))
        {
            // TODO: 9x bad performance than before
            // remove key if wall is placed
            GameManager.RemoveObjectInContainerIntersect(mx, my, ReferenceManager.Instance.keyContainer);
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

    private static void ApplyStartGoalCheckpointFieldColor(ref GameObject field)
    {
        // REF
        string[] tags = { "StartField", "GoalField", "CheckpointField" };

        for (int i = 0; i < tags.Length; i++)
        {
            // TODO: similar code to GraphicsSettings.cs SetOneColorStartGoal
            if (!field.CompareTag(tags[i])) continue;

            if (GraphicsSettings.Instance.oneColorStartGoalCheckpoint)
            {
                field.GetComponent<SpriteRenderer>().color =
                    ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors[4];

                if (field.TryGetComponent(out Animator anim))
                {
                    anim.enabled = false;
                }
            }
            else
            {
                // set colorful colors to start, goal, checkpoints fields
                List<Color> colors = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors;

                SpriteRenderer renderer = field.GetComponent<SpriteRenderer>();

                if (!field.CompareTag(tags[i])) continue;

                renderer.color = colors[i];

                if (field.TryGetComponent(out Animator anim))
                {
                    anim.enabled = true;
                }
            }
        }
    }

    private static GameObject InstantiateField(Vector2 pos, FieldType type, int rotation)
    {
        GameObject prefab = type.GetPrefab();
        GameObject res = MultiplayerManager.Instance.Multiplayer
            ? PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.Euler(0, 0, rotation))
            : Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation), ReferenceManager.Instance.fieldContainer);

        return res;
    }

    public static List<GameObject> GetNeighbors(GameObject field)
    {
        Vector2 pos = field.transform.position;
        return GetNeighbors((int)pos.x, (int)pos.y);
    }

    public static List<GameObject> GetNeighbors(int mx, int my)
    {
        List<GameObject> neighbors = new();
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        for (int d = 0; d < dx.Length; d++)
        {
            GameObject neighbor = GetField(dx[d] + mx, dy[d] + my);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
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
        foreach ((int x, int y) in checkPoses)
        {
            GameObject field = GetField(x, y);
            if (field != null) res.Add(field);
        }

        return res;
    }

    public static void FillPathWithFields(FieldType type, int rotation)
    {
        // generalized Bresenham's Line Algorithm optimized without /, find (unoptimized) algorithm here: https://www.uobabylon.edu.iq/eprints/publication_2_22893_6215.pdf
        // I tried my best to explain the variables, but I have no idea how it works

        Vector2 a = MouseManager.Instance.MouseWorldPos;
        Vector2 b = MouseManager.Instance.PrevMouseWorldPos;

        // increment and delta x
        float incX = Mathf.Sign(b.x - a.x);
        float dX = Mathf.Abs(b.x - a.x);

        // increment and delta y
        float incY = Mathf.Sign(b.y - a.y);
        float dY = Mathf.Abs(b.y - a.y);
        
        float cmpt = Mathf.Max(dX, dY); // max of both numbers
        float incD = -2 * Mathf.Abs(dX - dY); // increment of delta
        float incS = 2 * Mathf.Min(dX, dY); // I have no idea

        float error = incD + cmpt; // error of line
        float x = a.x; // where we are x
        float y = a.y; // where we are y

        while (cmpt >= 0)
        {
            Instance.SetField((int)x, (int)y, type, rotation);
            cmpt -= 1;

            if (error >= 0 || dX > dY) x += incX;
            if (error >= 0 || dX <= dY) y += incY;
            if (error >= 0) error += incD;
            else error += incS;
        }
    }

    #region Field intersection

    public static bool IntersectingAnyFieldsAtPos(float mx, float my, params FieldType[] t)
    {
        List<FieldType> types = t.ToList();

        List<GameObject> intersectingFields = GetFieldsAtPos(mx, my);
        foreach (GameObject field in intersectingFields)
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
            if (expectedCount != intersectingFields.Count || !types.Contains((FieldType)GetFieldType(field)))
                return false;
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