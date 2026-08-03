using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class RecordingRenderLoop
{
    private readonly bool fixedDisplayDuration;
    private readonly float displayDuration;
    private readonly float displaySpeed;
    private readonly float recordingFrequency;
    
    public RecordingRenderLoop(bool fixedDisplayDuration, float displayDuration, float displaySpeed, float recordingFrequency)
    {
        this.fixedDisplayDuration = fixedDisplayDuration;
        this.displayDuration = displayDuration;
        this.displaySpeed = displaySpeed;
        this.recordingFrequency = recordingFrequency;
    }
    
    public IEnumerator Play(Action<IReadOnlyList<RecordingFrame>, int> action, IReadOnlyList<RecordingFrame> recordedPositions, int startIndex = 0)
    {
        if (recordedPositions.IsNullOrEmpty()) yield break;
        
        float displayDelay = fixedDisplayDuration
            ? displayDuration / recordedPositions.Count
            : recordingFrequency / displaySpeed;
        
        for (int i = startIndex; i < recordedPositions.Count; i++)
        {
            action.Invoke(recordedPositions, i);
            
            // wait delay
            yield return new WaitForSecondsRealtime(displayDelay);
        }
    }
}