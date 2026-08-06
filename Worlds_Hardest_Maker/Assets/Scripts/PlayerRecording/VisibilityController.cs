using System;
using WorldsHardestMaker.PlayerRecording.Rendering;

namespace WorldsHardestMaker.PlayerRecording
{
    public class VisibilityController : IDisposable
    {
        private readonly RenderingController renderingController;
        private readonly EventBus eventBus;
    
        public VisibilityController(RenderingController renderingController, EventBus eventBus)
        {
            this.renderingController = renderingController;
            this.eventBus = eventBus;
        
            eventBus.Subscribe<TogglePathVisibilityRequest>(OnTogglePathVisibility);
            eventBus.Subscribe<ToggleOnionVisibilityRequest>(OnToggleOnionVisibility);
        }

        public void Dispose()
        {
            eventBus.Unsubscribe<TogglePathVisibilityRequest>(OnTogglePathVisibility);
            eventBus.Unsubscribe<ToggleOnionVisibilityRequest>(OnToggleOnionVisibility);
        }

        private void OnToggleOnionVisibility(ToggleOnionVisibilityRequest req)
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
    
        private void OnTogglePathVisibility(TogglePathVisibilityRequest req)
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
}