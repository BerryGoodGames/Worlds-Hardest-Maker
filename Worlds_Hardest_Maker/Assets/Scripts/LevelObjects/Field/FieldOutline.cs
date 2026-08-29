using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
///     Attach to every field prefab variant which has outlines (see TypesWithOutlines)
/// </summary>
public class FieldOutline : MonoBehaviour
{
    [SerializeField] private Color color = Color.black;
    [SerializeField] private bool imitateAlpha;
    [SerializeField] private float weight = 0.1f;
    
    [Separator] [SerializeField] private List<string> connectorTags;
    [SerializeField] private bool connectToOwnTag = true;
    
    private readonly Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right, };
    
    [HideInInspector] public bool UpdateOnStart = true;
    
    private GameObject lineContainer;
    private SpriteRenderer spriteRenderer;
    private bool hasSpriteRenderer;
    
    [HideInInspector] public LineRenderer[] LineRenderers;
    
    private ISheet Sheet => SheetUtils.ResolveFor(this);

    public event Action OnUpdateOutline = () => { };

    [Inject] private IDrawService drawService;

    private void Awake()
    {
        // create line container which has this transform as parent
        lineContainer = new("LineContainer")
        {
            transform =
            {
                parent = transform,
                localPosition = Vector2.zero,
            },
        };
    
        if (connectToOwnTag) connectorTags.Add(transform.tag);
    
        if (TryGetComponent(out spriteRenderer)) hasSpriteRenderer = true;
    }

    private void Start()
    {
        // get components if not already cached
        LineRenderers ??= GetComponentsInChildren<LineRenderer>();
    
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
        
        OnUpdateOutline.Invoke();
    }
    
    public void UpdateOutline(Vector2 dir, bool updateNeighbor = false)
    {
        // debug stuff so not important
        if (Dbg.Instance.Enabled && !Dbg.Instance.WallOutlines) return;
        
        ClearLineInDirection(dir);
        
        if (!IsConnectorInDirection(dir, updateNeighbor, true)) DrawLine(dir);
        
        OnUpdateOutline.Invoke();
    }
    
    private void DrawLine(Vector2 dir)
    {
        // draw settings
        drawService.SetWeight(weight);
        drawService.SetFill(color);
        drawService.SetLayerName(LayerManager.Instance.SortingLayers.Outline);
        drawService.SetRoundedCorners(false);
        
        Transform t = transform;
        Vector2 position = t.position;
        Vector2 localScale = t.localScale;
        float halfWidth = localScale.x / 2;
        float halfHeight = localScale.y / 2;
        float halfWeight = weight / 2;
        
        if (dir.Equals(Vector2.up) || dir.Equals(Vector2.down))
        {
            // get left & right hits to fill in gaps in inner corners
            bool left = IsConnectorInDirection(Vector2.left);
            bool right = IsConnectorInDirection(Vector2.right);
            
            LineRenderer line = drawService.DrawLine(
                position.x - halfWidth - (left ? weight : 0),
                position.y + dir.y * 0.5f - dir.y * halfWeight,
                position.x + halfWidth + (right ? weight : 0),
                position.y + dir.y * 0.5f - dir.y * halfWeight,
                lineContainer.transform
            );
            
            line.useWorldSpace = false;
        }
        else if (dir.Equals(Vector2.left) || dir.Equals(Vector2.right))
        {
            LineRenderer line = drawService.DrawLine(
                position.x + dir.x * 0.5f - dir.x * halfWeight,
                position.y + halfHeight,
                position.x + dir.x * 0.5f - dir.x * halfWeight,
                position.y - halfHeight,
                lineContainer.transform
            );
            
            line.useWorldSpace = false;
        }
        
        LineRenderers = GetComponentsInChildren<LineRenderer>();
    }
    
    private bool IsConnectorInDirection(Vector2 direction, bool updateNeighbor = false, bool drawRay = false)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 1);
        
        if (drawRay && Dbg.Instance.DrawRays) Debug.DrawRay(transform.position, direction, Color.red, 20);
        
        foreach (RaycastHit2D r in hits)
        {
            if (!SheetUtils.Exists(r.collider, Sheet)) continue;
            
            if (updateNeighbor && r.transform.TryGetComponent(out FieldOutline outlineNeighbor)) outlineNeighbor.UpdateOutline();
            
            if (!connectorTags.Contains(r.collider.tag)
                || !SheetUtils.Exists(r.collider, Sheet)) continue;
            
            return true;
        }
        
        return false;
    }
    
    private void ClearLines()
    {
        foreach (Transform child in lineContainer.transform) Destroy(child.gameObject);
        LineRenderers = Array.Empty<LineRenderer>();
    }
    
    private void ClearLineInDirection(Vector2 dir)
    {
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
        foreach (LineRenderer line in LineRenderers)
        {
            if (line == null) continue;
            
            line.startColor = color;
            line.endColor = color;
        }
    }
}