public interface IResettable
{
    void ResetState();
    
    public sealed void Subscribe(EventBus eventBus)
    {
        eventBus.Subscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
    
    public sealed void Unsubscribe(EventBus eventBus)
    {
        eventBus.Unsubscribe<ResetLevelEvent>(OnResetLevel);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
    
    private void OnResetLevel(ResetLevelEvent evt) => ResetState();
    private void OnSwitchToEdit(SwitchToEditEvent evt) => ResetState();
}