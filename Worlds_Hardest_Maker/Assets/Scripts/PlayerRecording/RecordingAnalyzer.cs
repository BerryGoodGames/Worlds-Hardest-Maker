public class RecordingAnalyzer
{
    public void Analyze()
    {
        // mark successful runs
        bool successful = true;
        for (int i = recordedPositions.Count - 1; i >= 0; i--)
        {
            RecordingFrame frame = recordedPositions[i];
            if (successful && frame.Died && i != recordedPositions.Count - 1)
            {
                frame.StartSuccessfulLine = true;
                successful = false;
            }

            if (recordedPositions[i].CheckpointHit)
            {
                if (successful) frame.StartSuccessfulLine = true;

                successful = true;
            }

            if (i == 0 && successful) frame.StartSuccessfulLine = true;

            recordedPositions[i] = frame;
        }
    }
}