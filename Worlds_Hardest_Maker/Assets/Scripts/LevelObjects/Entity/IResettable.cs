public interface IResettable
{
    void ResetState();
    
    public sealed void Subscribe(EventBus eventBus)
    {
        eventBus.Subscribe<ResetLevelEvent>(_ => ResetState());
        eventBus.Subscribe<SwitchToEditEvent>(_ => ResetState());
    }
    
    public sealed void Unsubscribe(EventBus eventBus)
    {
        eventBus.Unsubscribe<ResetLevelEvent>(_ => ResetState());
        eventBus.Unsubscribe<SwitchToEditEvent>(_ => ResetState());
    }
}