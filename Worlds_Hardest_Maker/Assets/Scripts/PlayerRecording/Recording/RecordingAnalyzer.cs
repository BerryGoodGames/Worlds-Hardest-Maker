using System;
using System.Collections.Generic;

namespace WorldsHardestMaker.PlayerRecording.Recording
{
    public class RecordingAnalyzer : IRecordingFrameStorage<AnalyzedFrame>
    {
        private AnalyzedFrame[] analyzedFrames = Array.Empty<AnalyzedFrame>();
        public IReadOnlyList<AnalyzedFrame> Frames => analyzedFrames;

        public void AnalyzeFrames(IRecordingFrameStorage<RawFrame> frameStorage)
        {
            IReadOnlyList<RawFrame> recordedPositions = frameStorage.Frames;
        
            analyzedFrames = new AnalyzedFrame[recordedPositions.Count];
        
            // mark successful runs
            bool currentlyInSuccessfulSegment = true;
            for (int i = recordedPositions.Count - 1; i >= 0; i--)
            {
                RawFrame frame = recordedPositions[i];
                
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
}