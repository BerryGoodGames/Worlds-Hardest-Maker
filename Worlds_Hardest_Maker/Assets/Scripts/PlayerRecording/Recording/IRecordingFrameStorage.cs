using System.Collections.Generic;

namespace WorldsHardestMaker.PlayerRecording.Recording
{
    public interface IRecordingFrameStorage<out T> where T : IRecordingFrame
    {
        public IReadOnlyList<T> Frames { get; }
    }
}