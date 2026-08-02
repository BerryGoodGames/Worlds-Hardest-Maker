using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public abstract class RecordingRenderer
{
    protected IRecordingFrameProvider FrameProvider { get; private set; }

    private bool fixedDisplayDuration;
    private float displayDuration;
    private float displaySpeed;
    private float recordingFrequency;

    public RecordingRenderer(IRecordingFrameProvider frameProvider)
    {
        FrameProvider = frameProvider;
    }
    
    protected IEnumerator RenderLoop(Action<int> action, int startIndex = 0)
    {
        List<RecordingFrame> recordedPositions = FrameProvider.RecordedPositions;
        if (recordedPositions.IsNullOrEmpty()) yield break;
        
        float displayDelay = fixedDisplayDuration
            ? displayDuration / recordedPositions.Count
            : recordingFrequency / displaySpeed;
        
        for (int i = startIndex; i < recordedPositions.Count; i++)
        {
            action.Invoke(i);
            
            // wait delay
            yield return new WaitForSecondsRealtime(displayDelay);
        }
    }
}