namespace WorldsHardestMaker.PlayerRecording
{
    public interface IRecordingService
    {
        public bool IsReplaying { get; set; }
        
        public void SetPathVisible(bool visible);
        public void SetOnionVisible(bool visible);
        
    }
}