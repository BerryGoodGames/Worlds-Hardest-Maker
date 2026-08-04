using UnityEngine;

public readonly struct AnalyzedRecordingFrame : IRecordingFrame
{
    public Vector2 Position { get; init; }
    public bool Died { get; init; }
    public bool CheckpointHit { get; init; }
    public bool StartsSuccessfulRun { get; init; }
    
    public override string ToString() => $"{{position: {Position}, died: {Died}, checkpoint hit: {CheckpointHit}, start successful run: {StartsSuccessfulRun}}}";
}