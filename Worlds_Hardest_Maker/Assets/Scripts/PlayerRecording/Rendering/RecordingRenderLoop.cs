using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[Serializable]
public class RecordingRenderLoop
{
    [SerializeField] private bool fixedDisplayDuration;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), true)] [PositiveValueOnly] private float displayFrequency = 0.0125f;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), false)] [PositiveValueOnly] private float displayDuration = 1;
    
    public IEnumerator Play<T>(Action<IReadOnlyList<T>, int> action, IReadOnlyList<T> recordedPositions, int startIndex = 0) where T : IRecordingFrame
    {
        if (recordedPositions.IsNullOrEmpty()) yield break;
        
        float displayDelay = fixedDisplayDuration
            ? displayDuration / recordedPositions.Count
            : displayFrequency;
        
        for (int i = startIndex; i < recordedPositions.Count; i++)
        {
            action.Invoke(recordedPositions, i);
            
            // wait delay
            yield return new WaitForSecondsRealtime(displayDelay);
        }
    }
}