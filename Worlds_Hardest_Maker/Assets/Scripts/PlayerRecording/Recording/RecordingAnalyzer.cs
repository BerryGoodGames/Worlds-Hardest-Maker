using System.Collections.Generic;

public class RecordingAnalyzer
{
    public void AnalyzeFrames(IRecordingFrameStorage frameStorage)
    {
        
        // TODO: use analyzed frame
        IReadOnlyList<RecordingFrame> recordedPositions = frameStorage.RecordedPositions;
        
        // mark successful runs
        bool currentlyInSuccessfulSegment = true;
        for (int i = recordedPositions.Count - 1; i >= 0; i--)
        {
            RecordingFrame frame = recordedPositions[i];
            bool startSuccessfulLine = frame.StartSuccessfulLine;
            if (currentlyInSuccessfulSegment && frame.Died && i != recordedPositions.Count - 1)
            {
                startSuccessfulLine = true;
                currentlyInSuccessfulSegment = false;
            }

            if (recordedPositions[i].CheckpointHit)
            {
                if (currentlyInSuccessfulSegment) startSuccessfulLine = true;

                currentlyInSuccessfulSegment = true;
            }

            if (i == 0 && currentlyInSuccessfulSegment) startSuccessfulLine = true;
            
            frameStorage.SetFrame(i, new()
            {
                StartSuccessfulLine = startSuccessfulLine,
                CheckpointHit = frame.CheckpointHit,
                Died = frame.Died,
                Position = frame.Position,
            });
        }
    }
}