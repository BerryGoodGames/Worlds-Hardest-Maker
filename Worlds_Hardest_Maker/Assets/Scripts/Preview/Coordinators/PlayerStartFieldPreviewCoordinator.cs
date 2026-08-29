using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerStartFieldPreviewCoordinator : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private FillPreviewCoordinator startFieldPreviewPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform container;
    [SerializeField] [PositiveValueOnly] private float followSpeed = 20;

    private readonly List<PreviewSlot> previewPool = new();

    [Inject] private IObjectResolver diContainer;
    [Inject] private IMouseService mouseService;
    [Inject] private PlayerPlacementRules placementRules;
    [Inject] private PreviewVisibilityRulesService visibilityRulesService;

    private void Update()
    {
        if (LevelSessionEditManager.Instance.IsPlaying)
        {
            HideAll();
            return;
        }

        Vector2 mouseGridPos = mouseService.MouseWorldPosGrid;

        if (!ShouldShowPreview(mouseGridPos))
        {
            HideAll();
            return;
        }

        ShowAt(GetCorners(mouseGridPos));
    }

    private bool ShouldShowPreview(Vector2 mouseGridPos)
    {
        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;

        if (currentEditMode != EditModeManager.Player) return false;
        if (!visibilityRulesService.IsPreviewVisible(currentEditMode)) return false;

        return placementRules.GetAutoPlacedStartFieldPositions(mouseGridPos, PlaceManager.GetCurrentSheet()).Any();
    }

    private static Vector2Int[] GetCorners(Vector2 position)
    {
        return new[]
        {
            new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y)),
            new Vector2Int(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new Vector2Int(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            new Vector2Int(Mathf.CeilToInt(position.x), Mathf.CeilToInt(position.y)),
        };
    }

    private void ShowAt(IReadOnlyList<Vector2Int> positions)
    {
        EnsurePoolCapacity(positions.Count);

        for (int i = 0; i < previewPool.Count; i++)
        {
            bool active = i < positions.Count;
            bool wasActive = previewPool[i].Preview.gameObject.activeSelf;

            previewPool[i].Preview.gameObject.SetActive(active);

            if (!active) continue;

            if (wasActive) previewPool[i].FollowComp.SetTarget(positions[i]);
            else previewPool[i].FollowComp.SnapTo(positions[i]);
        }
    }

    private void HideAll()
    {
        foreach (PreviewSlot slot in previewPool) slot.Preview.gameObject.SetActive(false);
    }

    private void EnsurePoolCapacity(int count)
    {
        while (previewPool.Count < count)
        {
            FillPreviewCoordinator preview = Instantiate(startFieldPreviewPrefab, container);
            
            diContainer.InjectGameObject(preview.gameObject);
            
            preview.SetSprite(EditModeManager.Start);
            
            SmoothPositionFollow follower = preview.gameObject.GetOrAddComponent<SmoothPositionFollow>();
            follower.Speed = followSpeed;
            follower.SnapTo(mouseService.MouseWorldPosGrid);
            
            preview.gameObject.SetActive(false);
            previewPool.Add(new(preview, follower));
        }
    }
    
    private readonly struct PreviewSlot
    {
        public readonly FillPreviewCoordinator Preview;
        public readonly SmoothPositionFollow FollowComp;

        public PreviewSlot(FillPreviewCoordinator preview, SmoothPositionFollow followComp)
        {
            Preview = preview;
            FollowComp = followComp;
        }
    }
}