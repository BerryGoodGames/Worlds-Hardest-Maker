using System.Collections.Generic;

public class RecordingAnalyzer : IRecordingFrameStorage<AnalyzedRecordingFrame>
{
    private AnalyzedRecordingFrame[] analyzedFrames;
    public IReadOnlyList<AnalyzedRecordingFrame> Frames => analyzedFrames;
    
    public void AnalyzeFrames(IRecordingFrameStorage<RawRecordingFrame> frameStorage)
    {
        IReadOnlyList<RawRecordingFrame> recordedPositions = frameStorage.Frames;
        
        analyzedFrames = new AnalyzedRecordingFrame[recordedPositions.Count];
        
        // mark successful runs
        bool currentlyInSuccessfulSegment = true;
        for (int i = recordedPositions.Count - 1; i >= 0; i--)
        {
            RawRecordingFrame frame = recordedPositions[i];
            bool startSuccessfulRun = false;
            if (currentlyInSuccessfulSegment && frame.Died && i != recordedPositions.Count - 1)
            {
                startSuccessfulRun = true;
                currentlyInSuccessfulSegment = false;
            }

            if (recordedPositions[i].CheckpointHit)
            {
                if (currentlyInSuccessfulSegment) startSuccessfulRun = true;

                currentlyInSuccessfulSegment = true;
            }

            if (i == 0 && currentlyInSuccessfulSegment) startSuccessfulRun = true;
            
            analyzedFrames[i] = new()
            {
                Position = frame.Position,
                Died = frame.Died,
                CheckpointHit = frame.CheckpointHit,
                StartsSuccessfulRun = startSuccessfulRun,
            };
        }
    }
}