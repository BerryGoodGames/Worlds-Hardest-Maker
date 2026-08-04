using UnityEngine;
using VContainer;

public class PlayerRecordingSpriteButton : MonoBehaviour
{
    private EventBus eventBus;

    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public void OnToggleSpriteVisibilityClicked()
    {
        eventBus.Fire(new TogglePlayerRecordingSpriteVisibilityRequest());
    }
}