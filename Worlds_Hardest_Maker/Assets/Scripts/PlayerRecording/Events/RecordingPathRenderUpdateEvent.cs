using UnityEngine;

public class RecordingPathRenderUpdateEvent
{
    public Vector2 Position { get; private set; }
    
    public RecordingPathRenderUpdateEvent(Vector2 position)
    {
        Position = position;
    }
}