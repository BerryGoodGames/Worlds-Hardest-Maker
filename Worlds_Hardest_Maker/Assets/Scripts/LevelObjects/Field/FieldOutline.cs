// File: LevelObjects/Field/FieldOutline.cs
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

    [HideInInspector] public bool UpdateOnStart = true;

    private GameObject lineContainer;
    private SpriteRenderer spriteRenderer;
    private bool hasSpriteRenderer;

    [HideInInspector] public LineRenderer[] LineRenderers;

    private ISheet Sheet => SheetUtils.ResolveFor(this);

    public event Action OnUpdateOutline = () => { };

    [Inject] private IDrawService drawService;
    private IOutlineConnectivityProvider connectivityProvider;

    [Inject]
    private void Construct(SceneOutlineConnectivityProvider sceneConnectivityProvider)
    {
        connectivityProvider = sceneConnectivityProvider;
    }

    /// <summary>
    ///     Tags this outline connects to, computed without mutating the serialized list
    ///     (readable even on a prefab asset that never ran Awake — used by preview outlines
    ///     to mirror a real prefab's outline settings).
    /// </summary>
    public IReadOnlyList<string> GetConnectorTags()
    {
        if (!connectToOwnTag) return connectorTags;

        List<string> tags = new(connectorTags) { tag, };
        return tags;
    }

    public Color Color => color;
    public float Weight => weight;

    private void Awake()
    {
        lineContainer = new("LineContainer")
        {
            transform =
            {
                parent = transform,
                localPosition = Vector2.zero,
            },
        };

        if (TryGetComponent(out spriteRenderer)) hasSpriteRenderer = true;
    }

    private void Start()
    {
        LineRenderers ??= GetComponentsInChildren<LineRenderer>();

        UpdateAlpha();

        if (UpdateOnStart) UpdateOutline(true);
    }

    private void Update() => UpdateAlpha();

    public void UpdateOutline(bool updateNeighbor = false)
    {
        if (Dbg.Instance.Enabled && !Dbg.Instance.WallOutlines) return;

        ClearLines();

        IReadOnlyList<string> tags = GetConnectorTags();

        foreach (Vector2 dir in OutlineGeometry.Directions)
        {
            if (updateNeighbor) connectivityProvider.NotifyNeighbors(transform.position, dir, Sheet);

            if (connectivityProvider.HasConnector(transform.position, dir, tags, Sheet)) continue;

            DrawLine(dir, tags);
        }

        OnUpdateOutline.Invoke();
    }

    public void UpdateOutline(Vector2 dir, bool updateNeighbor = false)
    {
        if (Dbg.Instance.Enabled && !Dbg.Instance.WallOutlines) return;

        ClearLineInDirection(dir);

        IReadOnlyList<string> tags = GetConnectorTags();

        if (updateNeighbor) connectivityProvider.NotifyNeighbors(transform.position, dir, Sheet);

        if (!connectivityProvider.HasConnector(transform.position, dir, tags, Sheet)) DrawLine(dir, tags);

        OnUpdateOutline.Invoke();
    }

    private void DrawLine(Vector2 dir, IReadOnlyList<string> tags)
    {
        drawService.SetWeight(weight);
        drawService.SetFill(color);
        drawService.SetLayerName(LayerManager.Instance.SortingLayers.Outline);
        drawService.SetRoundedCorners(false);

        Transform t = transform;
        Vector2 position = t.position;
        Vector2 localScale = t.localScale;

        bool leftConnected = false;
        bool rightConnected = false;

        if (dir.Equals(Vector2.up) || dir.Equals(Vector2.down))
        {
            leftConnected = connectivityProvider.HasConnector(position, Vector2.left, tags, Sheet);
            rightConnected = connectivityProvider.HasConnector(position, Vector2.right, tags, Sheet);
        }

        (Vector2 start, Vector2 end) = OutlineGeometry.GetLinePoints(position, localScale, weight, dir, leftConnected, rightConnected);

        LineRenderer line = drawService.DrawLine(start.x, start.y, end.x, end.y, lineContainer.transform);
        line.useWorldSpace = false;

        LineRenderers = GetComponentsInChildren<LineRenderer>();
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