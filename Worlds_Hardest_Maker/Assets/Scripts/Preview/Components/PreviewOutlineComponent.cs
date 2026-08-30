using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PreviewOutlineComponent : MonoBehaviour
{
    private Transform lineContainer;

    private Transform LineContainer
    {
        get
        {
            if (lineContainer == null)
            {
                GameObject container = new("OutlineLineContainer")
                {
                    transform = { parent = transform, localPosition = Vector2.zero, },
                };
                lineContainer = container.transform;
            }
            return lineContainer;
        }
    }

    private readonly struct OutlineSegment
    {
        public readonly Vector2 Direction;
        public readonly bool LeftConnected;
        public readonly bool RightConnected;

        public OutlineSegment(Vector2 direction, bool leftConnected, bool rightConnected)
        {
            Direction = direction;
            LeftConnected = leftConnected;
            RightConnected = rightConnected;
        }
    }

    private IOutlineConnectivityProvider batchProvider = NullOutlineConnectivityProvider.Instance;

    private readonly List<OutlineSegment> segments = new();
    private PreviewOutlineData currentData;
    private bool hasTopology;
    private Vector2 lastTopologyPosition;
    private EditMode lastTopologyEditMode;
    private Vector2 lastDrawnPosition;

    private FollowMouse followMouse;
    private bool triedGetFollowMouse;

    private FollowMouse FollowMouseComponent
    {
        get
        {
            if (!triedGetFollowMouse)
            {
                TryGetComponent(out followMouse);
                triedGetFollowMouse = true;
            }
            return followMouse;
        }
    }
    
    private EditMode fixedEditMode;
    private bool hasFixedEditMode;

    /// <summary>
    ///     Locks this outline to always use the given edit mode instead of tracking
    ///     LevelSessionEditManager.Instance.CurrentEditMode every frame. Use this for
    ///     previews whose type is fixed (paste, fill) so they don't reconnect to whatever
    ///     tool happens to be selected in the toolbar.
    /// </summary>
    public void SetFixedEditMode(EditMode editMode)
    {
        fixedEditMode = editMode;
        hasFixedEditMode = true;
    }

    private EditMode GetTrackedEditMode() =>
        hasFixedEditMode ? fixedEditMode : LevelSessionEditManager.Instance.CurrentEditMode;

    public void UpdateOutline() => UpdateOutline(GetTrackedEditMode());

    [Inject] private IDrawService drawService;
    [Inject] private PreviewOutlineDataProvider dataProvider;
    [Inject] private SceneOutlineConnectivityProvider sceneConnectivityProvider;

    public void SetBatchProvider(IOutlineConnectivityProvider provider) =>
        batchProvider = provider ?? NullOutlineConnectivityProvider.Instance;

    public void UpdateOutline(EditMode editMode)
    {
        RecomputeTopology(editMode, GetTopologyPosition());
        DrawSegments(transform.position);
    }

    /// <summary>
    ///     The position connectivity should be evaluated against. For a preview that follows
    ///     the mouse this is the destination cell it's tweening toward, not wherever it
    ///     currently sits mid-animation — connectivity raycasts only make sense at grid-aligned
    ///     positions. Static ghosts (fill/paste previews) have no FollowMouse and just use their
    ///     own (unmoving) position.
    /// </summary>
    private Vector2 GetTopologyPosition()
    {
        FollowMouse follow = FollowMouseComponent;
        return follow != null ? follow.TargetPosition : transform.position;
    }

    private void RecomputeTopology(EditMode editMode, Vector2 position)
    {
        hasTopology = true;
        lastTopologyEditMode = editMode;
        lastTopologyPosition = position;

        segments.Clear();

        ISheet sheet = PlaceManager.GetCurrentSheet();
        currentData = dataProvider.GetOutlineData(editMode, position, sheet);
        if (!currentData.Enabled) return;

        foreach (Vector2 dir in OutlineGeometry.Directions)
        {
            if (HasConnector(position, dir, currentData.ConnectorTags, sheet)) continue;

            bool leftConnected = false;
            bool rightConnected = false;

            if (dir.Equals(Vector2.up) || dir.Equals(Vector2.down))
            {
                leftConnected = HasConnector(position, Vector2.left, currentData.ConnectorTags, sheet);
                rightConnected = HasConnector(position, Vector2.right, currentData.ConnectorTags, sheet);
            }

            segments.Add(new(dir, leftConnected, rightConnected));
        }
    }

    /// <summary>
    ///     Redraws the already-decided segments at the given (possibly mid-tween) position, so
    ///     the outline always visually tracks the sprite instead of freezing between topology
    ///     updates.
    /// </summary>
    private void DrawSegments(Vector2 position)
    {
        lastDrawnPosition = position;

        ClearLines();
        if (!hasTopology || !currentData.Enabled) return;

        Vector2 localScale = transform.localScale;

        foreach (OutlineSegment segment in segments)
        {
            drawService.SetWeight(currentData.Weight);
            drawService.SetFill(currentData.Color);
            drawService.SetLayerName(LayerManager.Instance.SortingLayers.Outline);
            drawService.SetRoundedCorners(false);

            (Vector2 start, Vector2 end) = OutlineGeometry.GetLinePoints(
                position, localScale, currentData.Weight, segment.Direction, segment.LeftConnected, segment.RightConnected
            );

            LineRenderer line = drawService.DrawLine(start.x, start.y, end.x, end.y, LineContainer);
            line.useWorldSpace = true;
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
        EditMode currentEditMode = GetTrackedEditMode(); // <-- was LevelSessionEditManager.Instance.CurrentEditMode directly
        Vector2 topologyPosition = GetTopologyPosition();
        Vector2 currentPosition = transform.position;

        bool topologyChanged = !hasTopology || topologyPosition != lastTopologyPosition || currentEditMode != lastTopologyEditMode;
        if (topologyChanged) RecomputeTopology(currentEditMode, topologyPosition);

        if (topologyChanged || currentPosition != lastDrawnPosition) DrawSegments(currentPosition);
    }
}