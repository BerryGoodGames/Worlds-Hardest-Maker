// File: Preview/Coordinators/FillPreviewCoordinator.cs
using MyBox;
using UnityEngine;

[RequireComponent(typeof(PreviewSpriteComponent))]
[RequireComponent(typeof(PreviewRotationComponent))]
[RequireComponent(typeof(PreviewOutlineComponent))]
public class FillPreviewCoordinator : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private PreviewSpriteComponent spriteComponent;
    [SerializeField] [InitializationField] [MustBeAssigned] private PreviewRotationComponent rotationComponent;

    private PreviewOutlineComponent outlineComponent;

    private void Awake()
    {
        outlineComponent = gameObject.GetOrAddComponent<PreviewOutlineComponent>();
    }

    public void UpdateSprite() => spriteComponent.UpdateSprite();
    public void UpdateRotation() => rotationComponent.UpdateRotation();
    public void SetSprite(EditMode editMode) => spriteComponent.SetSprite(editMode);

    public void SetOutlineBatchProvider(IOutlineConnectivityProvider batchProvider) => outlineComponent.SetBatchProvider(batchProvider);
    public void UpdateOutline() => outlineComponent.UpdateOutline();
}