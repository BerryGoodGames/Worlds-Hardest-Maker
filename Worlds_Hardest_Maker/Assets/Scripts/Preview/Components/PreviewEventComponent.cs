using UnityEngine;
using VContainer;

/// <summary>
///     Handles event-driven state changes for placement preview.
///     Responds to edit mode transitions and activation events.
/// </summary>
public class PreviewEventComponent : MonoBehaviour
{
    [Inject] private EventBus eventBus;
    private PreviewFollowMouseComponent followMouseComponent;

    private void Start()
    {
        followMouseComponent = GetComponent<PreviewFollowMouseComponent>();
    }

    protected virtual void OnEnable()
    {
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }

    protected virtual void OnDisable()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }

    private void OnSwitchToPlay(SwitchToPlayEvent evt) => Hide();

    private void OnSwitchToEdit(SwitchToEditEvent evt) => Show();

    public void Show()
    {
        gameObject.SetActive(true);
        UpdatePositionToMouse();
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void UpdatePositionToMouse()
    {
        if (followMouseComponent == null) return;

        FollowMouse followMouse = followMouseComponent.GetFollowMouseComponent();
        if (followMouse == null) return;

        transform.position = FollowMouse.GetCurrentMouseWorldPos(followMouse.WorldPosition);
    }
}