using UnityEngine;

public readonly struct SelectionStartedEvent
{
    public readonly Vector2 Start;
    public SelectionStartedEvent(Vector2 start) => Start = start;
}

public readonly struct SelectionUpdatedEvent
{
    public readonly Vector2 Start;
    public readonly Vector2 End;
    public SelectionUpdatedEvent(Vector2 start, Vector2 end) { Start = start; End = end; }
}

public readonly struct SelectionEndedEvent
{
    public readonly Vector2 Start;
    public readonly Vector2 End;
    public SelectionEndedEvent(Vector2 start, Vector2 end) { Start = start; End = end; }
}

public readonly struct SelectionCancelledEvent { }