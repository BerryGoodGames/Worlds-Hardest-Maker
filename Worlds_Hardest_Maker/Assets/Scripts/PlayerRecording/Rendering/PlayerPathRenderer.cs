using System.Collections;
using System.Collections.Generic;
using LuLib.Color;
using LuLib.Transform;
using UnityEngine;

public class PlayerPathRenderer
{
    private const float VALUE_SHIFT = 0.3090169945f;
    
    private readonly IRecordingFrameStorage frameStorage;
    private readonly Transform recordingPathContainer;
    private readonly Color successColor;
    private readonly Color deathColor;
    private readonly float minDeathColorValue;
    private readonly GameObject recordingDeathPrefab;
    private readonly LineRenderer recordingLinePrefab;
    
    private LineRenderer lineRenderer;
    private Color lineColor;

    public PlayerPathRenderer(
        IRecordingFrameStorage frameStorage, 
        Transform recordingPathContainer, 
        Color successColor, Color deathColor,
        float minDeathColorValue,
        GameObject recordingDeathPrefab,
        LineRenderer recordingLinePrefab)
    {
        this.frameStorage = frameStorage;
        this.recordingPathContainer = recordingPathContainer;
        this.successColor = successColor;
        this.deathColor = deathColor;
        this.minDeathColorValue = minDeathColorValue;
        this.recordingDeathPrefab = recordingDeathPrefab;
        this.recordingLinePrefab = recordingLinePrefab;
    }

    public IEnumerator RenderPathRecording(RecordingRenderLoop renderLoop)
    {
        IReadOnlyList<RecordingFrame> recordedPositions = frameStorage.RecordedPositions;
        
        recordingPathContainer.DestroyChildren();
        
        BeginNewLine();
        
        lineRenderer.startColor = deathColor;
        lineRenderer.endColor = deathColor;

        return renderLoop.Play(
            (positions, i) =>
            {
                RecordingFrame currentFrame = positions[i];
                
                // display line
                AddLinePosition(currentFrame.Position);

                // if player dies or hits checkpoint and then will die, begin new red line 
                if (currentFrame.Died || (currentFrame.CheckpointHit && !currentFrame.StartSuccessfulLine))
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
                if (currentFrame.StartSuccessfulLine && !currentFrame.CheckpointHit)
                {
                    BeginNewLine();

                    lineRenderer.startColor = successColor;
                    lineRenderer.endColor = successColor;
                }

                // eventBus.Fire(new RecordingPathRenderUpdateEvent(currentFrame.Position));
                //
                // if (IsReplaying && i == positions.Count - 1)
                // {
                //     eventBus.Fire(new FinishReplayEvent());
                // }
            }, recordedPositions
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