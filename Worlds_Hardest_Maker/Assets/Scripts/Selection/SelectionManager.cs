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

    private GameObject selectionOutline;
    private LineAnimator selectionOutlineAnim;

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
            FollowMouse.WorldPosition worldPosition = GameManager.Instance.CurrentEditMode.GetWorldPosition();

            (Vector2 start, Vector2 end) = MouseManager.GetDragPositions(worldPosition);

            // disable normal placement preview
            GameManager.Instance.PlacementPreview.SetActive(false);

            if (Input.GetMouseButtonDown(GameManager.Instance.SelectionMouseButton)) OnStartSelect(start);
            else if (Input.GetMouseButtonUp(GameManager.Instance.SelectionMouseButton)) OnAreaSelected(start, end);

            if (prevStart == null || !prevStart.Equals(start) || !prevEnd.Equals(end)) OnAreaSelectionChanged(start, end);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelSelection();
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
        // called when area selection changed (lol)

        // set selection outline (if u didnt already see)
        AnimSelectionOutline(start, end);
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
        if(type == FieldManager.FieldType.WALL_FIELD) // also destroy keys/coins
        {
            Collider2D[] hits = Physics2D.OverlapAreaAll(lowestPos, highestPos, 3200);
            foreach (Collider2D hit in hits)
            {
                if(hit.gameObject.IsField() || hit.CompareTag("Key") || hit.CompareTag("Coin"))
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
        string tag = "";
        int? tagIndex = null;

        string[] tags = { "StartField", "GoalField", "CheckpointField" };

        for (int i = 0; i < tags.Length; i++)
        {
            if (prefab.CompareTag(tags[i]))
            {
                tag = tags[i];
                tagIndex = i;
                break;
            }
        }

        // get colorful colors to start, goal, checkpoints and startgoal fields
        List<Color> colors = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors;

        // remove player if at changed pos
        if (!PlayerManager.StartFields.Contains(type))
        {
            // TODO: 9x bad performance than before
            GameObject player = PlayerManager.GetPlayer();
            if (player != null && player.transform.position.Between(lowestPos, highestPos))
                Destroy(player);
        }


        foreach (Vector2 pos in poses)
        {
            int mx = (int)pos.x;
            int my = (int)pos.y;


            // set field at pos
            GameObject field = Instantiate(prefab, pos, Quaternion.Euler(0, 0, rotation), GameManager.Instance.FieldContainer.transform);

            if (tagIndex != null)
            {
                if (GraphicsSettings.Instance.oneColorStartGoal)
                {
                    field.GetComponent<SpriteRenderer>().color = ColorPaletteManager.GetColorPalette("Start Goal Checkpoint").colors[4];

                    if (field.TryGetComponent(out Animator anim))
                    {
                        anim.enabled = false;
                    }
                }
                //else
                //{

                //    SpriteRenderer renderer = field.GetComponent<SpriteRenderer>();
                //    if (field.CompareTag(tag))
                //    {
                //        renderer.color = colors[(int)tagIndex];

                //        if (field.TryGetComponent(out Animator anim))
                //        {
                //            anim.enabled = true;
                //        }
                //    }
                //}
            }

            if (field.TryGetComponent(out FieldOutline FOComp))
            {
                FOComp.updateOnStart = false;
            }
        }

        UpdateOutlinesInArea(type.GetPrefab().GetComponent<FieldOutline>() != null, new(lowestX, lowestY), new(highestX, highestY));
    }
    public void FillArea(List<Vector2> poses, GameManager.EditMode editMode)
    {
        if (poses.Count == 0) return;

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

        UpdateOutlinesInArea(false, poses[0].Floor(), poses.Last().Ceil());
    }
    public void FillArea(Vector2 start, Vector2 end, FieldManager.FieldType type)
    {
        Instance.FillArea(GetFillRange(start, end, FollowMouse.WorldPosition.MATRIX), type);
    }
    #endregion

    #region Delete
    public void DeleteSelectedArea()
    {
        DeleteArea(GameManager.Instance.CurrentSelectionRange);
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
        foreach(Collider2D collider in hits)
        {
            Destroy(collider.gameObject);
            DestroyImmediate(collider);
        }

        UpdateOutlinesInArea(false, lowestPos, highestPos);
    }

    #endregion

    #region Copy
    public void CopySelection()
    {
        Vector2 lowestPos = GameManager.Instance.CurrentSelectionRange[0];
        Vector2 highestPos = GameManager.Instance.CurrentSelectionRange.Last();

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
        if(selectionOutline != null) Destroy(selectionOutline);
        
        // set selection start and end
        selectionStart = start;
        selectionEnd = start;

        // set new outline
        LineManager.SetWeight(0.1f);
        LineManager.SetFill(Color.black);

        LineManager.SetLayerID(LineManager.DefaultLayerID);
        LineManager.SetOrderInLayer(0);
        selectionOutline = LineManager.DrawRect(
            start.x + 0.5f,
            start.y + 0.5f,
            -1,
            -1,
            alignCenter: false, parent: GameManager.Instance.SelectionOutlineContainer.transform
        );

        selectionOutlineAnim = selectionOutline.AddComponent<LineAnimator>();
    }
    public void AnimSelectionOutline(Vector2 start, Vector2 end)
    {
        if(selectionOutlineAnim != null)
        {
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

            List<Vector2> lineVertecies = new(new Vector2[]
            {
                new(x, y),
                new(x + w, y),
                new(x + w, y + h),
                new(x, y + h),
                new(x, y)
            });
            
            selectionOutlineAnim.AnimateAllPoints(lineVertecies, .1f, DG.Tweening.Ease.OutSine);
        }
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
            RaycastHit2D[] hits;

            // update lowest and highest field seperately cause raycasting
            GameObject lowestField = FieldManager.GetField(lowestPos);
            if (lowestField.TryGetComponent(out FOComp))
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

            // bottom Fields
            hits = Physics2D.RaycastAll(lowestPos, Vector2.right, width);
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.transform.TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.down, true);
            }

            // left Fields
            hits = Physics2D.RaycastAll(lowestPos, Vector2.up, height);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.left, true);
            }

            // top Fields
            hits = Physics2D.RaycastAll(highestPos, Vector2.left, width);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.up, true);
            }

            // right Fields
            hits = Physics2D.RaycastAll(highestPos, Vector2.down, height);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out FOComp))
                    FOComp.UpdateOutline(Vector2.right, true);
            }

            //for (int i = lowestX; i <= highestX; i++)
            //{
            //    if (FieldManager.GetField(i, lowestY).TryGetComponent(out FOComp))
            //        FOComp.UpdateOutline(Vector2.down, true);

            //    if (FieldManager.GetField(i, highestY).TryGetComponent(out FOComp))
            //        FOComp.UpdateOutline(Vector2.up, true);
            //}

            //for (int i = lowestY; i <= highestY; i++)
            //{
            //    if (FieldManager.GetField(lowestX, i).TryGetComponent(out FOComp))
            //        FOComp.UpdateOutline(Vector2.left, true);

            //    if (FieldManager.GetField(highestX, i).TryGetComponent(out FOComp))
            //        FOComp.UpdateOutline(Vector2.right, true);
            //}
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

    public void ResetPreview()
    {
        // reset preview
        DestroyPreview();

        // enable placement preview
        if (!GameManager.Instance.Playing)
        {
            GameObject preview = GameManager.Instance.PlacementPreview;
            preview.SetActive(true);
            preview.transform.position = FollowMouse.GetCurrentMouseWorldPos(preview.GetComponent<FollowMouse>().worldPosition);
        }

        // reset selection marking
        if (selectionOutline != null) Destroy(selectionOutline);
    }

    public void CancelSelection()
    {
        ResetPreview();

        // hide selection menu
        Instance.selectionOptions.SetActive(false);

        GameManager.Instance.Selecting = false;

        MenuManager.Instance.blockMenu = false;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
