using DG.Tweening;
using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
///     Component that applies rotation to preview based on edit mode.
///     Supports both immediate and smooth rotation.
/// </summary>
public class PreviewRotationComponent : MonoBehaviour, IPreviewDataReceiver<PreviewRotationData>
{
    public bool RotateToEditRotation = true;
    [Space] [SerializeField] private bool smoothRotation;    
    [SerializeField] [ConditionalField(nameof(smoothRotation))] [PositiveValueOnly] private float rotateDuration;
    
    private EventBus eventBus;
    private IPreviewRotationDataProvider previewRotationDataProvider;
    
    [Inject]
    private void Construct(EventBus eventBus, IPreviewRotationDataProvider previewRotationDataProvider)
    {
        this.eventBus = eventBus;
        this.previewRotationDataProvider = previewRotationDataProvider;
        
        eventBus.Subscribe<EditModeInitializedEvent>(OnEditModeInitialized);
        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Subscribe<EditRotationChangeEvent>(OnEditRotationChange);
    }
    
    private void OnEditModeInitialized(EditModeInitializedEvent evt) => UpdateRotation();
    private void OnEditModeChange(EditModeChangeEvent evt) => UpdateRotation();
    private void OnEditRotationChange(EditRotationChangeEvent evt) => UpdateRotation();
    
    private void UpdateRotation()
    {
        EditMode editMode = LevelSessionEditManager.Instance.CurrentEditMode;
        PreviewRotationData previewRotationData = previewRotationDataProvider.GetPreviewRotationData(editMode);
        
        ApplyPreviewData(previewRotationData);
    }
    
    public void ApplyPreviewData(PreviewRotationData previewRotationData)
    {
        if (previewRotationData.ResetRotation)
        {
            transform.localRotation = Quaternion.identity;
            return;
        }
        
        if (!RotateToEditRotation) return;
        
        Quaternion rotation = previewRotationData.TargetRotation;
        
        if (smoothRotation)
        {
            transform.DOKill();
            transform.DORotateQuaternion(rotation, rotateDuration)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }
        else transform.localRotation = rotation;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<EditModeInitializedEvent>(OnEditModeInitialized);
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<EditRotationChangeEvent>(OnEditRotationChange);
    }
}