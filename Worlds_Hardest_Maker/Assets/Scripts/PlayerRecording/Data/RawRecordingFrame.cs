using UnityEngine;

// TODO: make enum instead of flags
public readonly struct RawRecordingFrame : IRecordingFrame
{
    public Vector2 Position { get; init; }
    public bool Died { get; init; }
    public bool CheckpointHit { get; init; }
        
    public override string ToString() => $"{{position: {Position}, died: {Died}, checkpoint hit: {CheckpointHit}}}";

}