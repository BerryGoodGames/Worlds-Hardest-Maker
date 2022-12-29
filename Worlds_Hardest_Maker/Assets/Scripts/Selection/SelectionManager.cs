using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
///     contains methods for filling: GetFillRange, FillArea, GetBounds, GetBoundsMatrix
///     attach to game manager
/// </summary>
public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionOptions;
    [SerializeField] private MouseOverUI fillMouseOver;
    private RectTransform selectionOptionsRect;

    private GameObject selectionOutline;
    private LineAnimator selectionOutlineAnim;
    public bool Selecting { get; set; }

    public List<Vector2> CurrentSelectionRange
    {
        get => selectionStart == null || selectionEnd == null ? null : GetCurrentFillRange();
        set
        {
            if (value != null) return;
            selectionStart = null;
            selectionEnd = null;
        }
    }

    public static SelectionManager Instance { get; private set; }

    public static readonly List<EditMode> noFillPreviewModes = new(new[]
    {
        EditMode.BALL_DEFAULT,
        EditMode.BALL_CIRCLE,
        EditMode.GRAY_KEY,
        EditMode.RED_KEY,
        EditMode.BLUE_KEY,
        EditMode.GREEN_KEY,
        EditMode.YELLOW_KEY,
        EditMode.PLAYER
    });

    private Vector2 prevStart;
    private Vector2 prevEnd;
    public static Vector2? selectionStart;
    public static Vector2? selectionEnd;


    private void Update()
    {
        if (Input.GetMouseButton(KeybindManager.Instance.selectionMouseButton) && !EditModeManager.Instance.Playing)
            Selecting = true;

        // update selection markings
        if (!EditModeManager.Instance.Playing && MouseManager.Instance.MouseDragStart != null &&
            MouseManager.Instance.MouseDragCurrent != null && Selecting)
        {
            // get drag positions and world position mode
            FollowMouse.WorldPosition worldPosition = EditModeManager.Instance.CurrentEditMode.GetWorldPosition();

            (Vector2 start, Vector2 end) = MouseManager.GetDragPositions(worldPosition);

            // disable normal placement preview
            ReferenceManager.Instance.placementPreview.SetActive(false);

            if (Input.GetMouseButtonDown(KeybindManager.Instance.selectionMouseButton)) OnStartSelect(start);
            else if (Input.GetMouseButtonUp(KeybindManager.Instance.selectionMouseButton)) OnAreaSelected(start, end);

            if (!prevStart.Equals(start) || !prevEnd.Equals(end)) OnAreaSelectionChanged(start, end);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelSelection();
    }

    private void LateUpdate()
    {
        if (MouseManager.Instance.MouseDragStart == null || MouseManager.Instance.MouseDragCurrent == null) return;

        prevStart = ((Vector2)MouseManager.Instance.MouseDragStart).ConvertPosition(EditModeManager.Instance.CurrentEditMode
            .GetWorldPosition());
        prevEnd = ((Vector2)MouseManager.Instance.MouseDragCurrent).ConvertPosition(EditModeManager.Instance.CurrentEditMode
            .GetWorldPosition());
    }

    private void Start()
    {
        selectionOptionsRect = selectionOptions.GetComponent<RectTransform>();
        EditModeManager.Instance.OnPlay += CancelSelection;
        EditModeManager.Instance.OnEditModeChange += RemakePreview;

        fillMouseOver.onHovered += SetPreviewVisible;
        fillMouseOver.onUnhovered += SetPreviewInvisible;
    }

    #region Callbacks

    private void OnAreaSelectionChanged(Vector2 start, Vector2 end)
    {
        // called when area selection changed (lol)

        // set selection outline (if u didn't already see)
        AnimSelectionOutline(start, end);
    }

    private void OnAreaSelected(Vector2 start, Vector2 end)
    {
        // called when mouse button was released and area was selected
        selectionOptions.SetActive(true);
        float width = end.x - start.x;
        float height = end.y - start.y;

        Vector2 position = new(width < 0 ? end.x - 0.5f : end.x + 0.5f, height < 0 ? end.y - 0.5f : end.y + 0.5f);
        UIAttachToPoint posController = selectionOptions.GetComponent<UIAttachToPoint>();

        posController.point = position;

        selectionOptionsRect.pivot = new(width > 0 ? 0 : 1, height > 0 ? 0 : 1);

        RemakePreview();
    }

    private void OnStartSelect(Vector2 start)
    {
        // called when mouse button was pressed and user starts selecting
        InitSelectionOutline(start);

        selectionOptions.SetActive(false);

        MenuManager.Instance.blockMenu = true;
    }

    #endregion

    #region Get bounds

    // get bounds of multiple points (in matrix)
    private static (float, float, float, float) GetBounds(List<Vector2> points)
    {
        float lowestX = points[0].x;
        float highestX = points[0].x;
        float lowestY = points[0].y;
        float highestY = points[0].y;
        foreach (Vector2 pos in points)
        {
            if (lowestX > pos.x) lowestX = pos.x;
            if (lowestY > pos.y) lowestY = pos.y;
            if (highestX < pos.x) highestX = pos.x;
            if (highestY < pos.y) highestY = pos.y;
        }

        return (lowestX, highestX, lowestY, highestY);
    }

    private static (float, float, float, float) GetBounds(params Vector2[] points)
    {
        return GetBounds(points.ToList());
    }

    private static (int, int, int, int) GetBoundsMatrix(List<Vector2> points)
    {
        var (lowestX, highestX, lowestY, highestY) = GetBounds(points);
        return (Mathf.CeilToInt(lowestX), Mathf.FloorToInt(highestX), Mathf.CeilToInt(lowestY),
            Mathf.FloorToInt(highestY));
    }

    private static (int, int, int, int) GetBoundsMatrix(params Vector2[] points)
    {
        return GetBoundsMatrix(points.ToList());
    }

    #endregion

    #region Preview

    private static void RemakePreview()
    {
        if (ReferenceManager.Instance.fillPreviewContainer.childCount == 0) return;
        DestroyPreview();
        InitSelectedPreview();
    }

    private static void InitPreview(List<Vector2> range)
    {
        // set new previews, only if edit mode not in NoFillPreviewModes
        if (noFillPreviewModes.Contains(EditModeManager.Instance.CurrentEditMode)) return;

        foreach (Vector2 pos in range)
        {
            GameObject preview = Instantiate(PrefabManager.Instance.fillPreview, pos, Quaternion.identity,
                ReferenceManager.Instance.fillPreviewContainer);

            PreviewController c = preview.GetComponent<PreviewController>();
            c.Awake_();
            c.UpdateSprite();
            c.UpdateRotation(smooth: false);
            c.changeSpriteToCurrentEditMode = false;
        }
    }

    private static void DestroyPreview()
    {
        // destroy selection previews
        foreach (Transform preview in ReferenceManager.Instance.fillPreviewContainer)
        {
            Destroy(preview.gameObject);
        }
    }

    private static void InitSelectedPreview()
    {
        InitPreview(GetCurrentFillRange());
    }

    public static void UpdatePreviewRotation()
    {
        foreach (Transform preview in ReferenceManager.Instance.fillPreviewContainer)
        {
            preview.GetComponent<PreviewController>().UpdateRotation();
        }
    }

    public static void UpdatePreviewSprite()
    {
        foreach (Transform preview in ReferenceManager.Instance.fillPreviewContainer)
        {
            preview.GetComponent<PreviewController>().UpdateSprite();
        }
    }

    private static void SetPreviewVisible()
    {
        if (ReferenceManager.Instance.fillPreviewContainer.childCount == 0)
        {
            InitSelectedPreview();
        }

        ReferenceManager.Instance.fillPreviewContainer.gameObject.SetActive(true);
    }

    private static void SetPreviewInvisible()
    {
        ReferenceManager.Instance.fillPreviewContainer.gameObject.SetActive(false);
    }

    #endregion

    #region Fill

    public static List<Vector2> GetFillRange(Vector2 p1, Vector2 p2, FollowMouse.WorldPosition worldPosition)
    {
        bool inMatrix = worldPosition == FollowMouse.WorldPosition.MATRIX;

        // find bounds
        var (lowestX, highestX, lowestY, highestY) = inMatrix ? GetBoundsMatrix(p1, p2) : GetBounds(p1, p2);

        // collect every pos in range
        float increment = inMatrix ? 1 : 0.5f;
        List<Vector2> res = new();
        for (float x = lowestX; x <= highestX; x += increment)
        {
            for (float y = lowestY; y <= highestY; y += increment)
            {
                res.Add(new(x, y));
            }
        }

        return res;
    }

    public static List<Vector2> GetCurrentFillRange()
    {
        if (selectionStart == null || selectionEnd == null) return null;
        return GetFillRange((Vector2)selectionStart, (Vector2)selectionEnd,
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
        int rotation = FieldManager.IsRotatable(EditModeManager.Instance.CurrentEditMode)
            ? EditModeManager.Instance.EditRotation
            : 0;

        // find bounds
        (int lowestX, int highestX, int lowestY, int highestY) = GetBoundsMatrix(poses);

        Vector2 lowestPos = new(lowestX, lowestY);
        Vector2 highestPos = new(highestX, highestY);

        // check if its 1 wide
        if (lowestX == highestX || lowestY == highestY)
        {
            foreach (Vector2 pos in poses)
            {
                FieldManager.Instance.SetField((int)pos.x, (int)pos.y, type, rotation);
            }

            return;
        }

        // clear fields in area
        if (type == FieldType.WALL_FIELD) // also destroy keys/coins
        {
            int fieldCount = ReferenceManager.Instance.fieldContainer.childCount;
            int coinCount = ReferenceManager.Instance.coinContainer.childCount;
            int keyCount = ReferenceManager.Instance.keyContainer.childCount;
            Collider2D[] hits = new Collider2D[fieldCount + coinCount + keyCount];

            _ = Physics2D.OverlapAreaNonAlloc(lowestPos, highestPos, hits, 3200);
            foreach (Collider2D hit in hits)
            {
                if (hit == null) continue;

                if (hit.gameObject.IsField() || hit.CompareTag("Key") || hit.CompareTag("Coin"))
                    Destroy(hit.gameObject);
            }
        }
        else
        {
            Collider2D[] hits = Physics2D.OverlapAreaAll(lowestPos, highestPos, 3072);
            foreach (Collider2D hit in hits)
            {
                Destroy(hit.gameObject);
            }
        }

        // REF
        // get prefab
        GameObject prefab = type.GetPrefab();

        // search if tag is in tags
        int? tagIndex = null;

        string[] tags = { "StartField", "GoalField", "CheckpointField" };

        for (int i = 0; i < tags.Length; i++)
        {
            if (!prefab.CompareTag(tags[i])) continue;

            tagIndex = i;
            break;
        }

        // get colorful colors to start, goal, checkpoints and start goal fields
        List<Color> colors = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors;

        // remove player if at changed pos
        if (!PlayerManager.startFields.Contains(type))
        {
            // TODO: 9x bad performance than before
            GameObject player = PlayerManager.GetPlayer();

            if (player != null && player.transform.position.Between(lowestPos, highestPos))
                Destroy(player);
        }


        foreach (Vector2 pos in poses)
        {
            // set field at pos
            GameObject field = Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation),
                ReferenceManager.Instance.fieldContainer);

            if (tagIndex != null)
            {
                if (GraphicsSettings.Instance.oneColorStartGoalCheckpoint)
                {
                    field.GetComponent<SpriteRenderer>().color = colors[4];

                    if (field.TryGetComponent(out Animator anim))
                    {
                        anim.enabled = false;
                    }
                }
            }

            if (field.TryGetComponent(out FieldOutline FOComp))
            {
                FOComp.updateOnStart = false;
            }
        }

        UpdateOutlinesInArea(type.GetPrefab().GetComponent<FieldOutline>() != null, new(lowestX, lowestY),
            new(highestX, highestY));
    }

    public void FillArea(List<Vector2> poses, EditMode editMode)
    {
        if (poses.Count == 0) return;

        FieldType? fieldType = (FieldType?)GameManager.TryConvertEnum<EditMode, FieldType>(editMode);
        if (fieldType != null)
        {
            FillArea(poses, (FieldType)fieldType);
            return;
        }

        DeleteArea(poses);

        foreach (Vector2 pos in poses)
        {
            GameManager.PlaceEditModeAtPosition(editMode, pos);
        }

        UpdateOutlinesInArea(false, poses[0].Floor(), poses.Last().Ceil());
    }

    public void FillArea(Vector2 start, Vector2 end, FieldType type)
    {
        Instance.FillArea(GetFillRange(start, end, FollowMouse.WorldPosition.MATRIX), type);
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

        if (!PlayerManager.CanPlace(player.transform.position, false))
            PlayerManager.Instance.RemovePlayerAtPos(player.transform.position);

        UpdateOutlinesInArea(false, lowestPos, highestPos);
    }

    #endregion

    #region Copy

    public void CopySelection()
    {
        Vector2 lowestPos = CurrentSelectionRange[0];
        Vector2 highestPos = CurrentSelectionRange.Last();

        CopyManager.Copy(lowestPos, highestPos);

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
        selectionStart = start;
        selectionEnd = start;

        // set new outline
        LineManager.SetWeight(0.1f);
        LineManager.SetFill(Color.black);

        LineManager.SetLayerID(LineManager.defaultLayerID);
        LineManager.SetOrderInLayer(0);
        selectionOutline = LineManager.DrawRect(
            start.x + 0.5f,
            start.y + 0.5f,
            -1,
            -1,
            false, ReferenceManager.Instance.selectionOutlineContainer
        );

        selectionOutlineAnim = selectionOutline.AddComponent<LineAnimator>();
    }

    public void AnimSelectionOutline(Vector2 start, Vector2 end)
    {
        if (selectionOutlineAnim == null) return;

        // set selection start and end
        selectionStart = start;
        selectionEnd = end;

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

    private static void UpdateOutlinesInArea(bool hasOutline, Vector2 lowestPos, Vector2 highestPos)
    {
        int highestX = (int)highestPos.x;
        int highestY = (int)highestPos.y;
        int lowestX = (int)lowestPos.x;
        int lowestY = (int)lowestPos.y;
        int width = highestX - lowestX;
        int height = highestY - lowestY;

        // update outlines
        if (hasOutline)
        {
            // update lowest and highest field separately cause ray casting
            GameObject lowestField = FieldManager.GetField(lowestPos);
            if (lowestField.TryGetComponent(out FieldOutline FOComp))
            {
                FOComp.UpdateOutline(Vector2.left, true);
                FOComp.UpdateOutline(Vector2.down, true);
            }

            GameObject highestField = FieldManager.GetField(highestPos);
            if (highestField.TryGetComponent(out FOComp))
            {
                FOComp.UpdateOutline(Vector2.right, true);
                FOComp.UpdateOutline(Vector2.up, true);
            }

            // // horizontal
            RaycastHit2D[] hits = new RaycastHit2D[width];

            // bottom Fields
            _ = Physics2D.RaycastNonAlloc(lowestPos, Vector2.right, hits, width);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.down, true);
            }

            // top Fields
            _ = Physics2D.RaycastNonAlloc(highestPos, Vector2.left, hits, width);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.up, true);
            }

            // // vertical
            hits = new RaycastHit2D[height];

            // left Fields
            _ = Physics2D.RaycastNonAlloc(lowestPos, Vector2.up, hits, height);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.left, true);
            }

            // right Fields
            _ = Physics2D.RaycastNonAlloc(highestPos, Vector2.down, hits, height);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.right, true);
            }

            return;
        }

        // update fields around fill area
        (Vector2, Vector2, int)[] rays =
        {
            (new(lowestX - 1, lowestY - 1), Vector2.right, width + 2),
            (new(lowestX - 1, lowestY - 1), Vector2.up, height + 2),
            (new(highestX + 1, highestY + 1), Vector2.left, width + 2),
            (new(highestX + 1, highestY + 1), Vector2.down, height + 2)
        };

        foreach ((Vector2 origin, Vector2 direction, int length) in rays)
        {
            RaycastHit2D[] currentHits = new RaycastHit2D[length];
            _ = Physics2D.RaycastNonAlloc(origin, direction, currentHits, length);

            foreach (RaycastHit2D r in currentHits)
            {
                if (r.collider == null) continue;

                GameObject collider = r.collider.gameObject;

                if (collider.TryGetComponent(out FieldOutline outline))
                {
                    outline.UpdateOutline();
                }
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
            GameObject preview = ReferenceManager.Instance.placementPreview;
            preview.SetActive(true);
            preview.transform.position =
                FollowMouse.GetCurrentMouseWorldPos(preview.GetComponent<FollowMouse>().worldPosition);
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

        MenuManager.Instance.blockMenu = false;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}