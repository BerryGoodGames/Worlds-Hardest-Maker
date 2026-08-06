namespace WorldsHardestMaker.PlayerRecording
{
    public interface IRecordingService
    {
        public bool IsReplaying { get; }

        public void StopReplay();
        
        public void SetPathVisible(bool visible);
        public void SetOnionVisible(bool visible);
    }
}