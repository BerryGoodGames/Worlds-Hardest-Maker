using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class FieldManager : MonoBehaviour, IManager<FieldController>
{
    public static FieldManager Instance { get; private set; }

    public Transform DefaultContainer => ReferenceManager.Instance.FieldContainer;

    public FieldController SetInSheet(ManagerParameters args)
    {
        FieldController fieldAtPosition = GetInSheet(args.Position, args.Sheet);
        if (fieldAtPosition is not null && fieldAtPosition.FieldMode == args.FieldMode) return null;

        // remove any field at pos
        Remove(args.Position, true, args.Sheet);

        // place field according to edit mode
        FieldController field = ((IManager<FieldController>)this).Instantiate(args);

        if (field.TryGetComponent(out ColorCalibration calibration))
            calibration.Apply(LevelSessionEditManager.Instance.Playing && SettingsManager.Instance.OneColorSafeFields);

        // remove player if at changed pos
        if (!args.FieldMode.IsStartFieldForPlayer) PlayerManager.Instance.RemoveAtPosIntersect(args.Position);

        if (CoinManager.CannotPlaceFields.Contains(args.FieldMode))
            // remove coin if wall is placed
            GameManager.RemoveObjectInContainerIntersect(args.Position, ReferenceManager.Instance.CoinContainer);

        if (KeyManager.CannotPlaceFields.Contains(args.FieldMode))
            // remove key if wall is placed
            GameManager.RemoveObjectInContainerIntersect(args.Position, ReferenceManager.Instance.KeyContainer);

        return field;
    }

    // public FieldController Set(ManagerParameters args)
    // {
    //     
    // }

    public FieldController GetInSheet(Vector2 position, AnchorController sheet)
    {
        // get all collisions from layers Field and Void
        Collider2D[] collidedGameObjects = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Field)
            .Concat(Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Void)).ToArray();

        foreach (Collider2D c in collidedGameObjects)
        {
            // check if field
            if (!c.TryGetComponent(out FieldController f)) continue;

            if (IManager.IsInSheet(f, sheet)) return f;
        }

        return null;
    }

    public FieldController Get(Vector2 position)
    {
        // get all collisions from layers Field and Void
        Collider2D[] collidedGameObjects = Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Field)
            .Concat(Physics2D.OverlapCircleAll(position, 0.1f, LayerManager.Instance.Layers.Void)).ToArray();

        foreach (Collider2D c in collidedGameObjects)
        {
            if (c.TryGetComponent(out FieldController f)) return f;
        }

        return null;
    }

    public FieldController InstantiateInSheet(ManagerParameters args)
    {
        GameObject prefab = args.FieldMode.Prefab;
        GameObject res = Instantiate(
            prefab, args.Position, Quaternion.Euler(0, 0, args.Rotation),
            args.Sheet == null ? DefaultContainer : args.Sheet.AttachmentContainer
        );

        FieldController fieldController = res.GetComponent<FieldController>();
        fieldController.FieldMode = args.FieldMode;

        PlaceManager.AttachToSheet(res, PlaceManager.GetCurrentSheet());

        return fieldController;
    }

    public bool Remove(Vector2 position, bool updateOutlines = false, [CanBeNull] AnchorController sheet = null)
    {
        FieldController field = GetInSheet(position, sheet);

        bool fieldDestroyed = false;

        if (field != null)
        {
            DestroyImmediate(field.gameObject);
            fieldDestroyed = true;
        }

        if (!updateOutlines) return fieldDestroyed;

        // Update outlines beside removed field
        foreach (FieldController neighbor in GetNeighbors(position))
        {
            if (neighbor.TryGetComponent(out FieldOutline comp)) comp.UpdateOutline();
        }

        return fieldDestroyed;
    }

    public void PlaceField(FieldMode mode, int rotation, bool playSound, Vector2Int matrixPosition)
    {
        if (!mode.IsRotatable) rotation = 0;

        ManagerParameters args = new()
        {
            Position = matrixPosition,
            FieldMode = mode,
            Rotation = rotation,
        };

        if (((IManager<FieldController>)this).Set(args) is not null && playSound) AudioManager.Instance.Play(PlaceManager.Instance.GetSfx(mode));
    }

    public static void ApplySafeFieldsColor(bool oneColor)
    {
        ColorCalibration[] colorCalibrations = ReferenceManager.Instance.FieldContainer.GetComponentsInChildren<ColorCalibration>();

        foreach (ColorCalibration field in colorCalibrations) field.Apply(oneColor);
    }

    public List<FieldController> GetNeighbors(GameObject field)
    {
        Vector2Int position = Vector2Int.RoundToInt(field.transform.position);
        return GetNeighbors(position);
    }

    public List<FieldController> GetNeighbors(Vector2 position)
    {
        Vector2Int[] deltas = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, };

        List<FieldController> neighbors = new();

        foreach (Vector2Int d in deltas)
        {
            FieldController neighbor = Get(position + d);
            if (neighbor != null) neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public List<FieldController> GetNeighborsInSheet(GameObject field, [CanBeNull] AnchorController sheet)
    {
        Vector2Int position = Vector2Int.RoundToInt(field.transform.position);
        return GetNeighborsInSheet(position, sheet);
    }

    public List<FieldController> GetNeighborsInSheet(Vector2Int position, [CanBeNull] AnchorController sheet)
    {
        Vector2Int[] deltas = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, };

        List<FieldController> neighbors = new();

        foreach (Vector2Int d in deltas)
        {
            FieldController neighbor = GetInSheet(position + d, sheet);
            if (neighbor != null) neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public List<FieldController> GetFieldsAtGridPosInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
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
            FieldController field = GetInSheet(checkPosition, sheet);
            if (field != null) res.Add(field);
        }

        return res;
    }

    #region Field intersection

    public bool IntersectingAnyFieldsAtPos(Vector2 position, [CanBeNull] AnchorController sheet, params FieldMode[] t)
    {
        List<FieldMode> modes = t.ToList();

        List<FieldController> intersectingFields = GetFieldsAtGridPosInSheet(position, sheet);
        foreach (FieldController field in intersectingFields)
        {
            if (modes.Contains(field.FieldMode)) return true;
        }

        return false;
    }

    public bool IntersectingEveryFieldAtPos(Vector2 position, [CanBeNull] AnchorController sheet, params FieldMode[] t)
    {
        List<FieldMode> types = t.ToList();
        List<FieldController> intersectingFields = GetFieldsAtGridPosInSheet(position, sheet);
        foreach (FieldController field in intersectingFields)
        {
            if (!types.Contains(field.FieldMode)) return false;
        }
    
        return true;
    }

    public bool IsPosCoveredWithFieldTypeInSheet(Vector2 position, [CanBeNull] AnchorController sheet, params FieldMode[] t)
    {
        List<FieldMode> types = t.ToList();
        List<FieldController> intersectingFields = GetFieldsAtGridPosInSheet(position, sheet);
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

    public bool CorrespondsToEditMode(EditMode compare) => compare.Attributes.IsField;
}