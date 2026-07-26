using MyBox;
using UnityEngine;

[RequireComponent(typeof(PreviewSpriteComponent))]
[RequireComponent(typeof(PreviewRotationComponent))]
public class FillPreviewCoordinator : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private PreviewSpriteComponent spriteComponent;
    [SerializeField] [InitializationField] [MustBeAssigned] private PreviewRotationComponent rotationComponent;
    
    public void UpdateSprite()
    {
        spriteComponent.UpdateSprite();
    }
    
    public void UpdateRotation()
    {
        rotationComponent.UpdateRotation();
    }
}