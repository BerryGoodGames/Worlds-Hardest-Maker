// File: Preview/Coordinators/PastePreviewCoordinator.cs
using MyBox;
using UnityEngine;
using WorldsHardestMaker.CopyPaste;

public class PastePreviewCoordinator : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private PreviewSpriteComponent spriteComponent;

    private PreviewOutlineComponent outlineComponent;

    private void Awake()
    {
        // Self-heal rather than depend on every prefab variant having been manually
        // re-saved in the Editor after PreviewOutlineComponent was introduced.
        outlineComponent = gameObject.GetOrAddComponent<PreviewOutlineComponent>();
    }

    public void ApplyCopyData(CopyData data)
    {
        EditMode dataEditMode = data.Data.GetEditMode();
        spriteComponent.SetSprite(dataEditMode);
    }

    public void SetOutlineBatchProvider(IOutlineConnectivityProvider batchProvider) => outlineComponent.SetBatchProvider(batchProvider);
    public void UpdateOutline(EditMode editMode) => outlineComponent.UpdateOutline(editMode);
}