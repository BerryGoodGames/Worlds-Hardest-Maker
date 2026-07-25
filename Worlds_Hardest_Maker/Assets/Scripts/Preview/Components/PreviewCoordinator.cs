using UnityEngine;
using VContainer;

/// <summary>
///     Orchestrator component that coordinates all preview sub-components.
///     Receives events and distributes preview data to all receiving components.
///     
///     Attach this to the same GameObject that has all other preview components.
/// </summary>
[RequireComponent(typeof(PreviewSpriteComponent))]
[RequireComponent(typeof(PreviewRotationComponent))]
[RequireComponent(typeof(PreviewAnimationComponent))]
[RequireComponent(typeof(PreviewFollowMouseComponent))]
[RequireComponent(typeof(PreviewEventComponent))]
public class PreviewCoordinator : MonoBehaviour
{
    private PreviewSpriteComponent spriteComponent;
    private PreviewRotationComponent rotationComponent;
    
    private EventBus eventBus;
    private IEditModePreviewProvider previewProvider;

    [SerializeField] private bool logDebugInfo = false;

    [Inject]
    private void Construct(EventBus eventBus, IEditModePreviewProvider previewProvider)
    {
        this.eventBus = eventBus;
        this.previewProvider = previewProvider;
    }

    private void Start()
    {
        spriteComponent = GetComponent<PreviewSpriteComponent>();
        rotationComponent = GetComponent<PreviewRotationComponent>();

        if (spriteComponent == null) Debug.LogError("PreviewCoordinator requires PreviewSpriteComponent", gameObject);
        if (rotationComponent == null) Debug.LogError("PreviewCoordinator requires PreviewRotationComponent", gameObject);

        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChanged);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }

    private void OnEditModeChanged(EditModeChangeEvent evt)
    {
        if (logDebugInfo)
        {
            Debug.Log($"[PreviewCoordinator] Edit mode changed to: {evt.NewEditMode.name}");
        }

        UpdatePreviewData();
    }

    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        if (logDebugInfo)
        {
            Debug.Log("[PreviewCoordinator] Switched to edit mode");
        }

        UpdatePreviewData();
    }

    /// <summary>
    ///     Gets current preview data and distributes it to all receiver components.
    /// </summary>
    private void UpdatePreviewData()
    {
        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        float alpha = spriteComponent.GetAlpha();
        bool forceShow = spriteComponent.ShowSpriteWhenPasting;

        // Get preview data from service
        PreviewData previewData = previewProvider.GetPreviewData(currentEditMode, alpha, forceShow);

        // Distribute to components that implement IPreviewDataReceiver
        DistributePreviewData(previewData);
    }

    private void DistributePreviewData(PreviewData previewData)
    {
        if (spriteComponent != null)
        {
            spriteComponent.ApplyPreviewData(previewData);
        }
        
        if (rotationComponent != null)
        {
            rotationComponent.ApplyPreviewData(previewData);
        }

        if (logDebugInfo)
        {
            Debug.Log(
                $"[PreviewCoordinator] Distributed preview data - Sprite: {previewData.Sprite?.name ?? "null"}, ShouldRotate: {previewData.ShouldRotate}"
            );
        }
    }

    private void OnDestroy()
    {
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChanged);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
}
