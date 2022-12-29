using System.Linq;
using UnityEngine;

/// <summary>
///     apply to every field prefab variant which has outlines (vgl. TypesWithOutlines)
/// </summary>
public class FieldOutline : MonoBehaviour
{
    // array not dynamic
    public static readonly FieldType[] typesWithOutlines =
    {
        FieldType.WALL_FIELD,
        FieldType.GRAY_KEY_DOOR_FIELD,
        FieldType.RED_KEY_DOOR_FIELD,
        FieldType.BLUE_KEY_DOOR_FIELD,
        FieldType.GREEN_KEY_DOOR_FIELD,
        FieldType.YELLOW_KEY_DOOR_FIELD
    };

    public Color color = Color.black;
    public bool imitateAlpha;
    public float weight = 0.1f;
    public int order = 2;
    [Space] public string[] connectTags;
    public float rayLength = 1f;

    private readonly Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    [HideInInspector] public bool updateOnStart = true;
    private GameObject lineContainer;
    private SpriteRenderer spriteRenderer;
    private bool hasSpriteRenderer;

    private LineRenderer[] lineRenderers;

    private void Awake()
    {
        // create line container which has this transform as parent
        lineContainer = new("LineContainer")
        {
            transform = { parent = transform }
        };

        if (connectTags.Length == 0) connectTags = new[] { transform.tag };

        if (TryGetComponent(out spriteRenderer)) hasSpriteRenderer = true;
    }

    private void Start()
    {
        // get components if not already cached
        lineRenderers ??= GetComponentsInChildren<LineRenderer>();

        UpdateAlpha();

        if (updateOnStart) UpdateOutline(true);
    }

    private void Update()
    {
        UpdateAlpha();
    }

    // TODO: code duplication
    public void UpdateOutline(bool updateAround = false)
    {
        // TODO: IMPROVE
        // debug stuff so not important
        if (Dbg.Instance.dbgEnabled && !Dbg.Instance.wallOutlines) return;

        ClearLines();

        foreach (Vector2 dir in directions)
        {
            // TODO: use IsConnectorThere (?)
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, rayLength);
            if (Dbg.Instance.drawRays) Debug.DrawRay(transform.position, dir, Color.red, 20);

            bool there = false;

            // check if there are objects in that dir
            foreach (RaycastHit2D r in hits)
            {
                if (updateAround && r.collider.GetComponent<FieldOutline>() != null)
                {
                    r.collider.gameObject.GetComponent<FieldOutline>().UpdateOutline();
                }

                if (!connectTags.Contains(r.collider.tag)) continue;

                there = true;
                break;
            }

            // if there is a connector then no line
            if (there) continue;

            DrawLine(dir);
        }
    }

    public void UpdateOutline(Vector2 dir, bool update = false)
    {
        if (Dbg.Instance.dbgEnabled && !Dbg.Instance.wallOutlines) return;

        ClearLineInDir(dir);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, rayLength);
        if (Dbg.Instance.drawRays) Debug.DrawRay(transform.position, dir, Color.red, 20);

        bool there = false;

        // check if there are objects in that dir
        foreach (RaycastHit2D r in hits)
        {
            if (update && r.collider.gameObject.GetComponent<FieldOutline>() != null)
            {
                r.collider.gameObject.GetComponent<FieldOutline>().UpdateOutline();
            }

            if (!connectTags.Contains(r.collider.tag)) continue;

            there = true;
            break;
        }

        if (there) return;

        DrawLine(dir);
    }

    private void DrawLine(Vector2 dir)
    {
        Transform t = transform;
        Vector2 localPosition = t.localPosition;
        Vector2 localScale = t.localScale;

        LineManager.SetWeight(weight);
        LineManager.SetFill(color);
        LineManager.SetLayerID(LineManager.outlineLayerID);
        LineManager.SetOrderInLayer(order);
        if (dir.Equals(Vector2.up) || dir.Equals(Vector2.down))
        {
            // TODO: inefficient with rays n stuff
            // REF
            // get left & right hits to fill in gaps in inner corners
            bool left = IsConnectorInDirection(Vector2.left);
            bool right = IsConnectorInDirection(Vector2.right);

            LineManager.DrawLine(
                localPosition.x - localScale.x * 0.5f - (left ? weight : 0),
                localPosition.y + dir.y * 0.5f - dir.y * weight * 0.5f,
                localPosition.x + localScale.x * 0.5f + (right ? weight : 0),
                localPosition.y + dir.y * 0.5f - dir.y * weight * 0.5f,
                lineContainer.transform
            );
        }
        else if (dir.Equals(Vector2.left) || dir.Equals(Vector2.right))
        {
            LineManager.DrawLine(
                localPosition.x + dir.x * 0.5f - dir.x * weight * 0.5f,
                localPosition.y + localScale.y * 0.5f,
                localPosition.x + dir.x * 0.5f - dir.x * weight * 0.5f,
                localPosition.y - localScale.y * 0.5f,
                lineContainer.transform
            );
        }

        lineRenderers = GetComponentsInChildren<LineRenderer>();
    }

    private bool IsConnectorInDirection(Vector2 direction)
    {
        RaycastHit2D[] leftHits = Physics2D.RaycastAll(transform.position, direction, rayLength);

        foreach (RaycastHit2D r in leftHits)
        {
            if (!connectTags.Contains(r.collider.tag)) continue;

            return true;
        }

        return false;
    }

    private void ClearLines()
    {
        // clear lines
        foreach (Transform child in lineContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearLineInDir(Vector2 dir)
    {
        // clear line
        foreach (Transform child in lineContainer.transform)
        {
            if ((Vector2)child.gameObject.transform.localPosition != dir) continue;

            Destroy(child.gameObject);
            return;
        }
    }

    public void UpdateAlpha()
    {
        if (!imitateAlpha || !hasSpriteRenderer || color.a.EqualsFloat(spriteRenderer.color.a)) return;

        color = new(color.r, color.g, color.b, spriteRenderer.color.a);
        foreach (LineRenderer line in lineRenderers)
        {
            if (line == null) continue;

            line.startColor = color;
            line.endColor = color;
        }
    }
}