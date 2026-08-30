using MyBox;
using UnityEngine;
using WorldsHardestMaker.CopyPaste;

[RequireComponent(typeof(PreviewSpriteComponent))]
[RequireComponent(typeof(PreviewOutlineComponent))]
public class PastePreviewCoordinator : MonoBehaviour
{
    private PreviewSpriteComponent spriteComponent;
    private PreviewOutlineComponent outlineComponent;

    private void Awake()
    {
        spriteComponent = GetComponent<PreviewSpriteComponent>();
        outlineComponent = gameObject.GetOrAddComponent<PreviewOutlineComponent>();
    }
    
    public void ApplyCopyData(CopyData data)
    {
        EditMode dataEditMode = data.Data.GetEditMode();
        spriteComponent.SetSprite(dataEditMode);
        outlineComponent.SetFixedEditMode(dataEditMode);
    }

    public void SetOutlineBatchProvider(IOutlineConnectivityProvider batchProvider) => outlineComponent.SetBatchProvider(batchProvider);
    public void UpdateOutline(EditMode editMode) => outlineComponent.UpdateOutline(editMode);
}