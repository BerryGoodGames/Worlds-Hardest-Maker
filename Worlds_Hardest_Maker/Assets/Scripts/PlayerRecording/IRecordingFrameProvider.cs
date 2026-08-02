using System.Collections.Generic;

public interface IRecordingFrameProvider
{
    public List<RecordingFrame> RecordedPositions { get; }
}