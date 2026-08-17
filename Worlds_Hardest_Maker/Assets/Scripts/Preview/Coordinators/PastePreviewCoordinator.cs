using MyBox;
using UnityEngine;
using WorldsHardestMaker.CopyPaste;

[RequireComponent(typeof(PreviewSpriteComponent))]
[RequireComponent(typeof(PreviewRotationComponent))]
public class PastePreviewCoordinator : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private PreviewSpriteComponent spriteComponent;
    
    public void ApplyCopyData(CopyData data)
    {
        EditMode dataEditMode = data.Data.GetEditMode();
        
        spriteComponent.SetSprite(dataEditMode);
    }
}
