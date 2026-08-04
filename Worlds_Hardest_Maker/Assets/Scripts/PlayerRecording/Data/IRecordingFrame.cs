using UnityEngine;

public interface IRecordingFrame
{
    public Vector2 Position { get; init; }
    public bool Died { get; init; }
    public bool CheckpointHit { get; init; }
}