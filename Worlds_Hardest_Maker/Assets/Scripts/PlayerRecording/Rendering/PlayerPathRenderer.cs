using System;
using System.Collections;
using System.Collections.Generic;
using LuLib.Color;
using LuLib.Transform;
using MyBox;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class PlayerPathRenderer
{
    private const float VALUE_SHIFT = 0.3090169945f;
    
    private IRecordingFrameStorage<AnalyzedRecordingFrame> frameStorage;
    [SerializeField] private Transform recordingPathContainer;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color deathColor = Color.red;
    [SerializeField] [OverrideLabel("Min Value")] [Range(0, 1)] private float minDeathColorValue = 0.5f;
    [SerializeField] private GameObject recordingDeathPrefab;
    [SerializeField] private LineRenderer recordingLinePrefab;
    
    private LineRenderer lineRenderer;
    private Color lineColor;

    public void SetFrameStorage(IRecordingFrameStorage<AnalyzedRecordingFrame> frameStorage)
    {
        this.frameStorage = frameStorage;
    }

    public void SetActive(bool active)
    {
        recordingPathContainer.gameObject.SetActive(active);
    }

    public bool IsActive()
    {
        return recordingPathContainer.gameObject.activeSelf;
    }

    public void Clear()
    {
        recordingPathContainer.DestroyChildren();
    }

    public IEnumerator RenderPathRecording(RecordingRenderLoop renderLoop)
    {
        if (frameStorage == null)
        {
            Debug.LogWarning("Frame storage not assigned.");
            yield break;
        }
        
        IReadOnlyList<AnalyzedRecordingFrame> frames = frameStorage.Frames;
        
        recordingPathContainer.DestroyChildren();
        
        BeginNewLine();
        
        lineRenderer.startColor = deathColor;
        lineRenderer.endColor = deathColor;

        yield return renderLoop.Play(
            (positions, i) =>
            {
                AnalyzedRecordingFrame currentFrame = positions[i];
                
                // display line
                AddLinePosition(currentFrame.Position);

                // if player dies or hits checkpoint and then will die, begin new red line 
                if (currentFrame.Died || (currentFrame.CheckpointHit && !currentFrame.StartsSuccessfulRun))
                {
                    if (currentFrame.Died)
                    {
                        Object.Instantiate(recordingDeathPrefab, currentFrame.Position, Quaternion.identity, recordingPathContainer);
                    }

                    // calculate new color
                    float value = 1;

                    if (minDeathColorValue < 1)
                    {
                        value = (lineRenderer.startColor.GetHSV().z + VALUE_SHIFT) % (1 - minDeathColorValue) + minDeathColorValue;
                    }

                    Color newColor = Color.red.SetValue(value);

                    newColor.a = deathColor.a;

                    BeginNewLine();

                    lineRenderer.startColor = newColor;
                    lineRenderer.endColor = newColor;

                    if (currentFrame.CheckpointHit) AddLinePosition(currentFrame.Position);
                }

                // change color to green when successful run starts
                if (currentFrame.StartsSuccessfulRun && !currentFrame.CheckpointHit)
                {
                    BeginNewLine();

                    lineRenderer.startColor = successColor;
                    lineRenderer.endColor = successColor;
                }

                // TODO: re add the event firing (broken)
                // eventBus.Fire(new RecordingPathRenderUpdateEvent(currentFrame.Position));
                //
                // if (IsReplaying && i == positions.Count - 1)
                // {
                //     eventBus.Fire(new FinishReplayEvent());
                // }
            }, frames
        );
    }
    
    private void AddLinePosition(Vector2 position)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
    }
    
    private void BeginNewLine()
    {
        lineRenderer = Object.Instantiate(recordingLinePrefab, recordingPathContainer);
    }
}