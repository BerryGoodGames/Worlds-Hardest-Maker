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
    [Inject] private PreviewVisibilityRulesService visibilityService;

    public void SetBatchProvider(IOutlineConnectivityProvider provider) =>
        batchProvider = provider ?? NullOutlineConnectivityProvider.Instance;

    public void UpdateOutline(EditMode editMode)
    {
        RecomputeTopology(editMode, GetTopologyPosition());
        DrawSegments(transform.position);
    }

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
        EditMode currentEditMode = GetTrackedEditMode();

        // respect the same rules the sprite/animation preview already follows
        if (!visibilityService.IsPreviewVisible(currentEditMode))
        {
            if (hasTopology || LineContainerHasChildren()) ClearLines();
            hasTopology = false; // force a fresh recompute next time it becomes visible
            return;
        }

        Vector2 topologyPosition = GetTopologyPosition();
        Vector2 currentPosition = transform.position;

        bool topologyChanged = !hasTopology || topologyPosition != lastTopologyPosition || currentEditMode != lastTopologyEditMode;
        if (topologyChanged) RecomputeTopology(currentEditMode, topologyPosition);

        if (topologyChanged || currentPosition != lastDrawnPosition) DrawSegments(currentPosition);
    }
    
    private bool LineContainerHasChildren() => lineContainer != null && lineContainer.childCount > 0;
}