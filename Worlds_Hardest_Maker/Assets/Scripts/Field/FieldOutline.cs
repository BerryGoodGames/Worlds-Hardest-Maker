using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     apply to every field prefab variant which has outlines (vgl. TypesWithOutlines)
/// </summary>
public class FieldOutline : MonoBehaviour
{
    // array not dynamic
    public static readonly FieldType[] TypesWithOutlines =
    {
        FieldType.WALL_FIELD,
        FieldType.GRAY_KEY_DOOR_FIELD,
        FieldType.RED_KEY_DOOR_FIELD,
        FieldType.BLUE_KEY_DOOR_FIELD,
        FieldType.GREEN_KEY_DOOR_FIELD,
        FieldType.YELLOW_KEY_DOOR_FIELD
    };

    [FormerlySerializedAs("color")] public Color Color = Color.black;
    [FormerlySerializedAs("imitateAlpha")] public bool ImitateAlpha;
    [FormerlySerializedAs("weight")] public float Weight = 0.1f;
    [FormerlySerializedAs("order")] public int Order = 2;

    [FormerlySerializedAs("connectTags")] [Space]
    public string[] ConnectTags;

    [FormerlySerializedAs("rayLength")] public float RayLength = 1f;

    private readonly Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    [FormerlySerializedAs("updateOnStart")] [HideInInspector]
    public bool UpdateOnStart = true;

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

        if (ConnectTags.Length == 0) ConnectTags = new[] { transform.tag };

        if (TryGetComponent(out spriteRenderer)) hasSpriteRenderer = true;
    }

    private void Start()
    {
        // get components if not already cached
        lineRenderers ??= GetComponentsInChildren<LineRenderer>();

        UpdateAlpha();

        if (UpdateOnStart) UpdateOutline(true);
    }

    private void Update()
    {
        UpdateAlpha();
    }

    public void UpdateOutline(bool updateNeighbor = false)
    {
        // debug stuff so not important
        if (Dbg.Instance.DbgEnabled && !Dbg.Instance.WallOutlines) return;

        ClearLines();

        foreach (Vector2 dir in directions)
        {
            if (IsConnectorInDirection(dir, updateNeighbor)) continue;

            DrawLine(dir);
        }
    }

    public void UpdateOutline(Vector2 dir, bool updateNeighbor = false)
    {
        // debug stuff so not important
        if (Dbg.Instance.DbgEnabled && !Dbg.Instance.WallOutlines) return;

        ClearLineInDir(dir);

        if (IsConnectorInDirection(dir, updateNeighbor, true)) return;

        DrawLine(dir);
    }

    private void DrawLine(Vector2 dir)
    {
        Transform t = transform;
        Vector2 localPosition = t.localPosition;
        Vector2 localScale = t.localScale;

        LineManager.SetWeight(Weight);
        LineManager.SetFill(Color);
        LineManager.SetLayerID(LineManager.OutlineLayerID);
        LineManager.SetOrderInLayer(Order);
        if (dir.Equals(Vector2.up) || dir.Equals(Vector2.down))
        {
            // get left & right hits to fill in gaps in inner corners
            bool left = IsConnectorInDirection(Vector2.left);
            bool right = IsConnectorInDirection(Vector2.right);

            LineManager.DrawLine(
                localPosition.x - localScale.x * 0.5f - (left ? Weight : 0),
                localPosition.y + dir.y * 0.5f - dir.y * Weight * 0.5f,
                localPosition.x + localScale.x * 0.5f + (right ? Weight : 0),
                localPosition.y + dir.y * 0.5f - dir.y * Weight * 0.5f,
                lineContainer.transform
            );
        }
        else if (dir.Equals(Vector2.left) || dir.Equals(Vector2.right))
        {
            LineManager.DrawLine(
                localPosition.x + dir.x * 0.5f - dir.x * Weight * 0.5f,
                localPosition.y + localScale.y * 0.5f,
                localPosition.x + dir.x * 0.5f - dir.x * Weight * 0.5f,
                localPosition.y - localScale.y * 0.5f,
                lineContainer.transform
            );
        }

        lineRenderers = GetComponentsInChildren<LineRenderer>();
    }

    private bool IsConnectorInDirection(Vector2 direction, bool updateNeighbor = false, bool drawRay = false)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, RayLength);

        if (drawRay && Dbg.Instance.DrawRays) Debug.DrawRay(transform.position, direction, Color.red, 20);

        foreach (RaycastHit2D r in hits)
        {
            FieldOutline outlineNeighbor = r.collider.gameObject.GetComponent<FieldOutline>();
            if (updateNeighbor && outlineNeighbor != null)
            {
                outlineNeighbor.UpdateOutline();
            }

            // if (!connectTags.Contains(r.collider.tag)) continue;
            if (!r.collider.CompareTag(gameObject.tag)) continue;

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
        if (!ImitateAlpha || !hasSpriteRenderer || Color.a.EqualsFloat(spriteRenderer.color.a)) return;

        Color = new(Color.r, Color.g, Color.b, spriteRenderer.color.a);
        foreach (LineRenderer line in lineRenderers)
        {
            if (line == null) continue;

            line.startColor = Color;
            line.endColor = Color;
        }
    }
}