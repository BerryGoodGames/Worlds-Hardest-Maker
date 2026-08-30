// File: Preview/Components/PreviewOutlineComponent.cs
using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
///     Draws live outlines on a preview ghost, using the same geometry as real placed fields.
///     Connectivity is resolved against the real scene plus an optional batch of sibling ghosts
///     (set via <see cref="SetBatchProvider"/>), instead of colliders that don't exist yet.
/// </summary>
public class PreviewOutlineComponent : MonoBehaviour
{
    private Transform lineContainer;

    /// <summary>
    ///     Lazily created on first use rather than in Awake. Awake is deferred by Unity when an
    ///     object is instantiated under a currently-inactive parent, and preview ghosts are
    ///     sometimes spawned and immediately configured before their container is activated —
    ///     so this must not assume Awake has already run.
    /// </summary>
    private Transform LineContainer
    {
        get
        {
            if (lineContainer == null)
            {
                GameObject container = new("OutlineLineContainer")
                {
                    transform =
                    {
                        parent = transform,
                        localPosition = Vector2.zero,
                    },
                };

                lineContainer = container.transform;
            }

            return lineContainer;
        }
    }

    private IOutlineConnectivityProvider batchProvider = NullOutlineConnectivityProvider.Instance;

    private Vector2 lastPosition;
    private EditMode lastEditMode;
    private bool hasRenderedOnce;

    [Inject] private IDrawService drawService;
    [Inject] private PreviewOutlineDataProvider dataProvider;
    [Inject] private SceneOutlineConnectivityProvider sceneConnectivityProvider;

    public void SetBatchProvider(IOutlineConnectivityProvider provider)
    {
        batchProvider = provider ?? NullOutlineConnectivityProvider.Instance;
    }

    public void UpdateOutline() => UpdateOutline(LevelSessionEditManager.Instance.CurrentEditMode);

    public void UpdateOutline(EditMode editMode)
    {
        hasRenderedOnce = true;
        lastPosition = transform.position;
        lastEditMode = editMode;

        ClearLines();

        PreviewOutlineData data = dataProvider.GetOutlineData(editMode);
        if (!data.Enabled) return;

        Vector2 position = transform.position;
        Vector2 localScale = transform.localScale;
        ISheet sheet = PlaceManager.GetCurrentSheet();

        foreach (Vector2 dir in OutlineGeometry.Directions)
        {
            if (HasConnector(position, dir, data.ConnectorTags, sheet)) continue;

            bool leftConnected = false;
            bool rightConnected = false;

            if (dir.Equals(Vector2.up) || dir.Equals(Vector2.down))
            {
                leftConnected = HasConnector(position, Vector2.left, data.ConnectorTags, sheet);
                rightConnected = HasConnector(position, Vector2.right, data.ConnectorTags, sheet);
            }

            drawService.SetWeight(data.Weight);
            drawService.SetFill(data.Color);
            drawService.SetLayerName(LayerManager.Instance.SortingLayers.Outline);
            drawService.SetRoundedCorners(false);

            (Vector2 start, Vector2 end) = OutlineGeometry.GetLinePoints(position, localScale, data.Weight, dir, leftConnected, rightConnected);

            LineRenderer line = drawService.DrawLine(start.x, start.y, end.x, end.y, LineContainer);
            line.useWorldSpace = false;
        }
    }

    private bool HasConnector(Vector2 position, Vector2 direction, IReadOnlyList<string> tags, ISheet sheet)
    {
        return sceneConnectivityProvider.HasConnector(position, direction, tags, sheet)
               || batchProvider.HasConnector(position, direction, tags, sheet);
    }

    private void ClearLines()
    {
        foreach (Transform child in LineContainer) Destroy(child.gameObject);
    }

    private void Update()
    {
        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        Vector2 currentPosition = transform.position;

        if (hasRenderedOnce && currentPosition == lastPosition && currentEditMode == lastEditMode) return;

        UpdateOutline(currentEditMode);
    }
}