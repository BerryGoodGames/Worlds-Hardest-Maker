using UnityEngine;
using VContainer;

public class SelectionState : ISelectionState
{
    public bool IsSelecting { get; private set; }
    public Vector2? Start { get; private set; }
    public Vector2? End { get; private set; }

    private readonly EventBus eventBus;

    public SelectionState(EventBus eventBus)
    {
        this.eventBus = eventBus;
    }
    
    public void StartSelection(Vector2 start)
    {
        IsSelecting = true;
        Start = start;
        End = start;
        eventBus.Fire(new SelectionStartedEvent(start));
    }

    public void UpdateSelection(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
        eventBus.Fire(new SelectionUpdatedEvent(start, end));
    }

    public void EndSelection(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
        eventBus.Fire(new SelectionEndedEvent(start, end));
    }

    public void CancelSelection()
    {
        Start = null;
        End = null;
        IsSelecting = false;
        eventBus.Fire(new SelectionCancelledEvent());
    }
}