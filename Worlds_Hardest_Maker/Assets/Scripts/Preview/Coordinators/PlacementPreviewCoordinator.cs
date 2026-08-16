using MyBox;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

/// <summary>
///     Handles event-driven state changes for placement preview.
///     Responds to edit mode transitions and activation events.
/// </summary>
[RequireComponent(typeof(PreviewSpriteComponent))]
[RequireComponent(typeof(PreviewRotationComponent))]
[RequireComponent(typeof(PreviewAnimationComponent))]
[RequireComponent(typeof(PreviewFollowMouseComponent))]
public class PlacementPreviewCoordinator : MonoBehaviour
{
    [SerializeField] [MustBeAssigned] [InitializationField] private PreviewSpriteComponent spriteComponent;
    [SerializeField] [MustBeAssigned] [InitializationField] private PreviewRotationComponent rotationComponent;
    [SerializeField] [MustBeAssigned] [InitializationField] private PreviewAnimationComponent animationComponent;
    [SerializeField] [MustBeAssigned] [InitializationField] private PreviewFollowMouseComponent followMouseComponent;
    
    [Inject] private EventBus eventBus;
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        Hide();
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        Show();
        
        spriteComponent.UpdateSprite();
    }
    
    private void OnEditModeInitialized(EditModeInitializedEvent evt)
    {
        spriteComponent.UpdateSprite();
        rotationComponent.UpdateRotation();
    }
    
    private void OnEditModeChange(EditModeChangeEvent evt)
    {
        spriteComponent.UpdateSprite();
        rotationComponent.UpdateRotation();
    }

    private void OnSelectionStartedEvent(SelectionStartedEvent evt)
    {
        Hide();
    }

    private void OnSelectionClearedEvent(SelectionClearedEvent evt)
    {
        Show();
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void Start()
    {
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<EditModeInitializedEvent>(OnEditModeInitialized);
        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Subscribe<SelectionStartedEvent>(OnSelectionStartedEvent);
        eventBus.Subscribe<SelectionClearedEvent>(OnSelectionClearedEvent);
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<EditModeInitializedEvent>(OnEditModeInitialized);
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<SelectionStartedEvent>(OnSelectionStartedEvent);
        eventBus.Unsubscribe<SelectionClearedEvent>(OnSelectionClearedEvent);
    }
}