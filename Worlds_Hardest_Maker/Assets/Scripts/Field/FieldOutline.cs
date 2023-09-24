using System.Collections.Generic;
using MyBox;
using UnityEngine;

/// <summary>
///     Attach to every field prefab variant which has outlines (see TypesWithOutlines)
/// </summary>
public class FieldOutline : MonoBehaviour
{
    // array not dynamic
    public static readonly FieldType[] TypesWithOutlines =
    {
        FieldType.WallField,
        FieldType.GrayKeyDoorField,
        FieldType.RedKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.YellowKeyDoorField
    };

    [SerializeField] private Color color = Color.black;
    [SerializeField] private bool imitateAlpha;
    [SerializeField] private float weight = 0.1f;

    [Separator] [SerializeField] private List<string> connectorTags;
    [SerializeField] private bool connectToOwnTag = true;

    [Separator] [SerializeField] private float rayLength = 1f;

    private readonly Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    [HideInInspector] public bool UpdateOnStart = true;

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

        if (connectToOwnTag) connectorTags.Add(transform.tag);

        if (TryGetComponent(out spriteRenderer)) hasSpriteRenderer = true;
    }

    private void Start()
    {
        // get components if not already cached
        lineRenderers ??= GetComponentsInChildren<LineRenderer>();

        UpdateAlpha();

        if (UpdateOnStart) UpdateOutline(true);
    }

    private void Update() => UpdateAlpha();

    public void UpdateOutline(bool updateNeighbor = false)
    {
        // debug stuff so not important
        if (Dbg.Instance.Enabled && !Dbg.Instance.WallOutlines) return;

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
        if (Dbg.Instance.Enabled && !Dbg.Instance.WallOutlines) return;

        ClearLineInDirection(dir);

        if (IsConnectorInDirection(dir, updateNeighbor, true)) return;

        DrawLine(dir);
    }

    private void DrawLine(Vector2 dir)
    {
        // draw settings
        DrawManager.SetWeight(weight);
        DrawManager.SetFill(color);
        DrawManager.SetLayerID(DrawManager.OutlineLayerID);
        DrawManager.SetRoundedCorners(false);

        Transform t = transform;
        Vector2 localPosition = t.localPosition;
        Vector2 localScale = t.localScale;
        float halfWidth = localScale.x / 2;
        float halfHeight = localScale.y / 2;
        float halfWeight = weight / 2;

        if (dir.Equals(Vector2.up) || dir.Equals(Vector2.down))
        {
            // get left & right hits to fill in gaps in inner corners
            bool left = IsConnectorInDirection(Vector2.left);
            bool right = IsConnectorInDirection(Vector2.right);

            DrawManager.DrawLine(
                localPosition.x - halfWidth - (left ? weight : 0),
                localPosition.y + dir.y * 0.5f - dir.y * halfWeight,
                localPosition.x + halfWidth + (right ? weight : 0),
                localPosition.y + dir.y * 0.5f - dir.y * halfWeight,
                lineContainer.transform
            );
        }
        else if (dir.Equals(Vector2.left) || dir.Equals(Vector2.right))
        {
            DrawManager.DrawLine(
                localPosition.x + dir.x * 0.5f - dir.x * halfWeight,
                localPosition.y + halfHeight,
                localPosition.x + dir.x * 0.5f - dir.x * halfWeight,
                localPosition.y - halfHeight,
                lineContainer.transform
            );
        }

        lineRenderers = GetComponentsInChildren<LineRenderer>();
    }

    private bool IsConnectorInDirection(Vector2 direction, bool updateNeighbor = false, bool drawRay = false)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, rayLength);

        if (drawRay && Dbg.Instance.DrawRays) Debug.DrawRay(transform.position, direction, Color.red, 20);

        foreach (RaycastHit2D r in hits)
        {
            FieldOutline outlineNeighbor = r.collider.gameObject.GetComponent<FieldOutline>();
            if (updateNeighbor && outlineNeighbor != null) outlineNeighbor.UpdateOutline();

            if (!connectorTags.Contains(r.collider.tag)) continue;
            // if (!r.collider.CompareTag(gameObject.tag)) continue;

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

    private void ClearLineInDirection(Vector2 dir)
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