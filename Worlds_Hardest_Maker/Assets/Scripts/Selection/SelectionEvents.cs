using UnityEngine;

namespace WorldsHardestMaker.Selection
{
    public class SelectionStartedEvent
    {
        public readonly Vector2 Start;
        public SelectionStartedEvent(Vector2 start) => Start = start;
    }

    public class SelectionUpdatedEvent
    {
        public readonly Vector2 Start;
        public readonly Vector2 End;
        public SelectionUpdatedEvent(Vector2 start, Vector2 end) { Start = start; End = end; }
    }

    public class SelectionEndedEvent
    {
        public readonly Vector2 Start;
        public readonly Vector2 End;
        public SelectionEndedEvent(Vector2 start, Vector2 end) { Start = start; End = end; }
    }

    public class SelectionCancelledEvent { }

    public class SelectionClearedEvent { }
}