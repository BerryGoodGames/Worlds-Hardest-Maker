using UnityEngine;
using VContainer;

/// <summary>
///     Handles event-driven state changes for placement preview.
///     Responds to edit mode transitions and activation events.
/// </summary>
public class PreviewEventComponent : MonoBehaviour
{
    [Inject] private EventBus eventBus;

    private void Start()
    {
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }

    private void OnSwitchToPlay(SwitchToPlayEvent evt) => Hide();

    private void OnSwitchToEdit(SwitchToEditEvent evt) => Show();

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
}