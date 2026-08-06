using UnityEngine;
using VContainer;
using WorldsHardestMaker.PlayerRecording;

public class PlayerRecordingPathButton : MonoBehaviour
{
    private EventBus eventBus;

    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public void OnTogglePathVisibilityClicked()
    {
        eventBus.Fire(new TogglePathVisibilityRequest());
    }
}