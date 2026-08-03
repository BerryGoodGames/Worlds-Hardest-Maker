using System.Collections.Generic;

public interface IRecordingFrameStorage
{
    public IReadOnlyList<RecordingFrame> RecordedPositions { get; }
    
    public void SetFrame(int i, RecordingFrame newFrame);
}