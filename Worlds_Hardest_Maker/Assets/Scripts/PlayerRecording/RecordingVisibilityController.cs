using System;

public class RecordingVisibilityController : IDisposable
{
    private readonly RecordingRenderingController renderingController;
    private readonly EventBus eventBus;
    
    public RecordingVisibilityController(RecordingRenderingController renderingController, EventBus eventBus)
    {
        this.renderingController = renderingController;
        this.eventBus = eventBus;
        
        eventBus.Subscribe<TogglePlayerRecordingPathVisibilityRequest>(OnTogglePathVisibility);
        eventBus.Subscribe<TogglePlayerRecordingOnionVisibilityRequest>(OnToggleOnionVisibility);
    }

    public void Dispose()
    {
        eventBus.Unsubscribe<TogglePlayerRecordingPathVisibilityRequest>(OnTogglePathVisibility);
        eventBus.Unsubscribe<TogglePlayerRecordingOnionVisibilityRequest>(OnToggleOnionVisibility);
    }

    private void OnToggleOnionVisibility(TogglePlayerRecordingOnionVisibilityRequest req)
    {
        SetOnionVisible(!renderingController.IsOnionActive());
    }

    // TODO: this does more than setting visibility
    public void SetOnionVisible(bool visible)
    {
        renderingController.SetOnionActive(visible);
        
        renderingController.StopOnion();
        
        if (visible) renderingController.RenderOnion();
    }
    
    private void OnTogglePathVisibility(TogglePlayerRecordingPathVisibilityRequest req)
    {
        SetPathVisible(!renderingController.IsPathActive());
    }

    public void SetPathVisible(bool visible)
    {
        renderingController.SetPathActive(visible);
        
        renderingController.StopPath();
        
        if (visible) renderingController.RenderPath();
    }
}