using UnityEngine;

// TODO: make enum instead of flags
public struct RecordingFrame
{
    public Vector2 Position { get; init; }
    public bool Died { get; init; }
    public bool CheckpointHit { get; init; }
    public bool StartSuccessfulLine { get; init; }
        
    public override string ToString() =>
        $"{{position: {Position}, died: {Died}, checkpoint hit: {CheckpointHit}, start successful line: {StartSuccessfulLine}}}";

}