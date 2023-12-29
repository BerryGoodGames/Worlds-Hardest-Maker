using System.Collections.Generic;
using System.Linq;
using MyBox;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// class for global functions
// no active activities
public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance { get; private set; }
    
    [Foldout("Field Scriptable Objects")] public FieldObject Wall;
    [Foldout("Field Scriptable Objects")] public FieldObject Start;
    [Foldout("Field Scriptable Objects")] public FieldObject Goal;
    [Foldout("Field Scriptable Objects")] public FieldObject Checkpoint;
    [Foldout("Field Scriptable Objects")] public FieldObject OneWay;
    [Foldout("Field Scriptable Objects")] public FieldObject Conveyor;
    [Foldout("Field Scriptable Objects")] public FieldObject Water;
    [Foldout("Field Scriptable Objects")] public FieldObject Ice;
    [Foldout("Field Scriptable Objects")] public FieldObject Void;
    [Foldout("Field Scriptable Objects")] public FieldObject GrayKeyDoor;
    [Foldout("Field Scriptable Objects")] public FieldObject RedKeyDoor;
    [Foldout("Field Scriptable Objects")] public FieldObject GreenKeyDoor;
    [Foldout("Field Scriptable Objects")] public FieldObject BlueKeyDoor;
    [Foldout("Field Scriptable Objects")] public FieldObject YellowKeyDoor;

    public static FieldController GetField(Vector2Int position)
    {
        Collider2D[] collidedGameObjects = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Field);

        foreach (Collider2D c in collidedGameObjects)
        {
            if (c.TryGetComponent(out FieldController f)) return f;
        }

        return null;
    }

    [PunRPC]
    public bool RemoveField(Vector2Int position, bool updateOutlines = false)
    {
        FieldController field = GetField(position);

        bool fieldDestroyed = false;

        if (field != null)
        {
            DestroyImmediate(field);
            fieldDestroyed = true;
        }

        if (!updateOutlines) return fieldDestroyed;

        // update outlines beside removed field
        foreach (FieldController neighbor in GetNeighbors(position))
        {
            if (neighbor.TryGetComponent(out FieldOutline comp)) comp.UpdateOutline();
        }

        return fieldDestroyed;
    }

    [PunRPC]
    public FieldController SetField(Vector2Int position, FieldMode mode, int rotation)
    {
        FieldController fieldAtPosition = GetField(position);
        if (fieldAtPosition is not null && fieldAtPosition.FieldMode == mode) return null;
        
        // remove any field at pos
        RemoveField(position, true);

        // place field according to edit mode
        FieldController field = InstantiateField(position, mode, rotation);

        ApplyStartGoalCheckpointFieldColor(field.gameObject, null);

        // remove player if at changed pos
        if (!mode.IsStartFieldForPlayer) PlayerManager.Instance.RemovePlayerAtPosIntersect(position);

        if (CoinManager.CannotPlaceFields.Contains(mode))
            // remove coin if wall is placed
            GameManager.RemoveObjectInContainerIntersect(position, ReferenceManager.Instance.CoinContainer);

        if (KeyManager.CannotPlaceFields.Contains(mode))
            // remove key if wall is placed
            GameManager.RemoveObjectInContainerIntersect(position, ReferenceManager.Instance.KeyContainer);

        return field;
    }

    [PunRPC]
    public void SetField(Vector2Int position, FieldMode mode) => SetField(position, mode, 0);
    
    public void PlaceField(FieldMode mode, int rotation, bool playSound, Vector2Int matrixPosition)
    {
        // TODO: PlaceField vs. SetField??
        
        if (!mode.IsRotatable) rotation = 0;
        
        if (SetField(matrixPosition, mode, rotation) is not null && playSound)
        {
            AudioManager.Instance.Play(PlaceManager.Instance.GetSfx(mode));
        }
    }

    public static void ApplyStartGoalCheckpointFieldColor(GameObject field, bool? oneColor)
    {
        List<Color> colors = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").Colors;
        oneColor ??= GraphicsSettings.Instance.OneColorStartGoalCheckpoint;

        // special case for checkpoint
        SpriteRenderer renderer = field.GetComponent<SpriteRenderer>();
        if (field.CompareTag("Checkpoint"))
        {
            CheckpointController checkpoint = field.GetComponent<CheckpointController>();

            Color checkpointUnactivated = colors[(bool)oneColor ? 4 : 2];
            Color checkpointActivated = colors[(bool)oneColor ? 5 : 3];

            renderer.color = checkpoint.Activated ? checkpointActivated : checkpointUnactivated;

            if (field.TryGetComponent(out Animator anim)) anim.enabled = (bool)oneColor;

            return;
        }

        // // every other case
        string[] tags = { "Start", "Goal", };

        for (int i = 0; i < tags.Length; i++)
        {
            if (!field.CompareTag(tags[i])) continue;

            renderer.color = colors[(bool)oneColor ? 4 : i];

            break;
        }
    }

    private static FieldController InstantiateField(Vector2 pos, FieldMode mode, int rotation)
    {
        GameObject prefab = mode.Prefab;
        GameObject res = MultiplayerManager.Instance.Multiplayer
            ? PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.Euler(0, 0, rotation))
            : Instantiate(
                prefab, pos, Quaternion.Euler(0, 0, rotation),
                ReferenceManager.Instance.FieldContainer
            );

        return res.GetComponent<FieldController>();
    }

    public static List<FieldController> GetNeighbors(GameObject field)
    {
        Vector2Int position = Vector2Int.RoundToInt(field.transform.position);
        return GetNeighbors(position);
    }

    public static List<FieldController> GetNeighbors(Vector2Int position)
    {
        Vector2Int[] deltas = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, };

        List<FieldController> neighbors = new();

        foreach (Vector2Int d in deltas)
        {
            FieldController neighbor = GetField(position + d);
            if (neighbor != null) neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public static List<FieldController> GetFieldsAtPos(Vector2 position)
    {
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
        };

        checkPoses = checkPoses.Distinct().ToArray();

        List<FieldController> res = new();
        foreach (Vector2Int checkPosition in checkPoses)
        {
            FieldController field = GetField(checkPosition);
            if (field != null) res.Add(field);
        }

        return res;
    }

    #region Field intersection

    public static bool IntersectingAnyFieldsAtPos(Vector2 position, params FieldMode[] t)
    {
        List<FieldMode> modes = t.ToList();

        List<FieldController> intersectingFields = GetFieldsAtPos(position);
        foreach (FieldController field in intersectingFields)
        {
            if (modes.Contains(field.FieldMode)) return true;
        }

        return false;
    }

    public static bool IntersectingEveryFieldAtPos(Vector2 position, params FieldMode[] t)
    {
        List<FieldMode> types = t.ToList();
        List<FieldController> intersectingFields = GetFieldsAtPos(position);
        foreach (FieldController field in intersectingFields)
        {
            if (!types.Contains(field.FieldMode)) return false;
        }

        return true;
    }

    public static bool IsPosCoveredWithFieldType(Vector2 position, params FieldMode[] t)
    {
        List<FieldMode> types = t.ToList();
        List<FieldController> intersectingFields = GetFieldsAtPos(position);
        if (intersectingFields.Count == 0) return false;

        int expectedCount = IntersectionCountAtPos(position);

        foreach (FieldController field in intersectingFields)
        {
            if (expectedCount != intersectingFields.Count || !types.Contains(field.FieldMode)) return false;
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