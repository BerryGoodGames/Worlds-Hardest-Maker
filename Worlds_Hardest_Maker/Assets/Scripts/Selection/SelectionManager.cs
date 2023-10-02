using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     Methods for filling: GetFillRange, FillArea, GetBounds, GetBoundsMatrix
///     <para>Attach to game manager</para>
/// </summary>
public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionOptions;
    [SerializeField] private MouseOverUIRect fillMouseOver;
    private RectTransform selectionOptionsRect;

    private GameObject selectionOutline;
    private LineAnimator selectionOutlineAnim;
    public bool Selecting { get; set; }

    public List<Vector2> CurrentSelectionRange
    {
        get => SelectionStart == null || SelectionEnd == null ? null : GetCurrentFillRange();
        set
        {
            if (value != null) return;
            SelectionStart = null;
            SelectionEnd = null;
        }
    }

    public static SelectionManager Instance { get; private set; }

    public static readonly List<EditMode> NoFillPreviewModes = new(new[]
    {
        EditMode.GrayKey,
        EditMode.RedKey,
        EditMode.BlueKey,
        EditMode.GreenKey,
        EditMode.YellowKey,
        EditMode.Player
    });

    private Vector2 prevStart;
    private Vector2 prevEnd;
    public static Vector2? SelectionStart;
    public static Vector2? SelectionEnd;


    private void Update()
    {
        if (Input.GetMouseButton(KeybindManager.Instance.SelectionMouseButton) && !EditModeManager.Instance.Playing &&
            !EventSystem.current.IsPointerOverGameObject())
            Selecting = true;

        // update selection markings
        if (!EditModeManager.Instance.Playing && MouseManager.Instance.MouseDragStart != null &&
            MouseManager.Instance.MouseDragCurrent != null && Selecting)
        {
            // get drag positions and world position mode
            WorldPositionType worldPositionType =
                EditModeManager.Instance.CurrentEditMode.GetWorldPosition();

            (Vector2 start, Vector2 end) = MouseManager.GetDragPositions(worldPositionType);

            // disable normal placement preview
            ReferenceManager.Instance.PlacementPreview.gameObject.SetActive(false);

            if (Input.GetMouseButtonDown(KeybindManager.Instance.SelectionMouseButton)) OnStartSelect(start);
            else if (Input.GetMouseButtonUp(KeybindManager.Instance.SelectionMouseButton)) OnAreaSelected(start, end);

            if (!prevStart.Equals(start) || !prevEnd.Equals(end)) OnAreaSelectionChanged(start, end);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelSelection();
    }

    private void LateUpdate()
    {
        if (MouseManager.Instance.MouseDragStart == null || MouseManager.Instance.MouseDragCurrent == null) return;

        prevStart = ((Vector2)MouseManager.Instance.MouseDragStart).ConvertToGrid();
        prevEnd = ((Vector2)MouseManager.Instance.MouseDragCurrent).ConvertToGrid();
    }

    private void Start()
    {
        selectionOptionsRect = selectionOptions.GetComponent<RectTransform>();
        EditModeManager.Instance.OnPlay += CancelSelection;
        EditModeManager.Instance.OnEditModeChange += RemakePreview;

        fillMouseOver.OnHovered += SetPreviewVisible;
        fillMouseOver.OnUnhovered += SetPreviewInvisible;
    }

    #region Callbacks

    private void OnAreaSelectionChanged(Vector2 start, Vector2 end) =>
        // called when area selection changed (lol)
        // set selection outline (if u didn't already see)
        AnimSelectionOutline(start, end);

    private void OnAreaSelected(Vector2 start, Vector2 end)
    {
        // called when mouse button was released and area was selected
        selectionOptions.SetActive(true);
        float width = end.x - start.x;
        float height = end.y - start.y;

        Vector2 position = new(width < 0 ? end.x - 0.5f : end.x + 0.5f, height < 0 ? end.y - 0.5f : end.y + 0.5f);
        UIAttachToPoint posController = selectionOptions.GetComponent<UIAttachToPoint>();

        posController.Point = position;

        selectionOptionsRect.pivot = new(width > 0 ? 0 : 1, height > 0 ? 0 : 1);

        RemakePreview();
    }

    private void OnStartSelect(Vector2 start)
    {
        // called when mouse button was pressed and user starts selecting
        InitSelectionOutline(start);

        selectionOptions.SetActive(false);

        MenuManager.Instance.BlockMenu = true;
    }

    #endregion

    #region Get bounds

    // get bounds of multiple points (in matrix)
    private static (Vector2 lowest, Vector2 highest) GetBounds(List<Vector2> points)
    {
        Vector2 lowest = Vector2.one * points[0].x;
        Vector2 highest = Vector2.one * points[0].y;

        foreach (Vector2 pos in points)
        {
            if (lowest.x > pos.x) lowest.x = pos.x;
            if (lowest.y > pos.y) lowest.y = pos.y;
            if (highest.x < pos.x) highest.x = pos.x;
            if (highest.y < pos.y) highest.y = pos.y;
        }

        return (lowest, highest);
    }

    public static (Vector2 lowest, Vector2 highest) GetBounds(params Vector2[] points) => GetBounds(points.ToList());

    public static (Vector2Int lowest, Vector2Int highest) GetBoundsMatrix(List<Vector2> points)
    {
        (Vector2 lowest, Vector2 highest) = GetBounds(points);
        return (Vector2Int.CeilToInt(lowest), Vector2Int.FloorToInt(highest));
    }

    private static (Vector2Int lowest, Vector2Int highest) GetBoundsMatrix(params Vector2[] points) => GetBoundsMatrix(points.ToList());

    #endregion

    #region Preview

    private static void RemakePreview()
    {
        if (ReferenceManager.Instance.FillPreviewContainer.childCount == 0) return;
        DestroyPreview();
        InitSelectedPreview();
    }

    private static void InitPreview(List<Vector2> range)
    {
        // set new previews, only if edit mode not in NoFillPreviewModes
        if (NoFillPreviewModes.Contains(EditModeManager.Instance.CurrentEditMode)) return;

        foreach (Vector2 pos in range)
        {
            GameObject preview = Instantiate(PrefabManager.Instance.FillPreview, pos, Quaternion.identity,
                ReferenceManager.Instance.FillPreviewContainer);

            PreviewController c = preview.GetComponent<PreviewController>();
            c.Awake_();
            c.UpdateSprite();
            c.UpdateRotation(smooth: false);
        }
    }

    private static void DestroyPreview()
    {
        // destroy selection previews
        foreach (Transform preview in ReferenceManager.Instance.FillPreviewContainer)
        {
            Destroy(preview.gameObject);
        }
    }

    private static void InitSelectedPreview() => InitPreview(GetCurrentFillRange());

    public static void UpdatePreviewRotation()
    {
        foreach (Transform preview in ReferenceManager.Instance.FillPreviewContainer)
        {
            preview.GetComponent<PreviewController>().UpdateRotation();
        }
    }

    public static void UpdatePreviewSprite()
    {
        foreach (Transform preview in ReferenceManager.Instance.FillPreviewContainer)
        {
            preview.GetComponent<PreviewController>().UpdateSprite();
        }
    }

    private static void SetPreviewVisible()
    {
        if (ReferenceManager.Instance.FillPreviewContainer.childCount == 0) InitSelectedPreview();

        ReferenceManager.Instance.FillPreviewContainer.gameObject.SetActive(true);
    }

    private static void SetPreviewInvisible() =>
        ReferenceManager.Instance.FillPreviewContainer.gameObject.SetActive(false);

    #endregion

    #region Fill

    public static List<Vector2> GetFillRange(Vector2 p1, Vector2 p2, WorldPositionType worldPositionType)
    {
        bool inMatrix = worldPositionType is WorldPositionType.Matrix;

        // find bounds
        (Vector2 lowest, Vector2 highest) =
            inMatrix ? GetBoundsMatrix(p1, p2) : GetBounds(p1, p2);

        // collect every pos in range
        float increment = inMatrix ? 1 : 0.5f;
        List<Vector2> res = new();
        for (float x = lowest.x; x <= highest.x; x += increment)
        {
            for (float y = lowest.y; y <= highest.y; y += increment)
            {
                res.Add(new(x, y));
            }
        }

        return res;
    }

    public static List<Vector2> GetCurrentFillRange()
    {
        if (SelectionStart == null || SelectionEnd == null) return null;
        return GetFillRange((Vector2)SelectionStart, (Vector2)SelectionEnd,
            EditModeManager.Instance.CurrentEditMode.GetWorldPosition());
    }

    public void FillSelectedArea()
    {
        if (!Selecting) return;

        FillArea(CurrentSelectionRange, EditModeManager.Instance.CurrentEditMode);
        ResetPreview();
        Selecting = false;
        selectionOptions.SetActive(false);
    }

    public void FillArea(List<Vector2> poses, FieldType type)
    {
        if (CurrentSelectionRange == null) return;
        CurrentSelectionRange = null;

        // set rotation
        int rotation = type.IsRotatable()
            ? EditModeManager.Instance.EditRotation
            : 0;

        // find bounds
        (Vector2Int lowest, Vector2Int highest) = GetBoundsMatrix(poses);

        // check if its 1 wide
        if (lowest.x == highest.x || lowest.y == highest.y)
        {
            foreach (Vector2 pos in poses)
            {
                FieldManager.Instance.SetField(pos.ConvertToMatrix(), type, rotation);
            }

            return;
        }

        AdaptAreaToType(lowest, highest, type);

        // REF
        // get prefab
        GameObject prefab = type.GetPrefab();

        // search if tag is in tags
        string[] tags = { "StartField", "GoalField", "CheckpointField" };

        foreach (string tag in tags)
        {
            if (!prefab.CompareTag(tag)) continue;
            break;
        }

        foreach (Vector2 pos in poses)
        {
            // set field at pos
            GameObject field = Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation),
                ReferenceManager.Instance.FieldContainer);

            FieldManager.ApplyStartGoalCheckpointFieldColor(field, null);

            if (field.TryGetComponent(out FieldOutline foComp)) foComp.UpdateOnStart = false;
        }

        // remove player if at changed pos
        if (!PlayerManager.StartFields.Contains(type))
        {
            GameObject player = PlayerManager.GetPlayer();

            if (player != null && player.transform.position.IsBetween(lowest.ToVector2(), highest.ToVector2()))
                Destroy(player);
        }

        UpdateOutlinesInArea(type.GetPrefab().GetComponent<FieldOutline>() != null, lowest, highest);
    }

    public void FillArea(List<Vector2> poses, EditMode editMode)
    {
        if (poses.Count == 0) return;

        if (editMode.IsFieldType())
        {
            FillArea(poses, (FieldType)EnumUtils.TryConvertEnum<EditMode, FieldType>(editMode));
            return;
        }

        DeleteArea(poses);

        foreach (Vector2 pos in poses)
        {
            GameManager.PlaceEditModeAtPosition(editMode, pos);
        }

        UpdateOutlinesInArea(false, poses[0].Floor(), poses.Last().Ceil());
    }

    public void FillArea(Vector2 start, Vector2 end, FieldType type) =>
        Instance.FillArea(GetFillRange(start, end, WorldPositionType.Matrix), type);

    private void AdaptAreaToType(Vector2 lowestPos, Vector2 highestPos, FieldType type)
    {
        // clear fields in area
        int fieldLayer = (int)Mathf.Pow(2, LayerMask.NameToLayer("Field"));
        int fieldCount = ReferenceManager.Instance.FieldContainer.childCount;
        Collider2D[] fieldHits = new Collider2D[fieldCount];
        _ = Physics2D.OverlapAreaNonAlloc(lowestPos, highestPos, fieldHits, fieldLayer);

        foreach (Collider2D fieldHit in fieldHits)
        {
            if (fieldHit == null) continue;

            Destroy(fieldHit.gameObject);
        }

        int entityLayer = (int)Mathf.Pow(2, LayerMask.NameToLayer("Entity"));

        bool clearCoins = CoinManager.CannotPlaceFields.Contains(type);
        bool clearKeys = KeyManager.CannotPlaceFields.Contains(type);

        if (!clearCoins && !clearKeys) return;

        // clear coins + keys
        Collider2D[] entityHits = Physics2D.OverlapAreaAll(lowestPos, highestPos, entityLayer);

        foreach (Collider2D hit in entityHits)
        {
            if (hit == null ||
                (!clearCoins && !hit.CompareTag("Key")) ||
                (!clearKeys && !hit.CompareTag("Coin")) ||
                (!hit.CompareTag("Coin") && !hit.CompareTag("Key"))) continue;

            Destroy(hit.gameObject);
        }
    }

    #endregion

    #region Delete

    public void DeleteSelectedArea()
    {
        DeleteArea(CurrentSelectionRange);
        CancelSelection();
    }

    public static void DeleteArea(List<Vector2> poses)
    {
        // get everything in area
        if (poses.Count == 0) return;
        Vector2 lowestPos = poses[0];
        Vector2 highestPos = poses.Last();
        Vector2 castPos = Vector2.Lerp(lowestPos, highestPos, 0.5f);
        Vector2 castSize = highestPos - lowestPos;

        Collider2D[] hits = Physics2D.OverlapBoxAll(castPos, castSize, 0, 3712);

        // DESTROY IT MUHAHAHAHAHAHHAHAHAHAHAHAHAHAHA
        foreach (Collider2D collider in hits)
        {
            Destroy(collider.gameObject);
            DestroyImmediate(collider);
        }

        GameObject player = PlayerManager.GetPlayer();

        if (player != null && !PlayerManager.CanPlace(player.transform.position, false))
            PlayerManager.Instance.RemovePlayerAtPos(player.transform.position);

        UpdateOutlinesInArea(false, lowestPos, highestPos);
    }

    #endregion

    #region Copy

    public void CopySelection()
    {
        Vector2 lowestPos = CurrentSelectionRange[0];
        Vector2 highestPos = CurrentSelectionRange[^1];

        CopyManager.Instance.Copy(lowestPos, highestPos);

        CancelSelection();
    }

    public void CutSelection()
    {
        CopySelection();
        DeleteSelectedArea();
    }

    #endregion

    #region Selection outline

    public void InitSelectionOutline(Vector2 start)
    {
        // reset selection marking
        if (selectionOutline != null) Destroy(selectionOutline);

        // set selection start and end
        SelectionStart = start;
        SelectionEnd = start;

        // set new outline
        DrawManager.SetWeight(0.1f);
        DrawManager.SetFill(Color.black);

        DrawManager.SetLayerID(DrawManager.DefaultLayerID);
        DrawManager.SetOrderInLayer(0);
        selectionOutline = DrawManager.DrawRect(
            start.x + 0.5f,
            start.y + 0.5f,
            -1,
            -1,
            false, ReferenceManager.Instance.SelectionOutlineContainer
        ).gameObject;

        selectionOutlineAnim = selectionOutline.AddComponent<LineAnimator>();
    }

    public void AnimSelectionOutline(Vector2 start, Vector2 end)
    {
        if (selectionOutlineAnim == null) return;

        // set selection start and end
        SelectionStart = start;
        SelectionEnd = end;

        // get position and stuff
        float width = end.x - start.x;
        float height = end.y - start.y;

        float x = width > 0 ? start.x - 0.5f : start.x + 0.5f;
        float y = height > 0 ? start.y - 0.5f : start.y + 0.5f;
        float w = width > 0 ? width + 1 : width - 1;
        float h = height > 0 ? height + 1 : height - 1;

        List<Vector2> lineVertices = new(new Vector2[]
        {
            new(x, y),
            new(x + w, y),
            new(x + w, y + h),
            new(x, y + h),
            new(x, y)
        });

        selectionOutlineAnim.AnimateAllPoints(lineVertices, .1f, Ease.OutSine);
    }

    #endregion

    private static void UpdateOutlinesInArea(bool hasOutline, Vector2 lowest, Vector2 highest)
    {
        int width = (int)highest.x - (int)lowest.x;
        int height = (int)highest.y - (int)lowest.y;

        // update outlines
        if (hasOutline)
        {
            // update lowest and highest field separately cause ray casting
            GameObject lowestField = FieldManager.GetField(Vector2Int.RoundToInt(lowest));
            if (lowestField.TryGetComponent(out FieldOutline foComp))
            {
                foComp.UpdateOutline(Vector2.left, true);
                foComp.UpdateOutline(Vector2.down, true);
            }

            GameObject highestField = FieldManager.GetField(Vector2Int.RoundToInt(highest));
            if (highestField.TryGetComponent(out foComp))
            {
                foComp.UpdateOutline(Vector2.right, true);
                foComp.UpdateOutline(Vector2.up, true);
            }

            // // horizontal
            RaycastHit2D[] hits = new RaycastHit2D[width];

            // bottom Fields
            _ = Physics2D.RaycastNonAlloc(lowest, Vector2.right, hits, width);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out foComp))
                    foComp.UpdateOutline(Vector2.down, true);
            }

            // top Fields
            _ = Physics2D.RaycastNonAlloc(highest, Vector2.left, hits, width);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out foComp))
                    foComp.UpdateOutline(Vector2.up, true);
            }

            // // vertical
            hits = new RaycastHit2D[height];

            // left Fields
            _ = Physics2D.RaycastNonAlloc(lowest, Vector2.up, hits, height);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out foComp))
                    foComp.UpdateOutline(Vector2.left, true);
            }

            // right Fields
            _ = Physics2D.RaycastNonAlloc(highest, Vector2.down, hits, height);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out foComp))
                    foComp.UpdateOutline(Vector2.right, true);
            }

            return;
        }

        // update fields around fill area
        (Vector2, Vector2, int)[] rays =
        {
            (new(lowest.x - 1, lowest.y - 1), Vector2.right, width + 2),
            (new(lowest.x - 1, lowest.y - 1), Vector2.up, height + 2),
            (new(highest.x + 1, highest.y + 1), Vector2.left, width + 2),
            (new(highest.x + 1, highest.y + 1), Vector2.down, height + 2)
        };

        foreach ((Vector2 origin, Vector2 direction, int length) in rays)
        {
            RaycastHit2D[] currentHits = new RaycastHit2D[length];
            _ = Physics2D.RaycastNonAlloc(origin, direction, currentHits, length);

            foreach (RaycastHit2D r in currentHits)
            {
                if (r.collider == null) continue;

                GameObject collider = r.collider.gameObject;

                if (collider.TryGetComponent(out FieldOutline outline)) outline.UpdateOutline();
            }
        }
    }

    public void ResetPreview()
    {
        // reset preview
        DestroyPreview();

        // enable placement preview
        if (!EditModeManager.Instance.Playing)
        {
            GameObject preview = ReferenceManager.Instance.PlacementPreview.gameObject;
            preview.SetActive(true);
            preview.transform.position =
                FollowMouse.GetCurrentMouseWorldPos(preview.GetComponent<FollowMouse>().WorldPosition);
        }

        // reset selection marking
        if (selectionOutline != null) Destroy(selectionOutline);
    }

    public void CancelSelection()
    {
        ResetPreview();

        // hide selection menu
        Instance.selectionOptions.SetActive(false);

        Selecting = false;

        MenuManager.Instance.BlockMenu = false;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}