using UnityEngine;

namespace WorldsHardestMaker.PlayerRecording
{
    public class PathRenderUpdateEvent
    {
        public Vector2 Position { get; private set; }
    
        public PathRenderUpdateEvent(Vector2 position)
        {
            Position = position;
        }
    }
}