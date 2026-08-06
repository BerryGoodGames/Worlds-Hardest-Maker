using UnityEngine;

namespace WorldsHardestMaker.PlayerRecording
{
    // TODO: make enum instead of flags
    public readonly struct RawFrame : IRecordingFrame
    {
        public Vector2 Position { get; init; }
        public bool Died { get; init; }
        public bool CheckpointHit { get; init; }
        
        public override string ToString() => $"{{position: {Position}, died: {Died}, checkpoint hit: {CheckpointHit}}}";
    }
}