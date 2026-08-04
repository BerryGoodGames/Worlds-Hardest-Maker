using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[Serializable]
public class RecordingRenderLoop
{
    [SerializeField] private bool fixedDisplayDuration;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), true)] private float displaySpeed = 4;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), false)] private float displayDuration = 1;
    [SerializeField] [PositiveValueOnly] private float recordingFrequency = 0.05f;
    
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