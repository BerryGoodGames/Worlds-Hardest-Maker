using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// contains methods for filling: GetFillRange, FillArea, GetBounds, GetBoundsMatrix
/// attach to game manager
/// </summary>
public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionOptions;
    [SerializeField] private MouseOverUI fillMouseOver;
    private RectTransform selectionOptionsRect;

    public static SelectionManager Instance { get; private set; }

    public static readonly List<GameManager.EditMode> NoFillPreviewModes = new(new GameManager.EditMode[]
    {
        GameManager.EditMode.BALL_DEFAULT,
        GameManager.EditMode.BALL_CIRCLE,
        GameManager.EditMode.GRAY_KEY,
        GameManager.EditMode.RED_KEY,
        GameManager.EditMode.BLUE_KEY,
        GameManager.EditMode.GREEN_KEY,
        GameManager.EditMode.YELLOW_KEY,
        GameManager.EditMode.PLAYER
    });

    private Vector2 prevStart;
    private Vector2 prevEnd;
    public static Vector2? selectionStart;
    public static Vector2? selectionEnd;


    private void Update()
    {
        if (Input.GetMouseButton(GameManager.Instance.SelectionMouseButton) && !GameManager.Instance.Playing)
            GameManager.Instance.Selecting = true;

        // update selection markings
        if (!GameManager.Instance.Playing && MouseManager.Instance.MouseDragStart != null && MouseManager.Instance.MouseDragCurrent != null && GameManager.Instance.Selecting)
        {
            // get drag positions and world position mode
            (Vector2 start, Vector2 end) = MouseManager.GetDragPositions(FollowMouse.WorldPosition.GRID);

            // disable normal placement preview
            GameManager.Instance.PlacementPreview.SetActive(false);

            if (prevStart == null || !prevStart.Equals(start) || !prevEnd.Equals(end)) OnAreaSelectionChanged(start, end);

            if (Input.GetMouseButtonDown(GameManager.Instance.SelectionMouseButton)) OnStartSelect();
            else if(Input.GetMouseButtonUp(GameManager.Instance.SelectionMouseButton)) OnAreaSelected(start, end);
        }
    }

    private void LateUpdate()
    {
        if(MouseManager.Instance.MouseDragStart != null && MouseManager.Instance.MouseDragCurrent != null)
        {
            prevStart = ((Vector2)MouseManager.Instance.MouseDragStart).ConvertPosition(GameManager.Instance.CurrentEditMode.GetWorldPosition());
            prevEnd = ((Vector2)MouseManager.Instance.MouseDragCurrent).ConvertPosition(GameManager.Instance.CurrentEditMode.GetWorldPosition());
        }
    }

    private void Start()
    {
        selectionOptionsRect = selectionOptions.GetComponent<RectTransform>();
        GameManager.onPlay += CancelSelection;
        GameManager.onEditModeChange += RemakePreview;

        fillMouseOver.onHovered += SetPreviewVisible;
        fillMouseOver.onUnhovered += SetPreviewInvisible;
    }

    #region Callbacks
    private void OnAreaSelectionChanged(Vector2 start, Vector2 end)
    {
        // reset selection marking
        foreach (Transform stroke in GameManager.Instance.FillOutlineContainer.transform)
        {
            Destroy(stroke.gameObject);
        }

        // set selection start and end
        selectionStart = start;
        selectionEnd = end;

        // set new outline
        LineManager.SetWeight(0.1f);
        LineManager.SetFill(Color.black);

        float width = end.x - start.x;
        float height = end.y - start.y;

        LineManager.SetLayerID(LineManager.DefaultLayerID);
        LineManager.SetOrderInLayer(0);
        LineManager.DrawRect(
            width > 0 ? start.x - 0.5f : start.x + 0.5f,
            height > 0 ? start.y - 0.5f : start.y + 0.5f,
            width > 0 ? width + 1 : width - 1,
            height > 0 ? height + 1 : height - 1,
            alignCenter: false, parent: GameManager.Instance.FillOutlineContainer.transform
        );
    }
    private void OnAreaSelected(Vector2 start, Vector2 end)
    {
        // called when mouse button was released and area was selected
        selectionOptions.SetActive(true);
        float width = end.x - start.x;
        float height = end.y - start.y;

        Vector2 position = new(width < 0 ? (end.x - 0.5f) : (end.x + 0.5f), height < 0 ? (end.y - 0.5f) : (end.y + 0.5f));
        UIAttachToPoint posController = selectionOptions.GetComponent<UIAttachToPoint>();

        posController.point = position;

        selectionOptionsRect.pivot = new(width > 0 ? 0 : 1, height > 0 ? 0 : 1);
    }
    private void OnStartSelect()
    {
        // called when mouse button was pressed and user starts selecting
        selectionOptions.SetActive(false);
        CancelSelection();
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
    private static (float, float, float, float) GetBounds(params Vector2[] points) { return GetBounds(points.ToList()); }
    private static (int, int, int, int) GetBoundsMatrix(List<Vector2> points)
    {
        var (lowestX, highestX, lowestY, highestY) = GetBounds(points);
        return (Mathf.CeilToInt(lowestX), Mathf.FloorToInt(highestX), Mathf.CeilToInt(lowestY), Mathf.FloorToInt(highestY));
    }
    private static (int, int, int, int) GetBoundsMatrix(params Vector2[] points) { return GetBoundsMatrix(points.ToList()); }
    #endregion

    #region Preview
    private static void RemakePreview()
    {
        if (GameManager.Instance.FillPreviewContainer.transform.childCount == 0) return;
        DestroyPreview();
        InitSelectedPreview();
    }

    private static void InitPreview(List<Vector2> range)
    {
        // set new previews, only if editmode not in NoFillPreviewModes
        if (!NoFillPreviewModes.Contains(GameManager.Instance.CurrentEditMode))
        {
            foreach (Vector2 pos in range)
            {
                GameObject preview = Instantiate(GameManager.Instance.FillPreview, pos, Quaternion.identity, GameManager.Instance.FillPreviewContainer.transform);
                
                PreviewController c = preview.GetComponent<PreviewController>();
                c.Awake_();
                c.UpdateSprite();
                c.UpdateRotation(smooth: false);
                c.changeSpriteToCurrentEditMode = false;
            }
        }
    }

    private static void DestroyPreview()
    {
        // destroy selection previews
        foreach (Transform preview in GameManager.Instance.FillPreviewContainer.transform)
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
        foreach(Transform preview in GameManager.Instance.FillPreviewContainer.transform)
        {
            preview.GetComponent<PreviewController>().UpdateRotation();
        }
    }

    public static void UpdatePreviewSprite()
    {
        foreach (Transform preview in GameManager.Instance.FillPreviewContainer.transform)
        {
            preview.GetComponent<PreviewController>().UpdateSprite();
        }
    }

    private static void SetPreviewVisible()
    {
        if (GameManager.Instance.FillPreviewContainer.transform.childCount == 0)
        {
            InitSelectedPreview();
        }

        GameManager.Instance.FillPreviewContainer.SetActive(true);
    }
    private static void SetPreviewInvisible()
    {
        GameManager.Instance.FillPreviewContainer.SetActive(false);
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
        if(selectionStart == null || selectionEnd == null) return null;
        return GetFillRange((Vector2)selectionStart, (Vector2)selectionEnd, GameManager.Instance.CurrentEditMode.GetWorldPosition());
    }

    public void FillSelectedArea()
    {
        if (!GameManager.Instance.Selecting) return;

        FillArea(GameManager.Instance.CurrentSelectionRange, GameManager.Instance.CurrentEditMode);
        ResetPreview();
        GameManager.Instance.Selecting = false;
        selectionOptions.SetActive(false);
    }


    public void FillArea(List<Vector2> poses, FieldManager.FieldType type)
    {
        if (GameManager.Instance.CurrentSelectionRange == null) return;
        GameManager.Instance.CurrentSelectionRange = null;

        // set rotation
        int rotation = FieldManager.IsRotatable(GameManager.Instance.CurrentEditMode) ? GameManager.Instance.EditRotation : 0;

        // find bounds
        var (lowestX, highestX, lowestY, highestY) = GetBoundsMatrix(poses);

        // check if its 1 wide
        if (lowestX == highestX || lowestY == highestY)
        {
            foreach (Vector2 pos in poses)
            {
                FieldManager.Instance.SetField((int)pos.x, (int)pos.y, type, 0);
            }
            return;
        }

        // clear area
        foreach (Vector2 pos in poses)
        {
            FieldManager.Instance.RemoveField((int)pos.x, (int)pos.y);
        }

        foreach (Vector2 pos in poses)
        {
            int mx = (int)pos.x;
            int my = (int)pos.y;

            // set field at pos
            GameObject prefab = type.GetPrefab();
            GameObject field = Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation), GameManager.Instance.FieldContainer.transform);
            // REF
            string[] tags = { "StartField", "GoalField", "CheckpointField" };

            for (int i = 0; i < tags.Length; i++)
            {
                // TODO: similar code to GraphicsSettings.cs SetOneColorStartGoal + VERY bad performance
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

            if (field.TryGetComponent(out FieldOutline FOComp))
            {
                FOComp.updateOnStart = false;
            }
            // remove player if at changed pos
            if (!PlayerManager.StartFields.Contains(type))
            {
                // TODO: 9x bad performance than before
                PlayerManager.Instance.RemovePlayerAtPosIntersect(mx, my);
            }

            // remove coin if wall is placed
            if (type == FieldManager.FieldType.WALL_FIELD)
            {
                // TODO: 9x bad performance than before
                GameManager.RemoveObjectInContainerIntersect(mx, my, GameManager.Instance.CoinContainer);
            }
        }

        UpdateOutlinesInArea(type.GetPrefab().GetComponent<FieldOutline>() != null, new(lowestX, lowestY), new(highestX, highestY));
    }
    public void FillArea(List<Vector2> poses, GameManager.EditMode editMode)
    {
        FieldManager.FieldType? fieldType = (FieldManager.FieldType?)GameManager.TryConvertEnum<GameManager.EditMode, FieldManager.FieldType>(editMode);
        if(fieldType != null)
        {
            FillArea(poses, (FieldManager.FieldType)fieldType);
            return;
        }

        DeleteArea(poses);

        foreach(Vector2 pos in poses)
        {
            GameManager.Set(editMode, pos);
        }
    }
    public void FillArea(Vector2 start, Vector2 end, FieldManager.FieldType type)
    {
        Instance.FillArea(GetFillRange(start, end, FollowMouse.WorldPosition.MATRIX), type);
    }
    #endregion

    #region Delete

    public static void DeleteSelectedArea()
    {
        DeleteArea(GameManager.Instance.CurrentSelectionRange);
        CancelSelection();
    }

    public static void DeleteArea(List<Vector2> poses)
    {
        // get everything in area
        Vector2 lowestPos = poses[0];
        Vector2 highestPos = poses.Last();
        Vector2 castPos = Vector2.Lerp(lowestPos, highestPos, 0.5f);
        Vector2 castSize = highestPos - lowestPos;

        Collider2D[] hits = Physics2D.OverlapBoxAll(castPos, castSize, 0, 3712);

        // DESTROY IT MUHAHAHAHAHAHHAHAHAHAHAHAHAHAHA
        foreach(Collider2D collider in hits)
        {
            Destroy(collider.gameObject);
            DestroyImmediate(collider);
        }

        UpdateOutlinesInArea(false, lowestPos, highestPos);
    }

    #endregion

    #region Copy
    public static void CopySelection()
    {
        Vector2 lowestPos = GameManager.Instance.CurrentSelectionRange[0];
        Vector2 highestPos = GameManager.Instance.CurrentSelectionRange.Last();

        CopyManager.Copy(lowestPos, highestPos);

        CancelSelection();
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
        if (hasOutline == true)
        {
            FieldOutline FOComp;
            for (int i = lowestX; i <= highestX; i++)
            {
                if (FieldManager.GetField(i, lowestY).TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.down, true);

                if (FieldManager.GetField(i, highestY).TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.up, true);
            }

            for (int i = lowestY; i <= highestY; i++)
            {
                if (FieldManager.GetField(lowestX, i).TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.left, true);

                if (FieldManager.GetField(highestX, i).TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.right, true);
            }
        }
        else
        {
            // update fields around fill area
            (Vector2, Vector2, int)[] rays =
            {
                (new(lowestX - 1, lowestY - 1), Vector2.right, width + 2),
                (new(lowestX - 1, lowestY - 1), Vector2.up, height + 2),
                (new(highestX + 1, highestY + 1), Vector2.left, width + 2),
                (new(highestX + 1, highestY + 1), Vector2.down, height + 2)
            };

            foreach ((Vector2 Origin, Vector2 Direction, int Length) in rays)
            {
                RaycastHit2D[] currentHits = Physics2D.RaycastAll(Origin, Direction, Length);

                foreach (RaycastHit2D r in currentHits)
                {
                    GameObject collider = r.collider.gameObject;

                    if (collider.TryGetComponent(out FieldOutline outline))
                    {
                        outline.UpdateOutline(false);
                    }
                }
            }
        }
    }

    public static void ResetPreview()
    {
        // reset fill marking
        foreach (Transform stroke in GameManager.Instance.FillOutlineContainer.transform)
        {
            Destroy(stroke.gameObject);
        }

        // reset preview
        DestroyPreview();

        // enable placement preview
        if (!GameManager.Instance.Playing)
        {
            GameObject preview = GameManager.Instance.PlacementPreview;
            preview.SetActive(true);
            preview.transform.position = FollowMouse.GetCurrentMouseWorldPos(preview.GetComponent<FollowMouse>().worldPosition);
        }
    }

    public static void CancelSelection()
    {
        ResetPreview();

        // hide selection menu
        Instance.selectionOptions.SetActive(false);

        GameManager.Instance.Selecting = false;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
