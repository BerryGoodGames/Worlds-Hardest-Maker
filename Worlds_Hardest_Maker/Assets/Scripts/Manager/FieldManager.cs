using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// class for global functions
// no active activities
public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance { get; private set; }

    public static readonly FieldType[] SolidFields =
    {
        FieldType.WallField,
        FieldType.GrayKeyDoorField,
        FieldType.RedKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.YellowKeyDoorField,
    };

    public static FieldType? GetFieldType(GameObject field)
    {
        if (field == null) return null;

        return field.tag.GetFieldType();
    }

    public static GameObject GetField(Vector2Int position)
    {
        Collider2D[] collidedGameObjects = Physics2D.OverlapCircleAll(position, 0.1f, 3072);

        foreach (Collider2D c in collidedGameObjects)
        {
            if (c.gameObject.IsField()) return c.gameObject;
        }

        return null;
    }

    [PunRPC]
    public bool RemoveField(Vector2Int position, bool updateOutlines = false)
    {
        GameObject field = GetField(position);

        bool fieldDestroyed = false;

        if (field != null)
        {
            DestroyImmediate(field);
            fieldDestroyed = true;
        }

        if (!updateOutlines) return fieldDestroyed;

        // update outlines beside removed field
        foreach (GameObject neighbor in GetNeighbors(position))
        {
            if (neighbor.TryGetComponent(out FieldOutline comp)) comp.UpdateOutline();
        }

        return fieldDestroyed;
    }

    [PunRPC]
    public FieldController SetField(Vector2Int position, FieldType type, int rotation)
    {
        if (GetField(position) is not null && GetFieldType(GetField(position)) == type) return null;

        // remove any field at pos
        RemoveField(position, true);

        // place field according to edit mode
        FieldController field = InstantiateField(position, type, rotation);

        ApplyStartGoalCheckpointFieldColor(field.gameObject, null);

        // remove player if at changed pos
        if (!PlayerManager.StartFields.Contains(type)) PlayerManager.Instance.RemovePlayerAtPosIntersect(position);

        if (CoinManager.CannotPlaceFields.Contains(type))
            // remove coin if wall is placed
            GameManager.RemoveObjectInContainerIntersect(position, ReferenceManager.Instance.CoinContainer);

        if (KeyManager.CannotPlaceFields.Contains(type))
            // remove key if wall is placed
            GameManager.RemoveObjectInContainerIntersect(position, ReferenceManager.Instance.KeyContainer);

        return field;
    }

    [PunRPC]
    public void SetField(Vector2Int position, FieldType type) => SetField(position, type, 0);

    public static void ApplyStartGoalCheckpointFieldColor(GameObject field, bool? oneColor)
    {
        List<Color> colors = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").Colors;
        oneColor ??= GraphicsSettings.Instance.OneColorStartGoalCheckpoint;

        // special case for checkpoint
        SpriteRenderer renderer = field.GetComponent<SpriteRenderer>();
        if (field.CompareTag("CheckpointField"))
        {
            CheckpointController checkpoint = field.GetComponent<CheckpointController>();

            Color checkpointUnactivated = colors[(bool)oneColor ? 4 : 2];
            Color checkpointActivated = colors[(bool)oneColor ? 5 : 3];

            renderer.color = checkpoint.Activated ? checkpointActivated : checkpointUnactivated;

            if (field.TryGetComponent(out Animator anim)) anim.enabled = (bool)oneColor;

            return;
        }

        // // every other case
        string[] tags = { "StartField", "GoalField", };

        for (int i = 0; i < tags.Length; i++)
        {
            if (!field.CompareTag(tags[i])) continue;

            renderer.color = colors[(bool)oneColor ? 4 : i];

            break;
        }
    }

    private static FieldController InstantiateField(Vector2 pos, FieldType type, int rotation)
    {
        GameObject prefab = type.GetPrefab();
        GameObject res = MultiplayerManager.Instance.Multiplayer
            ? PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.Euler(0, 0, rotation))
            : Instantiate(
                prefab, pos, Quaternion.Euler(0, 0, rotation),
                ReferenceManager.Instance.FieldContainer
            );

        return res.GetComponent<FieldController>();
    }

    public static List<GameObject> GetNeighbors(GameObject field)
    {
        Vector2Int position = Vector2Int.RoundToInt(field.transform.position);
        return GetNeighbors(position);
    }

    public static List<GameObject> GetNeighbors(Vector2Int position)
    {
        Vector2Int[] deltas = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, };

        List<GameObject> neighbors = new();

        foreach (Vector2Int d in deltas)
        {
            GameObject neighbor = GetField(position + d);
            if (neighbor != null) neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public static List<GameObject> GetFieldsAtPos(Vector2 position)
    {
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
        };

        checkPoses = checkPoses.Distinct().ToArray();

        List<GameObject> res = new();
        foreach (Vector2Int checkPosition in checkPoses)
        {
            GameObject field = GetField(checkPosition);
            if (field != null) res.Add(field);
        }

        return res;
    }

    #region Field intersection

    public static bool IntersectingAnyFieldsAtPos(Vector2 position, params FieldType[] t)
    {
        List<FieldType> types = t.ToList();

        List<GameObject> intersectingFields = GetFieldsAtPos(position);
        foreach (GameObject field in intersectingFields)
        {
            if (types.Contains((FieldType)GetFieldType(field))) return true;
        }

        return false;
    }

    public static bool IntersectingEveryFieldAtPos(Vector2 position, params FieldType[] t)
    {
        List<FieldType> types = t.ToList();
        List<GameObject> intersectingFields = GetFieldsAtPos(position);
        foreach (GameObject field in intersectingFields)
        {
            if (!types.Contains((FieldType)GetFieldType(field))) return false;
        }

        return true;
    }

    public static bool IsPosCoveredWithFieldType(Vector2 position, params FieldType[] t)
    {
        List<FieldType> types = t.ToList();
        List<GameObject> intersectingFields = GetFieldsAtPos(position);
        if (intersectingFields.Count == 0) return false;

        int expectedCount = IntersectionCountAtPos(position);

        foreach (GameObject field in intersectingFields)
        {
            if (expectedCount != intersectingFields.Count || !types.Contains((FieldType)GetFieldType(field))) return false;
        }

        return true;
    }

    public static int IntersectionCountAtPos(Vector2 position)
    {
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
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