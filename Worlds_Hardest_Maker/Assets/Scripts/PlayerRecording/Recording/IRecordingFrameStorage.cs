using System.Collections.Generic;

public interface IRecordingFrameStorage<out T> where T : IRecordingFrame
{
    public IReadOnlyList<T> Frames { get; }
}