using DG.Tweening;
using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
///     Component that applies rotation to preview based on edit mode.
///     Supports both immediate and smooth rotation.
/// </summary>
public class PreviewRotationComponent : MonoBehaviour, IPreviewDataReceiver
{
    public bool RotateToEditRotation = true;
    [Space] [SerializeField] private bool smoothRotation;    
    [SerializeField] [ConditionalField(nameof(smoothRotation))] [PositiveValueOnly] private float rotateDuration;
    
    private EditMode lastEditMode;
    
    private EventBus eventBus;
    private IPreviewRotationService rotationService;
    
    [Inject]
    private void Construct(EventBus eventBus, IPreviewRotationService rotationService)
    {
        this.eventBus = eventBus;
        this.rotationService = rotationService;
        
        eventBus.Subscribe<EditRotationChangeEvent>(OnEditRotationChange);
    }
    
    private void OnEditRotationChange(EditRotationChangeEvent evt) => UpdateRotation();
    
    public void UpdateRotation(bool resetRotation = false, bool smooth = true)
    {
        print((resetRotation, smooth, RotateToEditRotation, smoothRotation));
        if (resetRotation)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -90);
            return;
        }
        
        if (!RotateToEditRotation) return;
        
        Quaternion rotation = rotationService.GetTargetRotation(true);
        
        if (smoothRotation && smooth)
        {
            transform.DOKill();
            transform.DORotateQuaternion(rotation, rotateDuration)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }
        else transform.localRotation = rotation;
    }
    
    public void ApplyPreviewData(PreviewData previewData)
    {
        // Store the current edit mode for later use
        lastEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        
        bool resetRotation = !previewData.ShouldRotate;
        UpdateRotation(resetRotation);
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<EditRotationChangeEvent>(OnEditRotationChange);
    }
}