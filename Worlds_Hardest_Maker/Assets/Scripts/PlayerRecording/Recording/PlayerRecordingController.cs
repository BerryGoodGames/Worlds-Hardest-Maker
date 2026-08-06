using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerRecordingController : IRecordingFrameStorage<RawRecordingFrame>
{
    [SerializeField] private PlayerRecorder recorder;
    private Coroutine recording;
    private MonoBehaviour runner;

    public IReadOnlyList<RawRecordingFrame> Frames => recorder.Frames;
    
    public PlayerRecordingController(PlayerRecorder recorder, MonoBehaviour runner)
    {
        this.recorder = recorder;
        this.runner = runner;
    }

    public void SetRunner(MonoBehaviour runner)
    {
        this.runner = runner;
    }

    public void StartRecording()
    {
        StopRecording();
        recording = runner.StartCoroutine(recorder.RecordPlayer());
    }

    public void StopRecording()
    {
        if (recording != null)
        {
            runner.StopCoroutine(recording);
            recording = null;
        }
    }
}