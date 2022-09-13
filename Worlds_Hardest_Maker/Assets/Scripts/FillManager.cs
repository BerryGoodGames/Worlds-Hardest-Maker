using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

/// <summary>
/// contains methods for filling: GetFillRange, FillArea, GetBounds, GetBoundsMatrix
/// attach to game manager
/// </summary>
public class FillManager : MonoBehaviour
{
    public static FillManager Instance { get; private set; }

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
    private void Update()
    {
        // update fill markings
        if (!GameManager.Instance.Playing && MouseManager.Instance.MouseDragStart != null && MouseManager.Instance.MouseDragEnd != null && GameManager.Instance.Filling)
        {
            // get drag positions and world position mode
            FollowMouse.WorldPosition worldPosition = GameManager.Instance.CurrentEditMode.GetWorldPosition();

            (Vector2 start, Vector2 end) = MouseManager.GetDragPositions(worldPosition);

            // disable normal placement preview
            GameManager.Instance.PlacementPreview.SetActive(false);

            if (prevStart == null || !prevStart.Equals(start) || !prevEnd.Equals(end))
            {
                // reset fill marking
                foreach (Transform stroke in GameManager.Instance.FillOutlineContainer.transform)
                {
                    Destroy(stroke.gameObject);
                }
                // reset fill previews
                foreach (Transform preview in GameManager.Instance.FillPreviewContainer.transform)
                {
                    Destroy(preview.gameObject);
                }

                // get fill range
                List<Vector2> fillRange = GetFillRange(start, end, worldPosition);

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
                    height > 0? height + 1 : height - 1, 
                    alignCenter: false, parent: GameManager.Instance.FillOutlineContainer.transform
                );

                // set new previews, only if editmode not in NoFillPreviewModes
                if (!NoFillPreviewModes.Contains(GameManager.Instance.CurrentEditMode))
                {
                    foreach (Vector2 pos in fillRange)
                    {
                        Instantiate(GameManager.Instance.FillPreview, pos, Quaternion.identity, GameManager.Instance.FillPreviewContainer.transform);
                    }
                }

                GameManager.Instance.CurrentFillRange = fillRange;
            }

            prevStart = (Vector2)MouseManager.Instance.MouseDragStart;
            prevEnd = (Vector2)MouseManager.Instance.MouseDragEnd;
        }
    }

    #region GET BOUNDS
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
        return (Mathf.RoundToInt(lowestX), Mathf.RoundToInt(highestX), Mathf.RoundToInt(lowestY), Mathf.RoundToInt(highestY));
    }
    private static (int, int, int, int) GetBoundsMatrix(params Vector2[] points) { return GetBoundsMatrix(points.ToList()); }
    #endregion

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

    [PunRPC]
    public void FillArea(List<Vector2> poses, FieldManager.FieldType type)
    {
        if (GameManager.Instance.CurrentFillRange == null) return;
        GameManager.Instance.CurrentFillRange = null;

        // find bounds
        var (lowestX, highestX, lowestY, highestY) = GetBoundsMatrix(poses);
        int width = highestX - lowestX;
        int height = highestY - lowestY;

        // check if its 1 wide
        if (lowestX == highestX || lowestY == highestY)
        {
            foreach (Vector2 pos in poses)
            {
                FieldManager.Instance.SetField((int)pos.x, (int)pos.y, type);
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
            GameObject prefab = FieldManager.GetPrefabByType(type);
            GameObject field = Instantiate(prefab, pos, Quaternion.identity, GameManager.Instance.FieldContainer.transform);
            // REF
            string[] tags = { "StartField", "GoalField", "StartAndGoalField", "CheckpointField" };

            for (int i = 0; i < tags.Length; i++)
            {
                // TODO: similar code to GraphicsSettings.cs SetOneColorStartGoal + VERY bad performance
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

        // update outlines
        if (FieldManager.GetPrefabByType(type).GetComponent<FieldOutline>() != null)
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
    [PunRPC]
    public void FillArea(Vector2 start, Vector2 end, FieldManager.FieldType type)
    {
        Instance.FillArea(GetFillRange(start, end, FollowMouse.WorldPosition.MATRIX), type);
    }

    public static void ResetPreview()
    {
        // reset fill marking
        foreach (Transform stroke in GameManager.Instance.FillOutlineContainer.transform)
        {
            Destroy(stroke.gameObject);
        }

        // reset preview
        foreach (Transform preview in GameManager.Instance.FillPreviewContainer.transform)
        {
            Destroy(preview.gameObject);
        }

        // enable placement preview
        if (!GameManager.Instance.Playing)
        {
            GameObject preview = GameManager.Instance.PlacementPreview;
            preview.SetActive(true);
            preview.transform.position = FollowMouse.GetCurrentMouseWorldPos(preview.GetComponent<FollowMouse>().worldPosition);
        }
        
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
